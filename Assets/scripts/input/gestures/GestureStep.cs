using UnityEngine;
using System.Collections;

public class GestureStep {
  public enum GestureRelativeTo {
    hmd,
    leftController,
    rightController,
  }

  public GestureRelativeTo relativeTo = GestureRelativeTo.hmd;

  public Vector3 difference;
  public float radius;

  GestureStep() {
  }

  GestureStep(GestureRelativeTo g, Vector3 d, float r) : this() {
    relativeTo = g;
    difference = d;
    radius = r;
  }

  public static GestureStep createGestureStep(GestureRelativeTo gestureRelativeTo, Vector3 distance, float radius) {
    return new GestureStep(gestureRelativeTo, distance, radius);
  }

  public static GestureStep recordGestureStep(SteamVR_TrackedController left, SteamVR_TrackedController right, float radius) {
    return new GestureStep(GestureRelativeTo.rightController, left.transform.position - right.transform.position, radius);
  }

  public static GestureStep recordGestureStep(SteamVR_Camera steamvrCamera, SteamVR_TrackedObject gestureDevice, float radius) {
    return new GestureStep(GestureRelativeTo.hmd, gestureDevice.transform.position - steamvrCamera.transform.position, radius);
  }

  public bool contains(SteamVR_Camera steamvrCamera, SteamVR_TrackedObject gestureDevice) {
    switch (relativeTo) {
      case GestureRelativeTo.hmd:
        return (difference - (gestureDevice.transform.position - steamvrCamera.transform.position)).magnitude < radius;
      case GestureRelativeTo.leftController:
      //return (difference - ())
      default:
        throw new System.Exception("Invalid GestureStep.relativeTo value");
    }

    return false;
  }

  public string ToJson() {
    return "{\"relativeTo\": \"" + getGestureRelativeTo(this.relativeTo) + "\",\"difference\": " + V3ToJson(difference) + ",\"radius\":" + radius + "}";
  }

  public static string V3ToJson(Vector3 v) {
    return
      "{\"x\": " + v.x +
      ",\"y\": " + v.y +
      ",\"z\": " + v.z +
      "}";
  }

  static string[] gestureRelativeToTranslations;

  static void initGestureRelativeToTranslations() {
    if (gestureRelativeToTranslations != null) {
      return;
    }

    gestureRelativeToTranslations = new string[3];

    gestureRelativeToTranslations[(int) GestureRelativeTo.hmd] = "hmd";
    gestureRelativeToTranslations[(int) GestureRelativeTo.leftController] = "leftController";
    gestureRelativeToTranslations[(int) GestureRelativeTo.rightController] = "rightController";
  }

  public static string getGestureRelativeTo(GestureRelativeTo gestureRelativeTo) {
    initGestureRelativeToTranslations();

    return gestureRelativeToTranslations[(int) gestureRelativeTo];
  }

}
