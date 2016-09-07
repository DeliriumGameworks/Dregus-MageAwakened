using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CalibrationLevel : MonoBehaviour {
  private const int LINE_LENGTH = 20;

  private enum CalibrationLevelState {
    hypnosis,
    hypnosisPrint,
    height,
    calculatingHeight,
    heightPrint,
    tpose,
    calculatingTpose,
    tposePrint,
    finished
  }

  public GameObject paper;
  public float delayToStartWriting = 7f;
  public GameObject saveStaffStand;
  public GameObject defaultStaffPrefab;

  private const float calibrateHeightTime = 3f;

  private CalibrationLevelState calibrationLevelState = CalibrationLevelState.hypnosis;
  private CalibrationLevelState lastPrintState = CalibrationLevelState.finished;
  private bool writing = false;
  private float[] heights;
  private bool grabbedToStart = false;

  public Kinesiology kinesiology { get; private set; }

  public SaveFab[] saveFabs;

  public float tPoseBegin = 0f;
  public float tPoseLength = 3f;
  public float tPoseEnd = 0f;

  public float pencilPoseBegin = 0f;
  public float pencilPoseLength = 3f;
  public float pencilPoseEnd = 0f;

  public bool finishedCalibration = false;

  void Awake() {
    List<Kinesiology> kinesiologies = Kinesiology.getKinesiologies();

    kinesiology = null;

    heights = new float[Mathf.RoundToInt(calibrateHeightTime * 100)];

    saveFabs = saveStaffStand.GetComponentsInChildren<SaveFab>();

    GameObject go;

    for (int i = 0; i < saveFabs.Length; ++i) {
      if (kinesiologies.Count > i) {
        if (kinesiologies[i].staffPrefab == null) {
          go = (GameObject) Instantiate(defaultStaffPrefab, saveFabs[i].holster);
        } else {
          go = (GameObject) Instantiate(kinesiologies[i].staffPrefab, saveFabs[i].holster);
        }
        go.AddComponent<KinesiologyLoader>().kinesiologyToLoad = kinesiologies[i];

        saveFabs[i].textObject.text = kinesiologies[i].playerName;
      } else {
        go = (GameObject) Instantiate(defaultStaffPrefab, saveFabs[i].holster);
        go.AddComponent<KinesiologyLoader>().kinesiologyToLoad = new Kinesiology();
      }

      go.transform.localPosition = Vector3.zero;

      go.GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectGrabbed += staffGrabbed;
    }

    StartCoroutine(write("Grab an orb to begin."));
  }

  void staffGrabbed(object sender, VRTK.InteractableObjectEventArgs e) {
    grabbedToStart = true;

    kinesiology = ((VRTK.VRTK_InteractableObject) sender).GetComponent<KinesiologyLoader>().kinesiologyToLoad;
  }

  private void OnEyesightCalibrated(CalibrationResult c) {
    if (c.calibrationStatus == CalibrationResult.Status.success) {

    } else {
      StartCoroutine(writeThenDelay(c.message, 2f));
    }
  }

  void Update() {
    if (finishedCalibration) return;

    if (kinesiology == null || !kinesiology.saved) {
      if (grabbedToStart && tPoseBegin == 0f) {
        tPoseBegin = Time.time + 3f;
        tPoseEnd = tPoseBegin + tPoseLength;
        StartCoroutine(write("Stand in a T pose for ~" + tPoseLength + " seconds"));
      }

      if (tPoseEnd != 0f && Time.time > tPoseEnd && pencilPoseBegin == 0f) {
        pencilPoseBegin = Time.time + 3f;
        pencilPoseEnd = pencilPoseBegin + pencilPoseLength;
        StartCoroutine(write("Stand in a pencil pose for ~" + pencilPoseLength + " seconds"));
      }

      recordTpose();
      recordPencilPose();
      saveKinesiology();
    } else if (kinesiology.saved) {
      FindObjectOfType<GestureController>().kinesiology = kinesiology;
      DataPersistenceManager.loadedKinesiology = kinesiology;

      StartCoroutine(writeStats());

      finishedCalibration = true;
    }
  }

  void recordTpose() {
    if (MathUtil.between(tPoseBegin, Time.time, tPoseEnd)) {
      KinesiologyCalibration.recordPlayerHeight();
      KinesiologyCalibration.recordPlayerArmspan();
      KinesiologyCalibration.recordPlayerShoulderHeight();
      KinesiologyCalibration.recordLeftRight();
    }
  }

  void recordPencilPose() {
    if (MathUtil.between(pencilPoseBegin, Time.time, pencilPoseEnd)) {
      KinesiologyCalibration.recordPlayerTorsoWidth();
    }
  }

  void saveKinesiology() {
    if (pencilPoseEnd != 0f && Time.time > pencilPoseEnd) {
      kinesiology = KinesiologyCalibration.exportToKinesiology();
      FindObjectOfType<GestureController>().kinesiology = kinesiology;

      StartCoroutine(writeStats());

      for (int i = 0; i < saveFabs.Length; ++i) {
        if (saveFabs[i].GetComponent<KinesiologyLoader>() == null) {
          saveFabs[i].textObject.text = kinesiology.playerName;
          break;
        }
      }

      foreach (LevelLoadTrigger t in FindObjectsOfType<LevelLoadTrigger>()) {
        t.enabled = true;
      }

      finishedCalibration = true;
    }
  }

  void clear() {
    paper.GetComponentInChildren<Text>().text = "";
  }

  void writeChar(char l) {
    paper.GetComponentInChildren<Text>().text += l;
  }

  IEnumerator write(string message) {
    clear();

    yield return StartCoroutine(append(message));
  }

  IEnumerator append(string message) {
    if (!writing) {
      writing = true;

      message = StringUtil.SplitToLines(message, ' ', LINE_LENGTH);

      for (int i = 0; i < message.Length; ++i) {
        writeChar(message[i]);

        yield return new WaitForSeconds(0.1f);
      }

      writing = false;
    }
  }

  IEnumerator writeThenDelay(string message, float delay) {
    yield return StartCoroutine(write(message));

    yield return new WaitForSeconds(delay);
  }


  IEnumerator writeThenIncrementState(string message) {
    yield return StartCoroutine(write(message));

    ++calibrationLevelState;
  }

  IEnumerator writeCalibrateHeight() {
    yield return StartCoroutine(writeThenDelay("Look straight ahead for " + Mathf.RoundToInt(calibrateHeightTime) + " seconds", 0.5f));

    Debug.Log("Kinesiology's eyesHeight calibrated");

    ++calibrationLevelState;
  }

  IEnumerator writeCalibrateTpose() {
    string tposeMessage = "Spread your arms into a T pose.";

    yield return StartCoroutine(writeThenDelay(tposeMessage, 1.5f));

    Debug.Log("Kinesiology's armSpan and shoulderHeight calibrated");

    ++calibrationLevelState;
  }

  IEnumerator writeStats() {
    clear();

    string[] messages = new string[] {
      "Height: " + (Mathf.Round(100f * kinesiology.eyesHeight) / 100f) + "m ",
      "Shoulders: " + (Mathf.Round(100f * kinesiology.shoulderHeight) / 100f) + "m ",
      "Armspan: " + (Mathf.Round(100f * kinesiology.armSpan) / 100f) + "m ",
      "TorsoWid: " + (Mathf.Round(100f * kinesiology.torsoWidth) / 100f) + "m ",
      "Armlen: " + (Mathf.Round(100f * kinesiology.armLength) / 100f) + "m ",
      "Punch that rotating thing to continue"
    };

    string message = "";

    for (int i = 0; i < messages.Length; ++i) {
      message += messages[i];
    }

    Debug.Log(message);

    yield return StartCoroutine(write(message));
  }

  IEnumerator writeStatsThenIncrement() {
    yield return StartCoroutine(writeStats());

    ++calibrationLevelState;
  }
}
