using UnityEngine;
using System.Collections;

public interface GestureListener {
  void onGesture180YDoubleExtend(SteamVR_Controller left, SteamVR_Controller right, SteamVR_Camera camera);
}
