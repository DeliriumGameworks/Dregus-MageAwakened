using UnityEngine;
using System.Collections;

using Controller = VRTK.VRTK_ControllerEvents;

/**
 * A GestureRule is one of the criteria that must be met to satisfy a GesturePoint.
 * 
 * Examples of GestureRules are "Put the controller near the floor."  To accomplish
 * this, you would want to specify a maxDistanceFromGround and a gestureRuleHandApplication
 * specifying how close they should get to the floor and which controllers.
 * 
 * TODO: This is a stub, need to finish implementing it, tons of features need to be added.
 */
public class GestureRule {
  public const float UNDEFINED_FLOAT_VALUE = float.MinValue;

  public enum GestureRuleHandApplication {
    undefined,
    dominant,
    nonDominant,
    either,
    both
  }

  public GestureRuleHandApplication gestureRuleHandApplication { get; set; }
  public float minDistanceFromGround { get; set; }
  public float maxDistanceFromGround { get; set; }

  public bool triggerMustBePressed { get; set; }

  public GestureRule() {
    gestureRuleHandApplication = GestureRuleHandApplication.undefined;
    minDistanceFromGround = UNDEFINED_FLOAT_VALUE;
    maxDistanceFromGround = UNDEFINED_FLOAT_VALUE;
    triggerMustBePressed = false;
  }

  public bool test(Kinesiology kinesiology) {
    switch (gestureRuleHandApplication) {
      case GestureRuleHandApplication.both:
        if (!testDistanceFromGround(kinesiology.left) || !testDistanceFromGround(kinesiology.right)) {
          return false;
        }
        if (!testTriggerDepressed(kinesiology.left) || !testTriggerDepressed(kinesiology.right)) {
          return false;
        }
        break;
      default:
        return false;
    }

    return true;
  }

  /**
   * Tests if this GestureRule requires min/max distance from ground and whether or not it passes.
   * If it's not required, it passes.
   */
  public bool testDistanceFromGround(Controller controller) {
    if (minDistanceFromGround != UNDEFINED_FLOAT_VALUE &&
      KinesiologyCalibration.getInstantaneousRaycastToGround(controller.transform) < minDistanceFromGround) {
      return false;
    }

    if (maxDistanceFromGround != UNDEFINED_FLOAT_VALUE &&
      KinesiologyCalibration.getInstantaneousRaycastToGround(controller.transform) > maxDistanceFromGround) {
      return false;
    }

    return true;
  }

  /**
   * Tests if the trigger(s) must be depressed, passes if not required or if triggers are depressed.
   */
  public bool testTriggerDepressed(Controller controller) {
    if (triggerMustBePressed) {
      return controller.triggerPressed;
    }

    return true;
  }
}
