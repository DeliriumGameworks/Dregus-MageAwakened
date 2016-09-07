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
 * 
 * TODO: I think that changing many of these test functions into inline functions would make the code
 *       more efficient.  Since FPS is so difficult to keep high, I think that this might be some low
 *       hanging fruit when we get to the optimization stage.
 */
public class GestureRule {
  public class GestureRuleFactory {

  }

  public const float UNDEFINED_FLOAT_VALUE = float.MinValue;
  public Vector3 UNDEFINED_VECTOR3_VALUE = Vector3.zero;

  public enum GestureRuleHandApplication {
    undefined,
    // TODO: Should dominant and nonDominant be removed?  I'm not sure how to determine which hand is dominant.
    //       and I'm not even sure what the benefit of this would be.  'either' probably accomplishes what we need.
    dominant,
    nonDominant,
    either,
    both
  }

  public GestureRuleHandApplication gestureRuleHandApplication { get; set; }

  /**
   * The minimum distance from the ground as a coefficient of the height of the player.
   * A 2m tall person will have an eye height of about 190cm, if minDistanceFromGround is 0.5f
   * they have to be at least 95cm off the ground.
   */
  public float minDistanceFromGround { get; set; }
  public float maxDistanceFromGround { get; set; }
  public float maxControllerDelta { get; set; }

  public bool triggerMustBePressed { get; set; }

  public float minAnchorDelta { get; set; }
  /** 
   * The left anchor point is the Vector3 coordinates of the left controller at the last gesture point.
   */
  public Vector3 leftAnchorPoint { get; set; }

  /**
   * The right anchor point is the Vector3 coordinates of the right controller at the last gesture point.
   */
  public Vector3 rightAnchorPoint { get; set; }

  public GestureRule() {
    gestureRuleHandApplication = GestureRuleHandApplication.undefined;
    minDistanceFromGround = UNDEFINED_FLOAT_VALUE;
    maxDistanceFromGround = UNDEFINED_FLOAT_VALUE;
    maxControllerDelta = UNDEFINED_FLOAT_VALUE;

    triggerMustBePressed = false;

    minAnchorDelta = UNDEFINED_FLOAT_VALUE;
    leftAnchorPoint = UNDEFINED_VECTOR3_VALUE;
    rightAnchorPoint = UNDEFINED_VECTOR3_VALUE;
  }

  public bool test(Kinesiology kinesiology) {
    switch (gestureRuleHandApplication) {
      case GestureRuleHandApplication.both:
        if (!testDistanceFromGround(kinesiology, kinesiology.left) || !testDistanceFromGround(kinesiology, kinesiology.right)) {
          return false;
        }
        if (!testTriggerDepressed(kinesiology.left) || !testTriggerDepressed(kinesiology.right)) {
          return false;
        }
        if (!testControllerDelta(kinesiology.left, kinesiology.right)) {
          return false;
        }
        if (!testAnchorDelta(kinesiology.left, leftAnchorPoint) || !testAnchorDelta(kinesiology.right, rightAnchorPoint)) {
          return false;
        }

        break;
      case GestureRuleHandApplication.either:
        if (!testDistanceFromGround(kinesiology, kinesiology.left) && !testDistanceFromGround(kinesiology, kinesiology.right)) {
          return false;
        }
        if (!testTriggerDepressed(kinesiology.left) && !testTriggerDepressed(kinesiology.right)) {
          return false;
        }
        if (!testControllerDelta(kinesiology.left, kinesiology.right)) {
          return false;
        }
        if (!testAnchorDelta(kinesiology.left, leftAnchorPoint) && !testAnchorDelta(kinesiology.right, rightAnchorPoint)) {
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
  public bool testDistanceFromGround(Kinesiology kinesiology, Controller controller) {
    float distanceToGround = KinesiologyCalibration.getInstantaneousRaycastToGround(controller.transform);
    float normalizedMinDistanceFromGround = kinesiology.eyesHeight * minDistanceFromGround;
    float normalizedMaxDistanceFromGround = kinesiology.eyesHeight * maxDistanceFromGround;

    if (minDistanceFromGround != UNDEFINED_FLOAT_VALUE &&
      distanceToGround < normalizedMinDistanceFromGround) {
      return false;
    }

    if (maxDistanceFromGround != UNDEFINED_FLOAT_VALUE &&
      distanceToGround > normalizedMaxDistanceFromGround) {
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

  // TODO: Add a min controller delta also, complicates the if () logic as well.
  public bool testControllerDelta(Controller c1, Controller c2) {
    if (maxControllerDelta == UNDEFINED_FLOAT_VALUE) return true;

    return Vector3.Distance(c1.transform.position, c2.transform.position) < maxControllerDelta;
  }

  // TODO: Add a max anchor delta if needed (?), complicates the if logic
  public bool testAnchorDelta(Controller c, Vector3 anchorPoint) {
    if (anchorPoint == UNDEFINED_VECTOR3_VALUE || minAnchorDelta == UNDEFINED_FLOAT_VALUE) {
      return true;
    }

    return Vector3.Distance(c.transform.position, anchorPoint) > minAnchorDelta;
  }
}
