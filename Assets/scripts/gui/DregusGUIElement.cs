using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class DregusGUIElement : MonoBehaviour {
  public string format = "%s";
  public string[] keyBindings = new string[1];

  private const float updateFrequency = .1f; // s
  private float nextUpdateTime = 0f;

  static Regex regex;

  public string text {
    get {
      return gameObject.GetComponent<Text>().text;
    }
    protected set {
      gameObject.GetComponent<Text>().text = value;
    }
  }

  void Start() {
    if (regex == null) {
      regex = new Regex("%s");
    }
  }

  void Update() {
    if (Time.time < nextUpdateTime) {
      return;
    }

    nextUpdateTime = Time.time + updateFrequency;

    text = string.Copy(format);

    foreach (string keyBinding in keyBindings) {
      text = regex.Replace(text, DregusGUILookup.get(keyBinding), 1);
    }
  }
}
