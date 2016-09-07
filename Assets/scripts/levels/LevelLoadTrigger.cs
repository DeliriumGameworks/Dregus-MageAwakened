using UnityEngine;
using System.Collections;

public class LevelLoadTrigger : MonoBehaviour {
  public string levelToLoad;

  public void loadLevel() {
    UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
    //SteamVR_LoadLevel.Begin(levelToLoad);
  }

  void OnTriggerEnter(Collider c) {
    loadLevel();
  }
}
