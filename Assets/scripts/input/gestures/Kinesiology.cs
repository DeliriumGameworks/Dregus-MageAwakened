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
  private string filename;

  [NonSerialized]
  GameObject mStaffPrefab;
  public GameObject staffPrefab {
    get {
      return mStaffPrefab;
    }
    set {
      mStaffPrefab = value;
    }
  }

  string mPlayerName;
  public string playerName {
    get {
      return mPlayerName;
    }
    set {
      mPlayerName = value;
      saved = false;
    }
  }

  DateTime mCreationDate;
  public DateTime creationDate {
    get {
      return mCreationDate;
    }
    set {
      mCreationDate = value;
      saved = false;
    }
  }

  System.Guid mPlayerId;
  public System.Guid playerId {
    get {
      return mPlayerId;
    }
    set {
      mPlayerId = value;
      saved = false;
    }
  }

  float mArmSpan;
  public float armSpan {
    get {
      return mArmSpan;
    }
    set {
      mArmSpan = value;
      saved = false;
    }
  }

  float mShoulderHeight;
  public float shoulderHeight {
    get {
      return mShoulderHeight;
    }
    set {
      mShoulderHeight = value;
      saved = false;
    }
  }

  float mArmLength;
  public float armLength {
    get {
      return mArmLength;
    }
    set {
      mArmLength = value;
      saved = false;
    }
  }

  float mEyesHeight;
  public float eyesHeight {
    get {
      return mEyesHeight;
    }
    set {
      mEyesHeight = value;
      saved = false;
    }
  }

  float mTorsoWidth;
  public float torsoWidth {
    get {
      return mTorsoWidth;
    }
    set {
      mTorsoWidth = value;
      saved = false;
    }
  }

  int mKinesiologyVersion;
  public int kinesiologyVersion {
    get {
      return mKinesiologyVersion;
    }
    private set {
      mKinesiologyVersion = value;
      saved = false;
    }
  }

  bool mSaved;
  public bool saved {
    get {
      return mSaved;
    }
    private set {
      mSaved = value;
    }
  }

  public Kinesiology() {
    playerId = new System.Guid();
    playerName = "";
    armSpan = 0f;
    shoulderHeight = 0f;
    armLength = 0f;
    eyesHeight = 0f;
    kinesiologyVersion = KINESIOLOGY_VERSION;
    saved = false;
  }

  public Controller left {
    get {
      return DataPersistenceManager.Controllers.left;
    }
  }

  public Controller right {
    get {
      return DataPersistenceManager.Controllers.right;
    }
  }

  public static KinesiologyList getKinesiologies() {
    if (kinesiologies == null) {
      string[] filenames = Directory.GetFiles(Application.persistentDataPath, "*.kin");
      kinesiologies = new KinesiologyList();

      for (int i = 0; i < filenames.Length; ++i) {
        kinesiologies.Add(load(filenames[i]));
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

      kinesiology.kinesiologyVersion = KINESIOLOGY_VERSION;
      kinesiology.filename = filename;
      kinesiology.saved = true;
    }

    return kinesiology;
  }

  public static void save(Kinesiology kinesiology, string filename) {
    if (kinesiology.saved) {
      return;
    }

    if (!filename.EndsWith(".kin")) {
      filename += ".kin";
    }

    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.OpenWrite(Path.Combine(Application.persistentDataPath, filename));

    bf.Serialize(file, kinesiology);

    kinesiology.filename = filename;

    file.Close();

    Debug.Log(file.Name + "  Application.persistentDataPath:" + Application.persistentDataPath);

    kinesiology.saved = true;

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
