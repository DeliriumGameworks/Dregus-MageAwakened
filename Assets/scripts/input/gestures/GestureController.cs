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
    Instantiate(fireNovaPrefab, this.transform);
  }
}
