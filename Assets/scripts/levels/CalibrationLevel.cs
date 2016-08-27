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

  // Use this for initialization
  void Awake() {
    List<Kinesiology> kinesiologies = Kinesiology.getKinesiologies();

    kinesiology = null;

    //Kinesiology.vrCamera = FindObjectOfType<SteamVR_Camera>();
    heights = new float[Mathf.RoundToInt(calibrateHeightTime * 100)];

    saveFabs = saveStaffStand.GetComponentsInChildren<SaveFab>();

    GameObject go;

    for (int i = 0; i < saveFabs.Length; ++i) {
      if (kinesiologies.Count > i) {
        go = (GameObject) Instantiate(kinesiologies[i].staffPrefab, saveFabs[i].holster);
        go.AddComponent<KinesiologyLoader>().kinesiologyToLoad = kinesiologies[i];
      } else {
        go = (GameObject) Instantiate(defaultStaffPrefab, saveFabs[i].holster);
        go.AddComponent<KinesiologyLoader>().kinesiologyToLoad = new Kinesiology();
      }

      go.transform.localPosition = Vector3.zero;

      go.GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectGrabbed += staffGrabbed;
    }

    StartCoroutine(write("Grab a Staff to begin."));
  }

  void staffGrabbed(object sender, VRTK.InteractableObjectEventArgs e) {
    grabbedToStart = true;

    //kinesiology = ((VRTK.VRTK_InteractableObject) sender).GetComponent<KinesiologyLoader>().kinesiologyToLoad;
    kinesiology = null;

    //kinesiology.playerHeightCalibrated.AddListener(OnEyesightCalibrated);
  }

  private void OnEyesightCalibrated(CalibrationResult c) {
    if (c.calibrationStatus == CalibrationResult.Status.success) {

    } else {
      StartCoroutine(writeThenDelay(c.message, 2f));
    }
  }

  // Update is called once per frame
  void Update() {
    if (grabbedToStart && tPoseBegin == 0f) {
      tPoseBegin = Time.time + 1f;
      tPoseEnd = tPoseBegin + tPoseLength;
      StartCoroutine(write("Stand in a T pose for ~" + tPoseLength + " seconds"));
    }

    if (tPoseEnd != 0f && Time.time > tPoseEnd && pencilPoseBegin == 0f) {
      pencilPoseBegin = Time.time + 1f;
      pencilPoseEnd = pencilPoseBegin + pencilPoseLength;
      StartCoroutine(write("Stand in a pencil pose for ~" + pencilPoseLength + " seconds"));
    }

    recordTpose();
    recordPencilPose();
    saveKinesiology();
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
    if (pencilPoseEnd != 0f && Time.time > pencilPoseEnd && kinesiology == null) {
      kinesiology = KinesiologyCalibration.exportToKinesiology();
      FindObjectOfType<GestureController>().kinesiology = kinesiology;
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

    //yield return StartCoroutine(kinesiology.calibratePlayerHeight(calibrateHeightTime));

    /*
    float startTime = Time.time;

    LayerMask layerMask = LayerMask.GetMask("floor");
    RaycastHit raycastHit;
    int heightsIter = 0;

    if (startTime + calibrateHeightTime > Time.time) {
      Physics.Raycast(Kinesiology.vrCamera.transform.position, -Vector3.up, out raycastHit, 4f, layerMask);

      heights[heightsIter] = raycastHit.distance;
      ++heightsIter;

      yield return new WaitForFixedUpdate();
    }

    System.Array.Resize<float>(ref heights, heightsIter);

    System.Array.Sort(heights);

    Debug.Log("heightsIter: " + heightsIter);

    if (heightsIter % 2 == 0) {
      kinesiology.eyesHeight = ((heights[Mathf.FloorToInt(heightsIter / 2f)] + heights[Mathf.FloorToInt(heightsIter / 2f) - 1]) / 2);
    } else {
      kinesiology.eyesHeight = heights[Mathf.FloorToInt(heightsIter / 2f)];
    }

    */

    Debug.Log("Kinesiology's eyesHeight calibrated");

    ++calibrationLevelState;
  }

  IEnumerator writeCalibrateTpose() {
    string tposeMessage = "Spread your arms into a T pose.";

    yield return StartCoroutine(writeThenDelay(tposeMessage, 1.5f));

    /*
    string errorMessage = kinesiology.calibratePlayerArms();

    while (errorMessage != null) {
      Debug.Log(errorMessage);

      yield return StartCoroutine(writeThenDelay(errorMessage, 1.5f));

      yield return StartCoroutine(writeThenDelay(tposeMessage, 1.5f));

      errorMessage = kinesiology.calibratePlayerArms();
    }
    */

    Debug.Log("Kinesiology's armSpan and shoulderHeight calibrated");

    ++calibrationLevelState;
  }

  IEnumerator writeStats() {
    clear();

    string[] messages = new string[3] {
      "Height: " + (Mathf.Round(100f * kinesiology.eyesHeight) / 100f) + "m",
      "Shoulders: " + (Mathf.Round(100f * kinesiology.shoulderHeight) / 100f) + "m",
      "Armspan: " + (Mathf.Round(100f * kinesiology.armSpan) / 100f) + "m"
    };

    Debug.Log(
        messages[0] + "\n" +
        messages[1] + "\n" +
        messages[2]
        );

    if (kinesiology.eyesHeight > 0f) {
      yield return StartCoroutine(append(messages[0]));

      writeChar('\n');
    }

    if (kinesiology.shoulderHeight > 0f) {
      yield return StartCoroutine(append(messages[1]));

      writeChar('\n');
    }

    if (kinesiology.armSpan > 0f) {
      yield return StartCoroutine(append(messages[2]));

      writeChar('\n');
    }
  }

  IEnumerator writeStatsThenIncrement() {
    yield return StartCoroutine(writeStats());

    ++calibrationLevelState;
  }
}
