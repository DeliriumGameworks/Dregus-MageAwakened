using UnityEngine;
using System.Collections;

public class Keyboard : MonoBehaviour {
  Key[] keys;

  // Use this for initialization
  void Start() {
    VRTK.VRTK_InteractableObject[] interactableKeys = gameObject.GetComponentsInChildren<VRTK.VRTK_InteractableObject>();

    keys = new Key[interactableKeys.Length];
    int index = 0;

    foreach (VRTK.VRTK_InteractableObject key in interactableKeys) {
      keys[index] = key.gameObject.AddComponent<Key>();
      ++index;
    }
  }

  // Update is called once per frame
  void Update() {

  }
}
