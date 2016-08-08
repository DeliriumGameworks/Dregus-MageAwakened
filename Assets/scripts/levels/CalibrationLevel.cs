using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

  private CalibrationLevelState calibrationLevelState = CalibrationLevelState.hypnosis;
  private CalibrationLevelState lastPrint = CalibrationLevelState.finished;
  private bool writing = false;
  private SteamVR_Camera vrCamera;
  private float[] heights;

  private const float calibrateHeightTime = 3f;
  public Kinesiology kinesiology { get; private set; }

  // Use this for initialization
  void Start() {
    kinesiology = new Kinesiology();
    vrCamera = FindObjectOfType<SteamVR_Camera>();
    heights = new float[Mathf.RoundToInt(calibrateHeightTime * 100)];
  }

  // Update is called once per frame
  void Update() {
    if (lastPrint != calibrationLevelState) {
      Debug.Log("Transitioned from " + lastPrint + " to " + calibrationLevelState);

      lastPrint = calibrationLevelState;
    }

    switch (calibrationLevelState) {
      case CalibrationLevelState.hypnosis:
        if (!writing && Time.time > delayToStartWriting) {
          Debug.Log("Hypnosis printing.");
          StartCoroutine(writeThenIncrementState("Please stand up straight and prepare for hypnosis."));

          ++calibrationLevelState;
        }

        break;
      case CalibrationLevelState.height:
        Debug.Log("Height calibrating.");
        StartCoroutine(writeCalibrateHeight());

        ++calibrationLevelState;
        break;
      // Skip calculatingHeight state
      case CalibrationLevelState.heightPrint:
        if (!writing) {
          Debug.Log("Height printing.");
          StartCoroutine(writeStatsThenIncrement());
        }

        break;
      case CalibrationLevelState.tpose:
        Debug.Log("Tpose calibrating.");
        StartCoroutine(writeCalibrateTpose());

        ++calibrationLevelState;

        break;
      // Skip calculatingTpose state
      case CalibrationLevelState.tposePrint:
        if (!writing) {
          Debug.Log("Tpose printing.");
          StartCoroutine(writeStatsThenIncrement());
        }

        break;
      case CalibrationLevelState.finished:
        if (!writing && Random.Range(0, 10) == 0) {
          clear();

          StartCoroutine(writeStats());

          ++calibrationLevelState;
        }

        break;
      default:
        break;
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

      message = SplitToLines(message, ' ', LINE_LENGTH);

      for (int i = 0; i < message.Length; ++i) {
        writeChar(message[i]);

        yield return new WaitForSeconds(0.1f);
      }

      writing = false;
    }
  }

  public static string SplitToLines(string text, char splitOnCharacter, int maxStringLength) {
    return SplitToLines(text, new char[1] { splitOnCharacter }, maxStringLength);
  }

  public static string SplitToLines(string text, char[] splitOnCharacters, int maxStringLength) {
    var sb = new System.Text.StringBuilder();
    var index = 0;

    while (text.Length > index) {
      // start a new line, unless we've just started
      if (index != 0)
        sb.AppendLine();

      // get the next substring, else the rest of the string if remainder is shorter than `maxStringLength`
      var splitAt = index + maxStringLength <= text.Length
          ? text.Substring(index, maxStringLength).LastIndexOfAny(splitOnCharacters)
          : text.Length - index;

      // if can't find split location, take `maxStringLength` characters
      splitAt = (splitAt == -1) ? maxStringLength : splitAt;

      // add result to collection & increment index
      sb.Append(text.Substring(index, splitAt).Trim());
      index += splitAt;
    }

    return sb.ToString();
  }

  IEnumerator writeThenIncrementState(string message) {
    yield return StartCoroutine(write(message));

    ++calibrationLevelState;
  }

  IEnumerator writeThenDelay(string message, float delay) {
    yield return StartCoroutine(write(message));

    yield return new WaitForSeconds(delay);
  }

  IEnumerator writeCalibrateHeight() {
    yield return StartCoroutine(writeThenDelay("Look straight ahead for " + Mathf.RoundToInt(calibrateHeightTime) + " seconds", 0.5f));

    float startTime = Time.time;

    LayerMask layerMask = LayerMask.GetMask("floor");
    RaycastHit raycastHit;
    int heightsIter = 0;

    if (startTime + calibrateHeightTime > Time.time) {
      Physics.Raycast(vrCamera.transform.position, -Vector3.up, out raycastHit, 4f, layerMask);

      heights[heightsIter] = raycastHit.distance;
      ++heightsIter;

      yield return new WaitForFixedUpdate();
    }

    System.Array.Resize<float>(ref heights, heightsIter);

    System.Array.Sort(heights);

    if (heightsIter % 2 == 0) {
      kinesiology.eyesHeight = ((heights[Mathf.FloorToInt(heightsIter / 2f)] + heights[Mathf.FloorToInt(heightsIter / 2f) - 1]) / 2);
    } else {
      kinesiology.eyesHeight = heights[Mathf.FloorToInt(heightsIter / 2f)];
    }

    Debug.Log("Kinesiology's eyesHeight calibrated");

    ++calibrationLevelState;
  }

  IEnumerator writeCalibrateTpose() {
    string tposeMessage = "Spread your arms into a T pose.";

    yield return StartCoroutine(writeThenDelay(tposeMessage, 1.5f));

    string errorMessage = kinesiology.tryDetectSteamVR();

    while (errorMessage != null) {
      Debug.Log(errorMessage);

      yield return StartCoroutine(writeThenDelay(errorMessage, 1.5f));

      yield return StartCoroutine(writeThenDelay(tposeMessage, 1.5f));

      errorMessage = kinesiology.tryDetectSteamVR();
    }

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
