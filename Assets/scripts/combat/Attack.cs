using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
  public enum Element { True, Physical, Acid, Fire, Frost, Lightning }

  public float baseMaximumDamage = 0;
  public float variance = 0;

  public Weapon weapon;

  public string attackName = "";

  public float minimumRange = 0;
  public float maximumRange = 5;

  public string[] validAnimationNames;

  public float getBaseDamage() {
    return Random.Range(baseMaximumDamage * (1 - variance), baseMaximumDamage);
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
  }
}
