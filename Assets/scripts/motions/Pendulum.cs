using UnityEngine;
using System.Collections;

public class Pendulum : MonoBehaviour {
  public float yOffset = 0.66f;

  public GameObject origin;
  public GameObject weight;
  public Transform[] links;

  private bool frozen;

  private Camera playerCamera;

  // Use this for initialization
  void Start() {
    frozen = true;
    links = GetComponentsInChildren<Transform>();

    playerCamera = FindObjectOfType<Camera>();

    origin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
  }

  // Update is called once per frame
  void Update() {
    if (Input.anyKey) {
      unfreeze();
    }
  }

  void unfreeze() {
    if (!frozen) {
      return;
    }

    frozen = false;

    Quaternion rot = playerCamera.transform.rotation;

    rot *= Quaternion.Euler(0, 90, 0);

    foreach (Transform link in links) {
      if (link.gameObject != origin) {
        link.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
      }
    }

    //weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

    //weight.GetComponent<Rigidbody>().AddForce(rot.eulerAngles * 500f);
  }
}
