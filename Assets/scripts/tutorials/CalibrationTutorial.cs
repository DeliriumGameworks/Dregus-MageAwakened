using UnityEngine;
using System.Collections;

public class CalibrationTutorial : MonoBehaviour {
  public Vector3 offset;

  public Transform[] transforms = new Transform[0];
  public int currentIndex { get; protected set; }

  // Use this for initialization
  void Start() {
    if (transforms.Length > 0) {
      offset = this.transform.position - transforms[0].transform.position;
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public void moveTo(int index) {
    Debug.Log("Current index: " + currentIndex + " / New index: " + index);
    Debug.Log("Current pos: " + this.transform.position + " / New position: " + (transforms[index].transform.position + offset));
    this.transform.position = transforms[index].transform.position + offset;
    currentIndex = index;
  }
}
