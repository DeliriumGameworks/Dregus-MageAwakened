using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/**
 * For lack of a better idea on designing a container for gestures, I'm going with
 * a static class where we define, in code, the gestures.
 */
public class GestureLibrary : MonoBehaviour {
  /* Spells */

  /** Nova **/

  /*** Nova Gesture Completed Event ***/
  public static UnityEvent novaGestureCompletedEvent = new UnityEvent();

  /**
   * A nova gesture is triggered by moving both controllers from a position of waist height or higher
   * to a position near the floor (within .33m of the floor).
   */
  static Gesture mNova = null;

  /**
   * Initializes the nova gesture if not already initialized.
   * 
   * @returns the nova gesture
   */
  public static Gesture nova() {
    if (mNova != null) return mNova;

    mNova = new Gesture();

    GesturePoint beg = new GesturePoint();

    // TODO: Make this based on waist height instead of arbitrarily .75m (which is too low for many taller people)
    GestureRule waistHeightOrHigher = new GestureRule();

    waistHeightOrHigher.gestureRuleHandApplication = GestureRule.GestureRuleHandApplication.both;
    waistHeightOrHigher.minDistanceFromGround = 0.75f;

    beg.gestureRules.Add(waistHeightOrHigher);

    beg.name = "Nova Initial State";
    beg.description = "Both Controllers Waist Height or Higher";

    mNova.gesturePoints.Add(beg);

    GesturePoint fin = new GesturePoint();

    GestureRule closeToFloor = new GestureRule();

    closeToFloor.gestureRuleHandApplication = GestureRule.GestureRuleHandApplication.both;
    closeToFloor.maxDistanceFromGround = 0.33f;

    fin.name = "Nova Final State";
    fin.description = "Both Controllers Close to Floor";

    fin.gestureRules.Add(closeToFloor);

    // Pass the nova completion event up the chain when it occurs.
    fin.onTriggered.AddListener(() => novaGestureCompletedEvent.Invoke());

    mNova.gesturePoints.Add(fin);

    return mNova;
  }
}
