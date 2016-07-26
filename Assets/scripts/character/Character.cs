using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
  public GameObject levelUpPrefab;
  public float levelUpPrefabDuration;

  public double xpEarned { get; protected set; }
  public double xpWorth { get; protected set; }
  public double initialXpEarned = 0;
  public double initialXpWorth = 50;

  public float initialHealth = 1;
  public float health { get; protected set; }

  protected Attack[] attacks;

  double[] levelXpThreshold = {
    1000, 3000, 4000, 7000, 11000, 18000, 29000, 47000, 76000, 123000,
    double.MaxValue
  };

  int getLevel() {
    for (int i = 0; i < levelXpThreshold.Length; ++i) {
      if (xpEarned < levelXpThreshold[i]) {
        return i + 1;
      }
    }

    return 0;
  }

  protected virtual void start() {
    attacks = GetComponents<Attack>();

    health = initialHealth;
    xpEarned = initialXpEarned;
    xpWorth = initialXpWorth;
  }

  // Use this for initialization
  void Start() {
    start();
  }

  // Update is called once per frame
  void Update() {

  }

  protected Character findCharacter(Collider collider) {
    foreach (Component component in collider.gameObject.GetComponents<Component>()) {
      if (component is Character) {
        return (Character) component;
      }
    }

    return null;
  }

  public bool isAlive() {
    return health > 0;
  }

  protected float distanceFrom(GameObject go) {
    return distanceFrom(go.transform.position);
  }

  protected float distanceFrom(Vector3 position) {
    return (position - this.transform.position).magnitude;
  }

  public bool applyDamage(float damage, Attack.Element damageType) {
    Debug.Log("Applying damage: " + damage + " of type " + damageType.ToString());

    health -= damage;

    return health <= 0;
  }

  public void onKill(Character enemy) {
    Debug.Log("Earned " + enemy.xpWorth + " xp");

    int lvl = getLevel();

    xpEarned += enemy.xpWorth;

    if (getLevel() > lvl) {
      StartCoroutine(levelUp());
    }
  }

  IEnumerator levelUp() {
    if (levelUpPrefab != null) {
      float startTime = Time.time;

      GameObject prefab = (GameObject) Instantiate(levelUpPrefab);

      while (levelUpPrefabDuration + startTime > Time.time) {
        prefab.transform.position = transform.position;

        yield return null;
      }

      Debug.Log("Finished");
      Destroy(prefab);
    }
  }
}
