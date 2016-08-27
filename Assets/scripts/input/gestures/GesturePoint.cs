using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using GestureRules = System.Collections.Generic.List<GestureRule>;

/**
 * A GesturePoint is a single step in a series of GesturePoints which
 * together form a single gesture.  It's like an step in the recipe
 * for completing a gesture.
 * 
 * As an example, take the YMCA hand symbols, you could break that into
 * four gestures, such that forming each letter with your hands comprised
 * one GesturePoint.
 */
public class GesturePoint {
  public GestureRules gestureRules { get; set; }
  public UnityEvent onTriggered = new UnityEvent();

  public GesturePoint() {
    gestureRules = new GestureRules();
  }

  public bool test(Kinesiology kinesiology) {
    foreach (GestureRule gestureRule in gestureRules) {
      if (!gestureRule.test(kinesiology)) {
        return false;
      }
    }

    onTriggered.Invoke();

    return true;
  }
}
