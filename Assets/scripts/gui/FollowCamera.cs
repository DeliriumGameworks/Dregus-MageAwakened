using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
  public float minDistanceFromPlayer = 1f;
  public float maxDistanceFromPlayer = 1.5f;
  public float yOffset = 0.66f;

  //private Quaternion initialRotationVector;
  private Camera playerCamera;

  // Use this for initialization
  void Start() {
    playerCamera = FindObjectOfType<Camera>();

    //initialRotationVector = transform.rotation;
  }

  // Update is called once per frame
  void Update() {
    updatePosition();
    updateRotation();
  }

  void updatePosition() {
    float currentDistance = Vector3.Distance(transform.position, playerCamera.transform.position);

    currentDistance = Mathf.Max(minDistanceFromPlayer, currentDistance);
    currentDistance = Mathf.Min(maxDistanceFromPlayer, currentDistance);

    Quaternion zRotation = playerCamera.transform.rotation;

    Vector3 zRotEuler = zRotation.eulerAngles;

    zRotEuler.x = 0;
    zRotEuler.z = 0;

    zRotation.eulerAngles = zRotEuler;

    Vector3 target = playerCamera.transform.position + (zRotation * Vector3.forward * currentDistance);

    target.y += yOffset;

    //transform.position = Vector3.MoveTowards(transform.position, target, 0.05f);
    transform.position = target;
  }

  void updateRotation() {
    Quaternion newRotation = Quaternion.LookRotation(transform.position - playerCamera.transform.position);

    //newRotation = newRotation * initialRotationVector;

    transform.rotation = newRotation;
  }
}
