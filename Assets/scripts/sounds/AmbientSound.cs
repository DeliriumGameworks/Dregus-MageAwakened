using UnityEngine;
using System.Collections;

public class AmbientSound : SoundEffect {
  // Use this for initialization
  void Start() {
    if (isValid()) {
      playLoopRandomAudioClip();
    }
  }

  // Update is called once per frame
  void Update() {

  }
}
