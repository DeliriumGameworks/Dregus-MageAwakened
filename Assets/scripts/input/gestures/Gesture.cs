using UnityEngine;
using System.Collections.Generic;

public class Gesture {
  enum GestureState : uint {
    /* 0 through uint.MaxValue - 2 is for state numbers -- should be enough! :) */
    GESTURE_NOT_STARTED = uint.MaxValue - 1,
    GESTURE_COMPLETE = uint.MaxValue
  }

  List<GestureStep> gesturePoints = new List<GestureStep>();
  GestureState gestureState = GestureState.GESTURE_NOT_STARTED;
  uint gestureCount = 0;

  public void addGesturePoint(GestureStep gesturePoint) {
    gesturePoints.Add(gesturePoint);
  }

  public void clearGesturePoints() {
    gesturePoints.Clear();
  }

  public bool inNextPoint(SteamVR_Camera steamvrCamera, SteamVR_TrackedObject gestureDevice) {
    if (gestureState == GestureState.GESTURE_NOT_STARTED) {
      if (gesturePoints[0].contains(steamvrCamera, gestureDevice)) {
        return true;
      }
    }

    return false;
  }

  public string ToJson() {
    bool start = true;
    string str = "{\"gestures\": [";

    foreach (GestureStep g in gesturePoints) {
      if (!start) {
        str += ", ";
      }

      str += g.ToJson();

      start = false;
    }

    return str + "]}";
  }
}
