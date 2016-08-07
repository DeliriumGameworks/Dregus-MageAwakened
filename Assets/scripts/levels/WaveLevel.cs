using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveLevel : MonoBehaviour {
  public BaseLevel baseLevel;
  public SoundEffect startWaveSound;
  public SoundEffect endWaveSound;

  public bool winCheck() {
    return false;
  }

  public bool loseCheck() {
    foreach (GameObject go in baseLevel.findPlayers()) {
      if (go.GetComponent<Character>().isAlive()) {
        return false;
      }
    }

    return true;
  }

  public List<Spawner> spawners { get; set; }
  public List<BaddyCharacter> baddies { get; set; }

  public BaddyCharacter[] baddiesRay;

  public float waveWaitDuration = 15f;

  int waveNum = 0;
  bool startingNextWave = false;

  int secondsUntilStart = 0;

  void Awake() {
    spawners = new List<Spawner>();
    baddies = new List<BaddyCharacter>();
  }

  // Use this for initialization
  void Start() {
    baseLevel = GetComponent<BaseLevel>();

    foreach (BaddyCharacter baddyCharacter in baddiesRay) {
      baddies.Add(baddyCharacter);
    }

    Spawner[] foundSpawners = FindObjectsOfType<Spawner>();

    foreach (Spawner spawner in foundSpawners) {
      spawners.Add(spawner);
    }
  }

  void Update() {
    if (loseCheck()) {
      DregusGUILookup.set("status", "You lost");
    } else if (startingNextWave && waveFinished()) {
      DregusGUILookup.set("status", "Wave " + waveNum + " starts in " + secondsUntilStart);
    } else {
      DregusGUILookup.set("status", "Wave in Progress");
    }

    DregusGUILookup.set("waveNum", "" + waveNum);

    Character player = baseLevel.findPlayers()[0].GetComponent<Character>();

    DregusGUILookup.set("playerHealth", "" + Mathf.RoundToInt(player.health));
    DregusGUILookup.set("xpEarned", "" + Mathf.RoundToInt(player.xpEarned));
    DregusGUILookup.set("playerLevel", "" + player.getLevel());

    int remainingBaddies = 0;

    foreach (Spawner spawner in spawners) {
      remainingBaddies += spawner.getBaddyCount();
    }

    DregusGUILookup.set("remainingBaddies", "" + remainingBaddies);

    if (loseCheck()) {
      return;
    }

    if (!startingNextWave && waveFinished()) {
      if (waveNum > 0) {
        playEndWaveSound();
      }

      ++waveNum;

      startingNextWave = true;
      StartCoroutine(delayedStartNextWave());
    }
  }

  void disableSpawners() {
    foreach (Spawner spawner in spawners) {
      spawner.spawning = false;
    }
  }

  IEnumerator delayedStartNextWave() {
    float startTime = Time.time + waveWaitDuration;

    do {
      yield return new WaitForSeconds(0.1f);

      secondsUntilStart = Mathf.RoundToInt(startTime - Time.time);
    } while (Time.time < startTime);

    startNextWave();

    startingNextWave = false;

    playStartWaveSound();
  }

  void playStartWaveSound() {
    if (startWaveSound != null && startWaveSound.isValid()) {
      startWaveSound.playOnceRandomAudioClip();
    }
  }

  void playEndWaveSound() {
    if (endWaveSound != null && endWaveSound.isValid()) {
      endWaveSound.playOnceRandomAudioClip();
    }
  }

  void startNextWave() {
    int enemyCount = getEnemyCount(waveNum);

    foreach (Spawner spawner in spawners) {
      spawner.healthMultiplier = getHealthMultiplier(waveNum);
      spawner.armorMultiplier = getArmorMultiplier(waveNum);
      spawner.damageMultiplier = getDamageMultiplier(waveNum);
      spawner.spawnInterval = getSpawnInterval(waveNum);
      spawner.spawnIntervalVariance = getSpawnIntervalVariance(waveNum);
      spawner.movementSpeedMultiplier = getMovementSpeedMultiplier(waveNum);

      for (int i = 0; i < enemyCount; ++i) {
        spawner.baddies.Add(baddies[Random.Range(0, baddies.Count - 1)]);
      }

      spawner.spawning = true;
    }
  }

  static float getHealthMultiplier(int waveNum) {
    return 1;
  }

  static float getArmorMultiplier(int waveNum) {
    return 1;
  }

  static float getDamageMultiplier(int waveNum) {
    return 1;
  }

  static float getSpawnInterval(int waveNum) {
    return 4;
  }

  static float getSpawnIntervalVariance(int waveNum) {
    return 0.8f;
  }

  static float getMovementSpeedMultiplier(int waveNum) {
    return 1f;
  }

  // enemyCount = 4 * ln (x + 1) + 2
  static int getEnemyCount(int waveNum) {
    int enemyCount = Mathf.RoundToInt(Mathf.Log(waveNum + 1, 2) + 1);

    Debug.Log("Enemy Count: " + enemyCount);

    return enemyCount;
  }

  bool waveFinished() {
    foreach (Spawner spawner in spawners) {
      if (spawner.getBaddyCount() > 0) {
        return false;
      }
    }

    return true;
  }
}
