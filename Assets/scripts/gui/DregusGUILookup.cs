using UnityEngine;
using System.Collections;

public class DregusGUILookup : MonoBehaviour {
  protected static Hashtable hashtable;

  protected static Hashtable getHashtable() {
    if (hashtable == null) {
      hashtable = new Hashtable();
    }

    return hashtable;
  }

  protected static void add(string key, string value) {
    getHashtable().Add(key, value);
  }

  public static void set(string key, string value) {
    if (contains(key)) {
      getHashtable()[key] = value;
    } else {
      add(key, value);
    }
  }

  public static string get(string key) {
    if (contains(key)) {
      return (string) getHashtable()[key];
    } else {
      return "";
    }
  }

  public static bool contains(string key) {
    return getHashtable().ContainsKey(key);
  }
}
