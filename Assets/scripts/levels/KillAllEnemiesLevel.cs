using UnityEngine;
using System.Collections;

public class KillAllEnemiesLevel : MonoBehaviour {
  public BaseLevel baseLevel;

  public bool winCheck() {
    foreach (GameObject go in baseLevel.findBaddies()) {
      if (go.GetComponent<Character>().isAlive()) {
        return false;
      }
    }

    return true;
  }

  public bool loseCheck() {
    foreach (GameObject go in baseLevel.findPlayers()) {
      if (go.GetComponent<Character>().isAlive()) {
        return false;
      }
    }

    return true;
  }

  // Use this for initialization
  void Start() {
    baseLevel = GetComponent<BaseLevel>();
  }
}
