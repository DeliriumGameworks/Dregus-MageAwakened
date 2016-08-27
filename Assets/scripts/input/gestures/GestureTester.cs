using UnityEngine;
using System.Collections.Generic;

public class GestureTester : MonoBehaviour {
  public Attack beamAttack;
  static SteamVR_TrackedController leftController;
  static SteamVR_TrackedController rightController;

  static SteamVR_Camera thisCamera;

  static float armLength = 1.6f / 3;
  static float headsetToCenterChest = 0.5f / 3f;

  List<Gesture> gestures = new List<Gesture>();

  GestureStep gestureStep;

  int beamRay = 0;

  private Attack beamShot;

  private CalibrationTutorial calibrationTutorial;

  // Use this for initialization
  void Start() {
    getCamera();

    GetComponent<SteamVR_TrackedController>().TriggerClicked += new ClickedEventHandler(DoTrigger);

    calibrationTutorial = FindObjectOfType<CalibrationTutorial>();
  }

  // Update is called once per frame
  void Update() {
    if (getLeftController() != null && getRightController() != null) {
      if (calibrationTutorial.currentIndex == 0) {
        calibrationTutorial.moveTo(1);
      }
    }

    if (beamRay == 0 && beamShot != null) {
      SoundEffect sfx = beamShot.gameObject.GetComponent<SoundEffect>();

      if (sfx != null) {
        sfx.audioSource.Stop();
      }
    }

    if (beamRay >= 2 && (!controllersAreOutstretched() || !controllersAreWithin(0.33f))) {
      if (beamShot == null) {
        beamRay = 0;

        return;
      }

      beamRay = 0;
      BeamParam bp = beamShot.GetComponent<BeamParam>();

      if (bp != null) {
        bp.bEnd = true;
      }

      StartCoroutine(destroyBeamShotDelayed());
    } else if (beamRay >= 2) {
      if (beamShot == null) {
        beamRay = 0;

        return;
      }

      beamShot.transform.position = beamGetOrigin();
      beamShot.transform.rotation = Quaternion.FromToRotation(Vector3.forward, beamGetOrigin() - beamGetChestLocation());
    }
  }

  System.Collections.IEnumerator destroyBeamShotDelayed() {
    yield return new WaitForSeconds(2);

    Destroy(beamShot);
  }

  void DoTrigger(object sender, ClickedEventArgs e) {
    if (!getPlayerCharacter().GetComponent<Character>().isAlive()) {
      return;
    }

    if (controllersAreWithin(0.33f) && controllersAtMostAngleFromUp(120f) && controllersAreOutstretched() && beamRay == 1) {
      beamRay = 2;

      if (beamShot != null) {
        Debug.Log("Not going to instantiate another beamShot");
        return;
      }

      beamShot = (Attack) Instantiate(beamAttack, beamGetOrigin(), Quaternion.FromToRotation(Vector3.forward, beamGetOrigin() - beamGetChestLocation()));

      SoundEffect sfx = beamShot.gameObject.GetComponent<SoundEffect>();

      if (sfx != null) {
        sfx.playLoopRandomAudioClip();
      }

      GeroBeam gb = beamShot.gameObject.GetComponent<GeroBeam>();

      if (gb != null) {
        gb.hitEvent.AddListener(gbHitEvent);
      }
    }

    if (controllersAreWithin(0.33f) && controllersAtLeastAngleFromUp(120f) && beamRay == 0) {
      beamRay = 1;

      calibrationTutorial.moveTo(2);
    }
  }

  PlayerCharacter getPlayerCharacter() {
    return FindObjectOfType<PlayerCharacter>();
  }

  void gbHitEvent(GameObject go) {
    Character character = go.GetComponent<Character>();

    if (character != null) {
      Attack.Element damageType = Attack.Element.Physical;
      beamAttack.attack(getPlayerCharacter().GetComponent<Character>(), character, damageType);
    }
  }

  static Vector3 beamGetOrigin() {
    return (getLeftController().transform.position + getRightController().transform.position) / 2;
  }

  static Vector3 beamGetChestLocation() {
    Vector3 chestLocation = (getCamera()).transform.position;

    chestLocation.y -= headsetToCenterChest;

    return chestLocation;
  }

  static bool cameraAtMostAngleFromUp(float angleFromUp) {
    return angleFromUp > Vector3.Angle(getCamera().transform.forward, Vector3.up);
  }

  static bool controllersAreWithin(float rangeOfEachOther) {
    Vector3 lPos = getLeftController().transform.position;
    Vector3 rPos = getRightController().transform.position;

    bool result = (lPos - rPos).magnitude < rangeOfEachOther;

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(rangeOfEachOther): " + result + "\n" +
      "rangeOfEachOther: " + rangeOfEachOther + "\n" +
      "lPos: " + lPos + "\n" +
      "rPos: " + rPos + "\n"
      );
      */

    return result;
  }

  static bool controllersAtLeastAngleFromUp(float angleFromUp) {
    Vector3 cPos = getCamera().transform.position;
    Vector3 lPos = getLeftController().transform.position;
    Vector3 rPos = getRightController().transform.position;

    float lTheta = Vector3.Angle(Vector3.up, lPos - cPos);
    float rTheta = Vector3.Angle(Vector3.up, rPos - cPos);

    bool result = lTheta >= angleFromUp && rTheta >= angleFromUp;

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(angleFromUp): " + result + "\n" +
      "angleFromUp: " + angleFromUp + "\n" +
      "lTheta: " + lTheta + "\n" +
      "rTheta: " + rTheta + "\n" +
      "cPos: " + cPos + "\n" +
      "lPos: " + lPos + "\n" +
      "rPos: " + rPos + "\n"
      );
      );
    */

    return result;
  }

  static bool controllersAtMostAngleFromUp(float angleFromUp) {
    Vector3 cPos = getCamera().transform.position;
    Vector3 lPos = getLeftController().transform.position;
    Vector3 rPos = getRightController().transform.position;

    float lTheta = Vector3.Angle(Vector3.up, lPos - cPos);
    float rTheta = Vector3.Angle(Vector3.up, lPos - cPos);

    bool result = lTheta <= angleFromUp && rTheta <= angleFromUp;

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(angleFromUp): " + result + "\n" +
      "angleFromUp: " + angleFromUp + "\n" +
      "lTheta: " + lTheta + "\n" +
      "rTheta: " + rTheta + "\n" +
      "cPos: " + cPos + "\n" +
      "lPos: " + lPos + "\n" +
      "rPos: " + rPos + "\n"
      );
      );
    */

    return result;
  }

  static bool controllersAreLevel() {
    float lY = getLeftController().transform.position.y;
    float rY = getRightController().transform.position.y;

    bool result = Mathf.Abs(lY - rY) <= 0.1f;

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(): " + result + "\n" +
      "lY: " + lY + "\n" +
      "rY: " + rY + "\n"
      );
      */

    return result;
  }

  static bool controllersAreOutstretchedLeftAndRight() {
    Vector3 cPos = getCamera().transform.position;
    Vector3 cRight = getCamera().transform.right;
    Vector3 lPos = getLeftController().transform.position;
    Vector3 rPos = getRightController().transform.position;

    float lTheta = Vector3.Angle(cRight, (lPos - cPos));
    float rTheta = Vector3.Angle(cRight, (rPos - cPos));

    bool result = lTheta > 180 - 10 && rTheta < 10;

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(): " + result + "\n" +
      "lTheta: " + lTheta + "\n" +
      "rTheta: " + rTheta + "\n" +
      "cRight: " + cRight + "\n" +
      "cPos: " + cPos + "\n" +
      "lPos: " + lPos + "\n" +
      "rPos: " + rPos + "\n"
      );
      */

    return result;
  }

  static bool controllersAreOutstretched() {
    return controllersAreOutstretched(armLength);
  }
  static bool controllersAreOutstretched(float armLength) {
    Vector3 cPos = getCamera().transform.position;
    Vector3 lPos = getLeftController().transform.position;
    Vector3 rPos = getRightController().transform.position;

    //float lTheta = Vector3.Angle(Vector3.up, lPos - cPos);
    //float rTheta = Vector3.Angle(Vector3.up, rPos - cPos);

    float lDelta = (lPos - cPos).magnitude;
    float rDelta = (rPos - cPos).magnitude;

    //bool result = lTheta < 100f && rTheta < 100f && lDelta >= armLength && rDelta >= armLength;
    bool result = lDelta >= armLength && rDelta >= armLength;

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(): " + result + "\n" +
      "armLength: " + armLength + "\n" +
      //"lTheta: " + lTheta + "\n" +
      //"rTheta: " + rTheta + "\n" +
      "lDelta: " + lDelta + "\n" +
      "rDelta: " + rDelta + "\n"
      );
      */

    return result;
  }

  static bool controllersAreForward() {
    Vector3 cPos = getCamera().transform.position;
    Vector3 lPos = getLeftController().transform.position;
    Vector3 rPos = getRightController().transform.position;

    Vector3 cFwd = getCamera().transform.forward;

    bool result = true;

    float lTheta = Vector3.Angle(cFwd, (lPos - cPos));
    float rTheta = Vector3.Angle(cFwd, (rPos - cPos));

    if (lTheta > 85 || rTheta > 85) {
      result = false;
    }

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(): " + result + "\n" +
      "cFwd: " + cFwd + "\n" +
      "lTheta: " + lTheta + "\n" +
      "rTheta: " + rTheta + "\n" +
      "cPos: " + cPos + "\n" +
      "lPos: " + lPos + "\n" +
      "rPos: " + rPos + "\n"
      );
      */

    return result;
  }

  static bool controllersAreAngledForward() {
    Vector3 cFwd = getCamera().transform.forward;
    Vector3 lFwd = getLeftController().transform.forward;
    Vector3 rFwd = getRightController().transform.forward;

    bool result = true;

    float lTheta = Vector3.Angle(cFwd, lFwd);
    float rTheta = Vector3.Angle(cFwd, rFwd);

    if (lTheta < 30 || rTheta < 30 || lTheta > 45 || rTheta > 45) {
      result = false;
    }

    /*
    Debug.Log(
      System.Reflection.MethodBase.GetCurrentMethod().Name + "(): " + result + "\n" +
      "lTheta: " + lTheta + "\n" +
      "rTheta: " + rTheta + "\n" +
      "cFwd: " + cFwd + "\n" +
      "lFwd: " + lFwd + "\n" +
      "rFwd: " + rFwd + "\n"
      );
      */

    return result;
  }

  static bool controllersAreOutstretchedForward() {
    return controllersAreOutstretched() & controllersAreForward();
  }

  static SteamVR_Camera getCamera() {
    if (thisCamera == null) {
      thisCamera = FindObjectOfType<SteamVR_Camera>();
    }

    return thisCamera;
  }

  static SteamVR_TrackedController getLeftController() {
    if (leftController == null) {
      foreach (SteamVR_TrackedController trackedController in FindObjectsOfType<SteamVR_TrackedController>()) {
        float angle = Vector3.Angle(getCamera().transform.right, (trackedController.transform.position - getCamera().transform.position));
        if (angle > 135) {
          leftController = trackedController;
        }
      }
    }

    return leftController;
  }

  static SteamVR_TrackedController getRightController() {
    if (rightController == null) {
      foreach (SteamVR_TrackedController trackedController in FindObjectsOfType<SteamVR_TrackedController>()) {
        float angle = Vector3.Angle(getCamera().transform.right, (trackedController.transform.position - getCamera().transform.position));
        if (angle < 45) {
          rightController = trackedController;
        }
      }
    }

    return rightController;
  }
}
