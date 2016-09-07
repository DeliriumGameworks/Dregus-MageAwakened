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
  public static GestureEvent novaGestureCompletedEvent = new GestureEvent();

  /**
   * A nova gesture is triggered by moving both controllers from a position of waist height or higher
   * to a position near the floor (within .33m of the floor).
   */
  private static Gesture mNova = null;

  /**
   * Initializes the nova gesture if not already initialized.
   * 
   * @returns the nova gesture
   */
  public static Gesture nova() {
    if (mNova != null) return mNova;

    mNova = new Gesture();

    mNova.name = "Nova";

    GesturePoint beg = new GesturePoint();

    // TODO: Make this based on waist height instead of arbitrarily .75m (which is too low for many taller people)
    GestureRule waistHeightOrHigher = new GestureRule();

    waistHeightOrHigher.gestureRuleHandApplication = GestureRule.GestureRuleHandApplication.both;
    waistHeightOrHigher.minDistanceFromGround = 0.45f;

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
    fin.onTriggered.AddListener(() => novaGestureCompletedEvent.Invoke(mNova));

    mNova.gesturePoints.Add(fin);

    return mNova;
  }

  /** Beam Attack **/

  /*** Beam Attack Completed Event ***/
  public static GestureEvent beamAttackCompletedEvent = new GestureEvent();

  /**
   * A beam attack gesture is triggered by clicking a trigger while your controllers are close together and
   * rapidly moving them towards the target.
   */
  private static Gesture mBeamAttack = null;

  public static Gesture beamAttack() {
    if (mBeamAttack != null) return mBeamAttack;

    mBeamAttack = new Gesture();

    mBeamAttack.name = "Beam Attack";

    GesturePoint beg = new GesturePoint();

    GestureRule eitherTriggerClicked = new GestureRule();

    eitherTriggerClicked.gestureRuleHandApplication = GestureRule.GestureRuleHandApplication.either;
    eitherTriggerClicked.maxControllerDelta = 0.33f;
    eitherTriggerClicked.triggerMustBePressed = true;

    beg.gestureRules.Add(eitherTriggerClicked);

    beg.name = "Beam Attack Initial State";
    beg.description = "Either trigger clicked, controllers close together";

    mBeamAttack.gesturePoints.Add(beg);

    GesturePoint fin = new GesturePoint();

    GestureRule controllersMoved = new GestureRule();

    controllersMoved.gestureRuleHandApplication = GestureRule.GestureRuleHandApplication.both;
    controllersMoved.maxControllerDelta = 0.33f;
    controllersMoved.minAnchorDelta = 1f;

    fin.gestureRules.Add(controllersMoved);

    fin.name = "Beam Attack Final State";
    fin.description = "Move hands at least 1 meter, but keep them together";

    fin.onTriggered.AddListener(() => beamAttackCompletedEvent.Invoke(mBeamAttack));

    mBeamAttack.gesturePoints.Add(fin);

    return mBeamAttack;
  }
}
