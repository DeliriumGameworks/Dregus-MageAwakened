using UnityEngine;
using System.Collections;
using System;

public class CollideEffect : MonoBehaviour {
  public float birth { get; set; }

  public float lifespan { get; set; }

  /* TODO: CollideEffects aren't always expiring when expected */
  public void OnExpires() {
    foreach (Transform t in this.transform) {
      Destroy(t.gameObject);
    }

    Destroy(this);
  }

  bool expired() {
    return Time.time > birth + lifespan;
  }

  public float InspectorLifespan = 3.0f;

  // Use this for initialization
  void Start() {
    birth = Time.time;
    lifespan = InspectorLifespan;
  }

  // Update is called once per frame
  void Update() {
    if (expired()) {
      OnExpires();
    }
  }
}
