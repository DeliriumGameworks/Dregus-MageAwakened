using UnityEngine;
using System.Collections;

public class BaddyCharacter : MonoBehaviour {
  AICharacter aiCharacter;
  AnimatedCharacter animatedCharacter;

  // Use this for initialization
  void OnEnable() {
    aiCharacter = GetComponent<AICharacter>();
    animatedCharacter = GetComponent<AnimatedCharacter>();
  }
}
