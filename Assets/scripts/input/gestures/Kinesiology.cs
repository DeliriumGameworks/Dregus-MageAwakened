using UnityEngine;
using System.Collections;
using System;

using Controller = VRTK.VRTK_ControllerEvents;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using KinesiologyList = System.Collections.Generic.List<Kinesiology>;

[Serializable]
public class Kinesiology {
  [NonSerialized]
  public const int KINESIOLOGY_VERSION = 1;
  [NonSerialized]
  private static KinesiologyList kinesiologies;
  [NonSerialized]
  private Controller[] controllers = new Controller[2];
  [NonSerialized]
  private string filename;

  public DateTime creationDate;
  public System.Guid playerId;
  public string playerName;
  public float armSpan;
  public float shoulderHeight;
  public float armLength;
  public float eyesHeight;
  public int kinesiologyVersion;
  public GameObject staffPrefab;

  public Kinesiology() {
    playerId = new System.Guid();
    playerName = "";
    armSpan = 0f;
    shoulderHeight = 0f;
    armLength = 0f;
    eyesHeight = 0f;
    kinesiologyVersion = KINESIOLOGY_VERSION;
  }

  public Controller left {
    get {
      return controllers[0];
    }
    set {
      controllers[0] = value;
    }
  }

  public Controller right {
    get {
      return controllers[1];
    }
    set {
      controllers[1] = value;
    }
  }

  public static KinesiologyList getKinesiologies() {
    if (kinesiologies == null) {
      string[] filenames = Directory.GetFiles(Application.persistentDataPath, "*.kin");
      kinesiologies = new KinesiologyList();

      for (int i = 0; i < filenames.Length; ++i) {
        kinesiologies[i] = load(filenames[i]);
      }
    }

    return kinesiologies;
  }

  private static Kinesiology load(string filename) {
    string fullpath = Path.Combine(Application.persistentDataPath, filename);
    Kinesiology kinesiology = null;

    if (File.Exists(fullpath)) {
      BinaryFormatter bf = new BinaryFormatter();

      FileStream file = File.OpenRead(fullpath);
      kinesiology = (Kinesiology) bf.Deserialize(file);
      file.Close();

      kinesiology.filename = filename;
    }

    return kinesiology;
  }

  private static void save(Kinesiology kinesiology, string filename) {
    if (!filename.EndsWith(".kin")) {
      filename += ".kin";
    }

    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.OpenWrite(Path.Combine(Application.persistentDataPath, filename));

    bf.Serialize(file, kinesiology);

    kinesiology.filename = filename;

    file.Close();

    for (int i = 0; i < kinesiologies.Count; ++i) {
      if (kinesiologies[i].playerId == kinesiology.playerId) {
        kinesiologies[i] = kinesiology;
        return;
      }
    }

    kinesiologies.Add(kinesiology);
  }

  public static Kinesiology get(System.Guid playerGuid) {
    foreach (Kinesiology k in getKinesiologies()) {
      if (k.playerId == playerGuid) {
        return k;
      }
    }

    return null;
  }
}
