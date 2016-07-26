using UnityEngine;
using System.Collections;

public class KillAllEnemiesLevel : BaseLevel {
  public override bool winCheck() {
    foreach (BaddyCharacter character in findBaddies()) {
      if (character.isAlive()) {
        return false;
      }
    }

    return true;
  }

  public override bool loseCheck() {
    foreach (PlayerCharacter character in findPlayers()) {
      if (character.isAlive()) {
        return false;
      }
    }

    return true;
  }

  // Use this for initialization
  void Start() {

  }
}
