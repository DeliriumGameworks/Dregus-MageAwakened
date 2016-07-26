using UnityEngine;
using System.Collections;

public class PlayerCharacter : Character {
  void Start() {
    start();
  }

  protected override void start() {
    base.start();
  }

  void OnGUI() {
    BaseLevel levelRules = GameObject.FindObjectOfType<BaseLevel>();
    Rect rect = new Rect(0, 0, 500, 200);

    if (levelRules.winCheck()) {
      GUI.Label(rect, "You win!");
    } else if (levelRules.loseCheck()) {
      GUI.Label(rect, "You lose!");
    }
  }

  // Update is called once per frame
  void Update() {
    Camera camera = GetComponentInChildren<Camera>();

    if (Input.GetButtonDown("Fire1")) {
      Weapon w = (Weapon) Instantiate(attacks[0].weapon, transform.position + (transform.forward) * 3, camera.transform.rotation);
      w.owner = this;
      w.addOnHitListener(onHitListener, attacks[0]);
    }

    if (Input.GetButtonDown("Fire3")) {
      foreach (BaddyCharacter baddy in GameObject.FindObjectsOfType<BaddyCharacter>()) {
        baddy.applyDamage(5000, Attack.Element.True);
        onKill(baddy);
      }
    }
  }

  void onHitListener(Attack attack, Collider coll) {
    Debug.Log("onHitListener");
    Character character = findCharacter(coll);
    float damage = attack.getBaseDamage() * attack.weapon.damageMultiplier;

    if (character != null) {
      if (character.applyDamage(damage, attack.weapon.damageType)) {
        onKill(character);
      }
    } else {
      Debug.Log("Would have dealt " + damage + " damage.");
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
