using UnityEngine;
using System.Collections;

public class ParticleColliderDamageMultiplier : MonoBehaviour {
  Attack attack;
  public float damageMultiplier = 1;
  public DregusParticleCollider dregusParticleCollider;

  // Use this for initialization
  void Start() {
    attack = GetComponentInParent<Attack>();
    dregusParticleCollider = GetComponentInParent<DregusParticleCollider>();
  }

  // Update is called once per frame
  void Update() {

  }

  void OnParticleCollision(GameObject other) {
    Debug.Log(other.name);
    Character c = other.gameObject.GetComponent<Character>();

    if (c != null) {
      attack.landAttack(null, c, dregusParticleCollider.damageType, damageMultiplier);
    }
  }
}
