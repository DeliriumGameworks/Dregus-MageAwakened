using UnityEngine;
using System.Collections.Generic;

public class GestureController : MonoBehaviour {
  public GameObject fireNovaPrefab;
  public Kinesiology kinesiology { get; set; }

  void Awake() {
    kinesiology = null;
    GestureLibrary.novaGestureCompletedEvent.AddListener(fireNova);
  }

  void Update() {
    if (kinesiology != null) {
      GestureLibrary.nova().test(kinesiology);
    }
  }

  void fireNova() {
    GameObject nova = (GameObject) Instantiate(fireNovaPrefab, transform, false);

    nova.transform.Rotate(Vector3.left, 90);
    nova.transform.Translate(Vector3.up * kinesiology.eyesHeight / 3);
  }
}
