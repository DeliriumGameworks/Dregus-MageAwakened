using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
  public Transform pivotTransform;
  public float closeAngle = 0f;
  public float openAngle = 90f;
  public float stepAngle = 1f;

  float currentAngle = 0f;
  float testRange = 5f;

  // Use this for initialization
  void Start() {

  }

  void FixedUpdate() {
    if (shouldBeOpen()) {
      open();
    } else {
      close();
    }
  }

  bool shouldBeOpen() {
    PlayerCharacter pc = GameObject.FindObjectOfType<PlayerCharacter>();
    if (pc != null) {
      if ((this.transform.position - pc.transform.position).magnitude < testRange) {
        return true;
      }
    }

    return false;
  }

  void open() {
    rotateDoorOnHinge(openAngle - currentAngle);
  }

  void close() {
    rotateDoorOnHinge(closeAngle - currentAngle);
  }

  void rotateDoorOnHinge(float angle) {
    float rotateAngle = Mathf.Abs(angle) / angle * Mathf.Min(Mathf.Abs(angle), Mathf.Abs(stepAngle));

    if (angle != 0) {
      this.transform.RotateAround(pivotTransform.position, Vector3.up, rotateAngle);

      currentAngle += rotateAngle;
    }
  }
}
