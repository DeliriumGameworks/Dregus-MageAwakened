using UnityEngine;
using System.Collections;

public class MathUtil {
  /**
   * @returns true if a < b < c
   */
  public static bool between(float a, float b, float c) {
    return (a < b) && (b < c);
  }
}
