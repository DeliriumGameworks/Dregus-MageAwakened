using UnityEngine;
using System.Collections;


public class CalibrationResult {
  public enum Status {
    nullStatus,
    success,
    failure,
  }

  public Status calibrationStatus = Status.nullStatus;
  public float value = 0f;
  public string message = null;
}
