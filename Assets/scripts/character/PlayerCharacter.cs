using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {
  Character character;

  void Start() {
    character = GetComponent<Character>();
    character.levelUpEvent.AddListener(onLevelUp);
  }

  void onLevelUp() {
    character.damageMultiplier *= 1.25f;
    character.maxHealth *= 1.25f;
    character.health = character.maxHealth;
  }

  // Update is called once per frame
  void Update() {
    Camera camera = GetComponentInChildren<Camera>();
    /*

    if (Input.GetButtonDown("Fire1")) {
      Weapon w = (Weapon) Instantiate(attacks[0].weapon, transform.position + (transform.forward) * 3, camera.transform.rotation);
      w.owner = this;
      w.addOnHitListener(onHitListener, attacks[0]);
    }

    if (Input.GetButtonDown("Fire3")) {
      foreach (BaddyCharacter baddy in GameObject.FindObjectsOfType<BaddyCharacter>()) {
        baddy.applyDamage(5000, Attack.Element.True, gameObject);
        onKill(baddy);
      }
    }
    */
  }

  void onHitListener(Attack attack, Collider coll) {
    Character colliderCharacter = Character.findCharacter(coll);

    if (colliderCharacter != null) {
      Attack.Element damageType = Attack.Element.Physical;

      if (attack.weapon != null) {
        damageType = attack.weapon.damageType;
      } else if (attack.projectile != null) {
        damageType = attack.weapon.damageType;
      }

      attack.attack(character, colliderCharacter, damageType);
    }
  }

  Vector3 getBottom() {
    CharacterController coll = GetComponent<CharacterController>();

    if (coll == null) {
      Debug.LogError("No capsule collider found");
    }

    return coll.transform.position - (Vector3.down * coll.height / 2);
  }
}
