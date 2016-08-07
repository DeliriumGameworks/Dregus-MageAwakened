using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour {
  public AudioSource audioSource;
  public AudioClip[] audioClips;

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  protected AudioClip getRandomAudioClip() {
    return audioClips[Random.Range(0, audioClips.Length - 1)];
  }

  public void playOnceRandomAudioClip() {
    audioSource.loop = false;
    audioSource.clip = getRandomAudioClip();

    audioSource.Play();
  }

  public void playLoopRandomAudioClip() {
    audioSource.loop = true;
    audioSource.clip = getRandomAudioClip();

    audioSource.Play();
  }

  public void playOnceDefault() {
    audioSource.clip = getRandomAudioClip();

    audioSource.Play();
  }

  public bool isValid() {
    return audioSource != null && audioClips != null && audioClips.Length >= 1;
  }
}
