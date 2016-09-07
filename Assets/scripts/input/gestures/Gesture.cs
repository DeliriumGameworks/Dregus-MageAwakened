using UnityEngine;
using System.Collections;

using GesturePoints = System.Collections.Generic.List<GesturePoint>;

public class Gesture {
  const int DEFAULT_INDEX = 0;

  public string name { get; set; }

  /**
   * The current unpassed index.
   */
  int gesturePointIndex;

  public GesturePoints gesturePoints { get; set; }

  public Gesture() {
    gesturePoints = new GesturePoints();
    gesturePointIndex = DEFAULT_INDEX;
  }

  public void test(Kinesiology kinesiology) {
    if (gesturePoints.Count <= gesturePointIndex || gesturePointIndex < 0) {
      return;
    }

    if (gesturePoints[gesturePointIndex].test(kinesiology)) {
      Debug.Log("Passed " + gesturePoints[gesturePointIndex].name + ": " + gesturePoints[gesturePointIndex].description);

      ++gesturePointIndex;

      // TODO Record Vector3.positions for controllers et al so we can start doing tests based on that.

      if (gesturePointIndex == gesturePoints.Count) {
        reset();
      } else {
        Vector3 lAnchorPoint = kinesiology.left.transform.position;
        Vector3 rAnchorPoint = kinesiology.right.transform.position;

        foreach (GestureRule gestureRule in gesturePoints[gesturePointIndex].gestureRules) {
          gestureRule.leftAnchorPoint = lAnchorPoint;
          gestureRule.rightAnchorPoint = rAnchorPoint;
        }
      }
    }
  }

  public void reset() {
    Debug.Log("Resetting Gesture " + name);

    gesturePointIndex = DEFAULT_INDEX;
  }
}
