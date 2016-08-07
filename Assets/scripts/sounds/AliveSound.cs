using UnityEngine;
using System.Collections;

// TODO: Implement this
public class AliveSound : SoundEffect {
  Character character;

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (character != null && character.isAlive()) {
      //playOnceRandomAudioClip();
    }
  }
}
