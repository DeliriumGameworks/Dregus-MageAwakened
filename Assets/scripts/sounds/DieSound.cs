using UnityEngine;
using System.Collections;

public class DieSound : SoundEffect {
  bool triggered;

  // Use this for initialization
  void Start() {
    triggered = false;
  }

  // Update is called once per frame
  void Update() {
    if (triggered) {
      return;
    }

    if (!gameObject.GetComponentInParent<Character>().isAlive()) {
      triggered = true;

      Debug.Log("Playing Death Sound");

      playOnceRandomAudioClip();
    }
  }
}
