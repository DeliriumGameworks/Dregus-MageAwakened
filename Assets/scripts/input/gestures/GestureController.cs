using UnityEngine;
using System.Collections.Generic;

public class GestureController : MonoBehaviour {
  List<GestureListener> gestureListeners = new List<GestureListener>();

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  void registerGestureListener(GestureListener gestureListener) {
    gestureListeners.Add(gestureListener);
  }

  void deregisterGestureListener(GestureListener gestureListener) {
    gestureListeners.Remove(gestureListener);
  }
}
