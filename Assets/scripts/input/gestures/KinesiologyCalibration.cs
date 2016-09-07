using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

using Controller = VRTK.VRTK_ControllerEvents;

public class KinesiologyCalibration {
  private const float MIN_LR_CALC_THRESHOLD = 1f;

  public static string exported = "no";

  private static SteamVR_Camera mVrCamera = null;
  public static SteamVR_Camera vrCamera {
    get {
      if (mVrCamera == null) {
        mVrCamera = GameObject.FindObjectOfType<SteamVR_Camera>();
      }

      return mVrCamera;
    }
    set {
      mVrCamera = value;
    }
  }

  private static Controller[] mControllers = null;
  private static Controller[] controllers {
    get {
      if (mControllers == null) {
        mControllers = GameObject.FindObjectsOfType<Controller>();
      }

      return mControllers;
    }
    set {
      mControllers = value;
    }
  }

  // TODO: Properly handle the case where we don't have any confidence one way or the other.
  public static Controller left {
    get {
      return (left0Confidence > 0f ? controllers[0] : controllers[1]);
    }
  }

  public static Controller right {
    get {
      return (left0Confidence <= 0f ? controllers[0] : controllers[1]);
    }
  }

  private static SortedFloatList playerHeightList = new SortedFloatList();
  private static SortedFloatList playerArmspanList = new SortedFloatList();
  private static SortedFloatList playerShoulderHeightList = new SortedFloatList();
  private static SortedFloatList playerTorsoWidthList = new SortedFloatList();

  /**
   * Used to distinguish between left and right controllers.
   * 
   * @returns the confidence level that controller at index 0 is the left controller.
   */
  public static float left0Confidence { get; private set; }

  private KinesiologyCalibration() {
    throw new Exception("Constructor not implemented");
  }

  public static void preload(Kinesiology kinesiology) {
    playerHeightList.Add(kinesiology.eyesHeight);
    playerArmspanList.Add(kinesiology.armSpan);
    playerShoulderHeightList.Add(kinesiology.shoulderHeight);
    playerTorsoWidthList.Add(kinesiology.torsoWidth);
  }

  /**
   * Clears all the calibration data.  Use this before calibrating another user.
   */
  public static void clearAll() {
    playerHeightList.Clear();
    playerArmspanList.Clear();
    playerShoulderHeightList.Clear();
    playerTorsoWidthList.Clear();

    left0Confidence = 0f;
    exported = "no";
  }

  /**
   * Best if used while the player is standing upright and facing forward.  Records the current 
   * headset height as the player height.
   */
  public static void recordPlayerHeight() {
    playerHeightList.Add(calculatePlayerHeight());
  }

  /**
   * @returns the height of the player based on the current headset height.
   */
  public static float calculatePlayerHeight() {
    return getInstantaneousRaycastToGround(vrCamera.transform);
  }

  /**
   * @returns the median value from the list of recorded headset heights.
   */
  public static float getCurrentPlayerHeight() {
    return playerHeightList.median;
  }

  /**
   * Best if used while the player is in a t pose.  Records the distance between the controllers
   * as the armspan.
   */
  public static void recordPlayerArmspan() {
    playerArmspanList.Add(calculateCurrentPlayerArmspan());
  }

  /**
   * @returns the current distance between the controllers.
   */
  public static float calculateCurrentPlayerArmspan() {
    return calculateCurrentDistanceBetweenControllers();
  }

  /**
   * @returns the median value from the list of recorded armspans.
   */
  public static float getCurrentPlayerArmspan() {
    return playerArmspanList.median;
  }

  /**
   * Best if used while the player is in a t pose.  Records the average distance between the controllers
   * and the ground as the shoulder height.
   */
  public static void recordPlayerShoulderHeight() {
    playerShoulderHeightList.Add(calculatePlayerShoulderHeight());
  }

  /**
   * @returns the current average distance between the controllers and the ground
   */
  public static float calculatePlayerShoulderHeight() {
    float c1dist = getInstantaneousRaycastToGround(controllers[0].transform);
    float c2dist = getInstantaneousRaycastToGround(controllers[1].transform);

    return (c1dist + c2dist) / 2f;
  }

  /**
   * @returns the median value from the list of recorded shoulder heights.
   */
  public static float getCurrentPlayerShoulderHeight() {
    return playerShoulderHeightList.median;
  }

  /**
   * Best if used while player is in a pencil pose.  Records the distance between the controllers
   * as the torso width.
   */
  public static void recordPlayerTorsoWidth() {
    playerTorsoWidthList.Add(calculatePlayerTorsoWidth());
  }

  /**
   * @returns the current distance between the controllers.
   */
  public static float calculatePlayerTorsoWidth() {
    return calculateCurrentDistanceBetweenControllers();
  }

  /**
   * @returns the median value from the list of recorded shoulder heights.
   */
  public static float getCurrentPlayerTorsoWidth() {
    return playerTorsoWidthList.median;
  }

  /**
   * @returns the current distance between the controllers.
   */
  public static float calculateCurrentDistanceBetweenControllers() {
    return Vector3.Distance(controllers[0].transform.position, controllers[1].transform.position);
  }

  /**
   * Tracks the confidence in whether the first controller is the left or right controller.
   */
  public static void recordLeftRight() {
    if (calculateCurrentPlayerArmspan() > MIN_LR_CALC_THRESHOLD) {
      if (
        between(15f, cameraControllerAngle(controllers[0]), 100f) &&
        between(15f, cameraControllerAngle(controllers[1]), 100f)
        ) {
        if (controllerIsLeft(controllers[0])) {
          ++left0Confidence;
        } else if (controllerIsRight(controllers[0])) {
          --left0Confidence;
        }
      }
    }
  }

  public static float cameraControllerAngle(Controller controller) {
    Vector3 cameraXZ = new Vector3(vrCamera.transform.position.x, controller.transform.position.y, vrCamera.transform.position.z);

    return Vector3.Angle(vrCamera.transform.forward, controller.transform.position - cameraXZ);
  }

  public static bool controllerIsLeft(Controller controller) {
    Vector3 cameraXZ = new Vector3(vrCamera.transform.position.x, controller.transform.position.y, vrCamera.transform.position.z);

    float angleFromForward = Vector3.Angle(vrCamera.transform.forward, controller.transform.position - cameraXZ);
    float angleFromLeft = Vector3.Angle(-vrCamera.transform.right, controller.transform.position - cameraXZ);

    return angleFromForward <= 100f && angleFromLeft <= 75f;
  }

  public static bool controllerIsRight(Controller controller) {
    Vector3 cameraXZ = new Vector3(vrCamera.transform.position.x, controller.transform.position.y, vrCamera.transform.position.z);

    float angleFromForward = Vector3.Angle(vrCamera.transform.forward, controller.transform.position - cameraXZ);
    float angleFromRight = Vector3.Angle(vrCamera.transform.right, controller.transform.position - cameraXZ);

    return angleFromForward <= 100f && angleFromRight <= 75f;
  }

  public static float getInstantaneousRaycastToGround(Transform t, float maxDistance = 4f) {
    RaycastHit raycastHit;

    Physics.Raycast(t.position, -Vector3.up, out raycastHit, maxDistance, LayerMask.GetMask("floor"));

    return raycastHit.distance;
  }

  /** Other Helpers **/
  public static bool between(float min, float val, float max) {
    return (min <= val) && (val <= max);
  }

  public static Kinesiology exportToKinesiology() {
    Kinesiology kinesiology = new Kinesiology();

    kinesiology.eyesHeight = KinesiologyCalibration.getCurrentPlayerHeight();
    kinesiology.armSpan = KinesiologyCalibration.getCurrentPlayerArmspan();

    // Don't overwrite an existing armLength in case we loaded.
    if (kinesiology.armLength == 0f) {
      kinesiology.armLength = (KinesiologyCalibration.getCurrentPlayerArmspan() - KinesiologyCalibration.getCurrentPlayerTorsoWidth()) / 2;
    }

    kinesiology.torsoWidth = KinesiologyCalibration.getCurrentPlayerTorsoWidth();
    kinesiology.shoulderHeight = KinesiologyCalibration.getCurrentPlayerShoulderHeight();
    kinesiology.creationDate = new DateTime();

    exported = "yes";

    return kinesiology;
  }
}
