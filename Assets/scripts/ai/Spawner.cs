using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class BaddySpawnEvent : UnityEvent<GameObject> {

}

public class Spawner : MonoBehaviour {
  public BaddySpawnEvent baddySpawnEvent;
  public SpawnerPool spawnerPool;

  public System.Collections.Generic.List<BaddyCharacter> baddies { get; set; }
  public System.Collections.Generic.List<BaddyCharacter> liveBaddies { get; set; }

  public float collisionRange = 1f;

  private bool isSpawning;

  public bool spawning {
    get {
      return isSpawning;
    }
    set {
      if (value) {
        nextSpawnTime = Time.time + calculateSpawnInterval();
      }

      isSpawning = value;
    }
  }

  float nextSpawnTime = 0;
  public float spawnInterval { get; set; }
  public float spawnIntervalVariance { get; set; }
  public float healthMultiplier { get; set; }
  public float armorMultiplier { get; set; }
  public float damageMultiplier { get; set; }
  public float movementSpeedMultiplier { get; set; }

  void Awake() {
    baddies = new System.Collections.Generic.List<BaddyCharacter>();
    liveBaddies = new System.Collections.Generic.List<BaddyCharacter>();
    baddySpawnEvent = new BaddySpawnEvent();
  }

  // Use this for initialization
  void Start() {
    if (spawnerPool == null) {
      spawnerPool = GetComponentInParent<SpawnerPool>();
    }
  }

  // Update is called once per frame
  void Update() {
    if (spawning && Time.time >= nextSpawnTime && baddies.Count > 0 && noBaddiesInRange()) {
      nextSpawnTime += calculateSpawnInterval();
      spawn();
    }

    cleanLiveBaddies();
  }

  void cleanLiveBaddies() {
    for (int i = 0; i < liveBaddies.Count; ++i) {
      if (!liveBaddies[i].GetComponent<Character>().isAlive()) {
        liveBaddies.RemoveAt(i);
        --i;
      }
    }
  }

  bool noBaddiesInRange() {
    foreach (BaddyCharacter baddyCharacter in baddies) {
      if ((this.transform.position - baddyCharacter.transform.position).magnitude < collisionRange) {
        return false;
      }
    }

    return true;
  }

  float calculateSpawnInterval() {
    return Random.Range(spawnInterval * (1 - spawnIntervalVariance), spawnInterval * (1 + spawnIntervalVariance));
  }

  void spawn() {
    GameObject baddyGo = getNextBaddy();

    if (baddyGo == null) {
      return;
    }

    if (spawnerPool == null) {
      baddyGo = (GameObject) Instantiate(baddyGo, transform.position, transform.rotation);
    } else {
      baddyGo = spawnerPool.borrowPrefab(baddyGo);

      baddyGo.SetActive(true);

      baddyGo.transform.position = transform.position;
      baddyGo.transform.rotation = transform.rotation;
    }

    liveBaddies.Add(baddyGo.GetComponent<BaddyCharacter>());
    baddies.RemoveAt(0);

    Character character = baddyGo.GetComponent<Character>();
    AICharacter aiCharacter = baddyGo.GetComponent<AICharacter>();

    character.initialHealth = character.initialHealth * healthMultiplier;
    character.health = character.initialHealth;
    character.damageMultiplier = damageMultiplier;

    aiCharacter.baddyNavStrategy = AICharacter.BaddyNavStrategy.chaseClosestNoLineOfSight;
    aiCharacter.panicMoveSpeed *= movementSpeedMultiplier;

    baddySpawnEvent.Invoke(baddyGo);

    StartCoroutine(enableNavMeshAgent(baddyGo));
  }

  GameObject getNextBaddy() {
    for (int i = 0; i < baddies.Count; ++i) {
      if (!baddies[i].isActiveAndEnabled) {
        return baddies[i].gameObject;
      }
    }

    return null;
  }

  IEnumerator enableNavMeshAgent(GameObject baddyGo) {
    yield return new WaitForSeconds(0.2f);

    baddyGo.GetComponent<NavMeshAgent>().enabled = true;

    yield return new WaitForSeconds(0.1f);

    baddyGo.GetComponent<NavMeshAgent>().Resume();
  }

  public int getBaddyCount() {
    return baddies.Count + liveBaddies.Count;
  }
}
