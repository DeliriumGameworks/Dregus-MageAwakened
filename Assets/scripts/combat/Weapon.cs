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

    getHitEvent().AddListener(handler);
  }

  public void clearListeners() {
    getHitEvent().RemoveAllListeners();
  }

  public HitEvent getHitEvent() {
    if (hitEvent == null) {
      hitEvent = new HitEvent();
    }

    return hitEvent;
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
    getHitEvent().Invoke(sourceAttack, coll);
  }
}
