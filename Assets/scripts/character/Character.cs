using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LevelUpEvent : UnityEvent {

}

public class DeathEvent : UnityEvent {

}

public class Character : MonoBehaviour {
  public LevelUpEvent levelUpEvent = new LevelUpEvent();
  public DeathEvent deathEvent = new DeathEvent();

  public GameObject levelUpPrefab;
  public float levelUpPrefabDuration;
  public AudioSource playerAudioSource;
  public AudioClip levelUpAudioClip;

  private const int MAX_LEVEL = 126;

  public float xpEarned { get; set; }
  public float xpWorth { get; set; }
  public float initialXpEarned = 0;
  public float initialXpWorth = 50;

  public float initialHealth = 1;
  public float maxHealth { get; set; }
  public float health { get; set; }

  public float initialDamageMultiplier = 1;
  public float damageMultiplier { get; set; }

  public bool tagFriendlyFire = false;

  public Attack[] attacks;

  // x is (current level - 1)
  // xpToLevel = 5 * ((x + 20)^2)
  public int getLevel() {
    int level;
    float xp = 0;

    for (level = 0; xpEarned > xp && level < MAX_LEVEL; ++level) {
      xp += 15f * (level + 20) * (level + 20);

      if (level > MAX_LEVEL) {
        return MAX_LEVEL;
      }
    }
    /*
    for (int i = 0; i < levelXpThreshold.Length; ++i) {
      if (xpEarned < levelXpThreshold[i]) {
        return i + 1;
      }
    }
    */

    return level;
  }

  void OnEnable() {
    attacks = GetComponents<Attack>();

    health = initialHealth;
    maxHealth = initialHealth;
    xpEarned = initialXpEarned;
    xpWorth = initialXpWorth;
    damageMultiplier = initialDamageMultiplier;
  }

  // Update is called once per frame
  void Update() {

  }

  public static Character findCharacter(Collider collider) {
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

  public bool applyDamage(float damage, Attack.Element damageType, GameObject source) {
    if (health <= 0) {
      return false;
    }

    if (!source.name.Contains("Controller")) {
      Debug.Log("Applying damage: " + damage + " of type " + damageType.ToString() + " from " + source.name + " to " + name);
    }

    if (!tagFriendlyFire) {
      if (source.CompareTag(tag)) {
        return false;
      }
    }

    health -= damage;

    deathEvent.Invoke();

    if (health < 0) {
      health = 0;
    }

    return health <= 0;
  }

  public void onKill(Character enemy) {
    Debug.Log(name + " earned " + enemy.xpWorth + " xp");

    int lvl = getLevel();

    xpEarned += enemy.xpWorth;

    if (getLevel() > lvl) {
      StartCoroutine(levelUp());
    }
  }

  IEnumerator levelUp() {
    xpWorth *= 1.5f;

    if (levelUpPrefab != null) {
      float startTime = Time.time;

      GameObject prefab = (GameObject) Instantiate(levelUpPrefab);

      if (playerAudioSource != null && levelUpAudioClip != null) {
        playerAudioSource.clip = levelUpAudioClip;
        playerAudioSource.loop = false;

        playerAudioSource.Play();
      }

      while (levelUpPrefabDuration + startTime > Time.time) {
        prefab.transform.position = transform.position;

        yield return null;
      }

      Debug.Log("Finished");
      Destroy(prefab);
    }
  }

  void Destroy() {
    gameObject.SetActive(false);
  }

  void OnDisable() {
    CancelInvoke();

    deathEvent.RemoveAllListeners();
  }
}
