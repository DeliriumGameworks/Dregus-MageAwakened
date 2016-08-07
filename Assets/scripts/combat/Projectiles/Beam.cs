using UnityEngine;
using System.Collections;

public class Beam : BaseProjectile {
  public GameObject beamAttack;
  public bool beamActive = false;

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
  }

  public override void handleCollision(GameObject obj, Collision c) {
    if (collided) {
      // already collided, don't do anything
      return;
    }

    // stop the projectile
    collided = true;
    stop();
  }
}
