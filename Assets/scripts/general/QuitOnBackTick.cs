using UnityEngine;
using System.Collections;

public class QuitOnBackTick : MonoBehaviour {

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (Input.GetKey(KeyCode.BackQuote)) {
      Application.Quit();
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
  }
}
