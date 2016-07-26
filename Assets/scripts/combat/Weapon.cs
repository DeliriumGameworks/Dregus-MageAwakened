using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class HitEvent : UnityEvent<Attack, Collider> {

}

public class Weapon : MonoBehaviour {
  public Character owner;

  private HitEvent hitEvent;

  public float damageMultiplier = 1;
  public Attack.Element damageType;

  protected Attack sourceAttack;

  public void addOnHitListener(UnityAction<Attack, Collider> handler, Attack attack) {
    sourceAttack = attack;

    if (hitEvent == null) {
      hitEvent = new HitEvent();
    }

    hitEvent.AddListener(handler);
  }

  public void clearListeners() {
    hitEvent.RemoveAllListeners();
  }

  // Use this for initialization
  void Start() {
    start();
  }

  protected virtual void start() {
  }

  // Update is called once per frame
  void Update() {

  }

  protected void OnTriggerEnter(Collider coll) {
    if (hitEvent == null) {
      return;
    }

    hitEvent.Invoke(sourceAttack, coll);
  }
}
