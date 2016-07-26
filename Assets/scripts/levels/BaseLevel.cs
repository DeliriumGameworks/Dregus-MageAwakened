using UnityEngine;
using System.Collections;

public abstract class BaseLevel : MonoBehaviour {
  public abstract bool winCheck();
  public abstract bool loseCheck();

  // Update is called once per frame
  void Update() {

  }

  protected PlayerCharacter[] findPlayers() {
    GameObject[] playerTagObjects = GameObject.FindGameObjectsWithTag("Player");
    PlayerCharacter[] players = new PlayerCharacter[playerTagObjects.Length];

    int cursor = 0;

    foreach (GameObject playerGo in playerTagObjects) {
      players[cursor] = playerGo.GetComponent<PlayerCharacter>();

      if (players[cursor] != null) {
        ++cursor;
      }
    }

    return players;
  }

  protected BaddyCharacter[] findBaddies() {
    GameObject[] enemyTagObjects = GameObject.FindGameObjectsWithTag("Enemy");
    BaddyCharacter[] baddies = new BaddyCharacter[enemyTagObjects.Length];

    int cursor = 0;

    foreach (GameObject baddyGo in enemyTagObjects) {
      baddies[cursor] = baddyGo.GetComponent<BaddyCharacter>();

      if (baddies[cursor] != null) {
        ++cursor;
      }
    }

    return baddies;
  }
}
