using UnityEngine;
using System.Collections;

public class Kinesiology {
  public System.Guid playerId;
  public string playerName;
  public float armSpan;
  public float shoulderHeight;
  public float armLength;
  public float eyesHeight;

  public Kinesiology() {
    playerId = new System.Guid();
    playerName = "";
    armSpan = 0f;
    shoulderHeight = 0f;
    armLength = 0f;
    eyesHeight = 0f;
  }

  // Do not serialize
  private SteamVR_TrackedController[] controllers = new SteamVR_TrackedController[2];
  public SteamVR_TrackedController left {
    get {
      return controllers[0];
    }
    private set {
      controllers[0] = value;
    }
  }

  public SteamVR_TrackedController right {
    get {
      return controllers[1];
    }
    private set {
      controllers[1] = value;
    }
  }

  public SteamVR_Camera vrCamera { get; private set; }

  public const int KINESIOLOGY_VERSION = 1;

  public string tryDetectSteamVR() {
    if (eyesHeight == 0f && armSpan == 0f) {
      return "eyesHeight or armSpan must be set before detecting steamVR";
    }

    SteamVR_Camera[] cameras = GameObject.FindObjectsOfType<SteamVR_Camera>();

    if (cameras.Length != 1) {
      return "Found an unexpected number of cameras: " + cameras.Length;
    }

    vrCamera = cameras[0];

    SteamVR_TrackedController[] lControllers = GameObject.FindObjectsOfType<SteamVR_TrackedController>();

    if (lControllers.Length != 2) {
      return "Found an unexpected number of controllers: " + lControllers.Length;
    }

    SteamVR_TrackedController c1 = lControllers[0];
    SteamVR_TrackedController c2 = lControllers[1];

    float tempArmSpan = armSpan;

    if (tempArmSpan == 0f) {
      tempArmSpan = eyesHeight;
    }

    if (Vector3.Distance(c1.transform.position, c2.transform.position) < 0.8f * tempArmSpan) {
      return "Controllers are not far enough away from each other";
    }

    if (!yWithinMetersOfEyes(c1.transform, 0.5f) || !yWithinMetersOfEyes(c2.transform, 0.5f)) {
      return "One of the controllers is not within 1.5 feet of the eyes, super tall person, or not a t pose.";
    }

    Vector3 camXZ = new Vector3(vrCamera.transform.position.x, 0f, vrCamera.transform.position.z);

    camXZ.y = c1.transform.position.y;
    float angle1 = Vector3.Angle(c1.transform.position - camXZ, Vector3.up);

    camXZ.y = c2.transform.position.y;
    float angle2 = Vector3.Angle(c2.transform.position - camXZ, Vector3.up);

    if (angle1 < 80f || angle1 > 100f || angle2 < 80f || angle2 > 100f) {
      Debug.Log(
        "c1.pos: " + c1.transform.position + "\n" +
        "c2.pos: " + c2.transform.position + "\n" +
        "camXZ: " + camXZ + "\n" +
        "angle1: " + angle1 + "\n" +
        "angle2: " + angle2 + "\n"
        );
      return "Arms aren't straight enough to calculate shoulder height";
    }

    angle1 = Vector3.Angle(vrCamera.transform.right, c1.transform.position - camXZ);
    angle2 = Vector3.Angle(vrCamera.transform.right, c2.transform.position - camXZ);

    if (angle1 <= 20f) {
      if (angle2 < 160f) {
        return "Unable to differentiate between controllers";
      }

      left = c1;
      right = c2;
    } else {
      if (angle2 > 20f) {
        return "Unable to differentiate between controllers";
      }

      left = c2;
      right = c1;
    }

    armSpan = Vector3.Distance(c1.transform.position, c2.transform.position);
    shoulderHeight = (getInstantaneousRaycastToGround(c1.transform) + getInstantaneousRaycastToGround(c2.transform)) / 2f;

    return null;
  }

  public float getInstantaneousRaycastToGround(Transform t, float maxDistance = 4f) {
    RaycastHit raycastHit;

    Physics.Raycast(t.position, -Vector3.up, out raycastHit, maxDistance, LayerMask.GetMask("floor"));

    return raycastHit.distance;
  }

  public bool yWithinMetersOfEyes(Transform t, float m) {
    return Mathf.Abs(vrCamera.transform.position.y - t.position.y) <= m;
  }

  public static Kinesiology load(System.Guid mPlayerId) {
    return null;
  }

  public static void save(Kinesiology kinesiology) {
    string json = saveToJson(kinesiology);

    return;
  }

  public static string saveToJson(Kinesiology kinesiology) {
    return "";
  }
}
