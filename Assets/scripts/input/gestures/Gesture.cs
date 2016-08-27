using UnityEngine;
using System.Collections;

using GesturePoints = System.Collections.Generic.List<GesturePoint>;

public class Gesture {
  const int DEFAULT_INDEX = int.MinValue;
  int gesturePointIndex;
  public GesturePoints gesturePoints { get; set; }

  public Gesture() {
    gesturePoints = new GesturePoints();
    gesturePointIndex = DEFAULT_INDEX;
  }

  public void test(Kinesiology kinesiology) {
    int testIndex = (gesturePointIndex == DEFAULT_INDEX ? 0 : gesturePointIndex + 1);

    if (gesturePoints[testIndex].test(kinesiology)) {
      ++gesturePointIndex;
    }
  }
}
