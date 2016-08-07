using UnityEngine;
using System.Collections;

public class CreateSound : SoundEffect {
  public float chanceToPlaySound = 1f;

  // Use this for initialization
  void Start() {
    if (isValid()) {
      if (Random.Range(0f, 1f) <= chanceToPlaySound) {
        audioSource.PlayOneShot(getRandomAudioClip());
      }
    }
  }

  // Update is called once per frame
  void Update() {

  }
}
