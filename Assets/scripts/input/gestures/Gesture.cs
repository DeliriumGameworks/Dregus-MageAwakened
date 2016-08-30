using UnityEngine;
using System.Collections;

using GesturePoints = System.Collections.Generic.List<GesturePoint>;

public class Gesture {
  const int DEFAULT_INDEX = 0;

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

      if (gesturePointIndex == gesturePoints.Count) {
        reset();
      }
    }
  }

  public void reset() {
    gesturePointIndex = DEFAULT_INDEX;
  }
}
