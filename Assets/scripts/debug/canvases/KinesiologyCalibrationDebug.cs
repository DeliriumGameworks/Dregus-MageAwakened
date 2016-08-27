using UnityEngine;
using System.Collections;

public class KinesiologyCalibrationDebug : MonoBehaviour {
  private UnityEngine.UI.Text text;
  public float refreshRate = .1f;
  private float nextRefresh = 0f;

  void Start() {
    text = GetComponent<UnityEngine.UI.Text>();
  }

  // Update is called once per frame
  void Update () {
    if (Time.time > nextRefresh) {
      nextRefresh = Time.time + refreshRate;
    } else {
      return;
    }

    text.text = "";

    text.text += "Height:   " + Mathf.RoundToInt(KinesiologyCalibration.getCurrentPlayerHeight() * 100) + "cm";
    text.text += "\n";
    text.text += "Armspan:  " + Mathf.RoundToInt(KinesiologyCalibration.getCurrentPlayerArmspan() * 100) + "cm";
    text.text += "\n";
    text.text += "Shoulder: " + Mathf.RoundToInt(KinesiologyCalibration.getCurrentPlayerShoulderHeight() * 100) + "cm";
    text.text += "\n";
    text.text += "Torso:    " + Mathf.RoundToInt(KinesiologyCalibration.getCurrentPlayerTorsoWidth() * 100) + "cm";
    text.text += "\n";
    text.text += "LeftConf: " + KinesiologyCalibration.left0Confidence;
    text.text += "\n";
    text.text += "Saved:    " + KinesiologyCalibration.exported;
  }
}
