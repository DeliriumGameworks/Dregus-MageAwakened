using UnityEngine;
using System.Collections.Generic;

public class GestureController : MonoBehaviour {
  public System.Guid playerId;
  public Kinesiology playerKinesiology;
  public SteamVR_TrackedController[] controllers = new SteamVR_TrackedController[2];
  SteamVR_TrackedController left = null;
  SteamVR_TrackedController right = null;

  // Use this for initialization
  void Start() {
    if (playerId != null) {
      playerKinesiology = Kinesiology.load(playerId);
    } else {
      playerKinesiology = new Kinesiology();
    }

    DregusGUILookup.set("gestureStatus", "Initialized");

    controllers[0] = null;
    controllers[1] = null;
  }

  // Update is called once per frame
  void Update() {
    string status = "";

    if (controllersNotNull()) {
      status = "Detected controllers";
    }

    if (leftAndRightDetected()) {
      status = "Left and Right detected";
    } else {
      detectLeftAndRight();
    }

    DregusGUILookup.set("gestureStatus", status);
  }

  void detectLeftAndRight() {

  }

  bool leftAndRightDetected() {
    return left != null && right != null;
  }

  bool controllersNotNull() {
    int i;

    for (i = 0; i < controllers.Length; ++i) {
      if (controllers[i] == null) {
        return true;
      }
    }

    return false;
  }

  bool haveControllers() {
    if (controllersNotNull()) {
      return true;
    }

    SteamVR_TrackedController[] lControllers = FindObjectsOfType<SteamVR_TrackedController>();

    if (lControllers.Length != controllers.Length) {
      return false;
    }

    for (int i = 0; i < lControllers.Length; ++i) {
      controllers[i] = lControllers[i];
    }

    return controllersNotNull();
  }
}
