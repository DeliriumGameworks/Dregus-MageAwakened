using UnityEngine;
using System.Collections.Generic;

public class BaseLevel : MonoBehaviour {
  GameObject[] players;
  GameObject[] baddies;

  float playersStale = 2f; // seconds until refreshing players
  float nextPlayersRefresh = 0f;

  void Start() {
    nextPlayersRefresh = Time.time + playersStale;

    players = refreshPlayers();
    baddies = refreshBaddies();
  }

  // Update is called once per frame
  void Update() {
    if (Time.time > nextPlayersRefresh) {
      nextPlayersRefresh = Time.time + playersStale;

      players = refreshPlayers();
      baddies = refreshBaddies();
    }
  }

  public GameObject[] findPlayers() {
    return players;
  }

  public GameObject[] refreshPlayers() {
    PlayerCharacter[] baddyCharacters = FindObjectsOfType<PlayerCharacter>();
    GameObject[] gameObjects = new GameObject[baddyCharacters.Length];

    for (int i = 0; i < baddyCharacters.Length; ++i) {
      gameObjects[i] = baddyCharacters[i].gameObject;
    }

    return gameObjects;
  }

  public GameObject[] findBaddies() {
    return baddies;
  }

  public GameObject[] refreshBaddies() {
    BaddyCharacter[] baddyCharacters = FindObjectsOfType<BaddyCharacter>();
    GameObject[] gameObjects = new GameObject[baddyCharacters.Length];

    for (int i = 0; i < baddyCharacters.Length; ++i) {
      gameObjects[i] = baddyCharacters[i].gameObject;
    }

    return gameObjects;
  }
}
