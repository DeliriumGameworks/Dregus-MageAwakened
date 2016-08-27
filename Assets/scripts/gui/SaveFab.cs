using UnityEngine;
using System.Collections;

public class SaveFab : MonoBehaviour {
  public Transform holster;
  public UnityEngine.UI.Text textObject;

  // Use this for initialization
  void Start() {
    if (textObject == null) {
      textObject = GetComponentInChildren<UnityEngine.UI.Text>();
    }
  }

  // Update is called once per frame
  void Update() {

  }
}
