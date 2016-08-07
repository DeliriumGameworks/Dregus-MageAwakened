using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class AttackEvent : UnityEvent<Character, Character, Attack, float> {

}

public class Attack : MonoBehaviour {
  public AttackEvent attackEvent = new AttackEvent();
  public enum Element { True, Physical, Acid, Fire, Frost, Lightning }

  public float baseMaximumDamage = 0;
  public float variance = 0;

  public Weapon weapon;
  public BaseProjectile projectile;

  public string attackName = "";

  public float minimumRange = 0;
  public float maximumRange = 5;

  public string[] validAnimationNames;

  public SoundEffect soundEffect;

  public void attack(Character attacker, Character target, Element damageType) {
    float damage = getBaseDamage() * attacker.damageMultiplier;

    if (target.applyDamage(damage, damageType, attacker.gameObject)) {
      attacker.onKill(target);
    }

    attackEvent.Invoke(attacker, target, this, damage);
  }

  private float getBaseDamage() {
    return Random.Range(baseMaximumDamage * (1 - variance), baseMaximumDamage);
  }

  public void triggerSwingSound() {
    if (soundEffect != null && soundEffect.isValid()) {
      soundEffect.playOnceDefault();
    }
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }
}
