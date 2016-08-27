using UnityEngine;
using System.Collections;
using List = System.Collections.Generic.List<float>;

public class SortedFloatList {
  public enum SortMethod {
    binary,
    selection
  }

  List mList = new List();
  private bool ascending;
  private SortMethod sortMethod;

  public SortedFloatList(bool ascending = true, SortMethod sortMethod = SortMethod.binary) {
    this.ascending = ascending;
    this.sortMethod = sortMethod;
  }

  public void Add(float value) {
    int index;

    switch (sortMethod) {
      case SortMethod.binary:
        if (ascending) {
          index = binaryInsert(value);
        } else {
          index = binaryInsertDesc(value);
        }

        break;
      case SortMethod.selection:
        index = selectionInsert(value);

        break;
      default:
        throw new System.Exception("Invalid SortMethod");
    }

    if (index > this.Count) {
      mList.Add(value);
    } else {
      mList.Insert(index, value);
    }
  }

  private int selectionInsert(float value) {
    for (int i = 0; i < mList.Count; ++i) {
      if (ascending) {
        if (mList[i] > value) {
          return i;
        }
      } else {
        if (mList[i] < value) {
          return i;
        }
      }
    }

    return this.Count;
  }

  private int binaryInsert(float value) {
    if (this.Count == 0) {
      return 0;
    }

    int lowerBound = 0;
    int upperBound = this.Count - 1;
    int curIn = 0;

    while (true) {
      curIn = (upperBound + lowerBound) / 2;

      if (this[curIn] == value) {
        return curIn;
      } else if (this[curIn] < value) {
        lowerBound = curIn + 1; // its in the upper

        if (lowerBound > upperBound) {
          return curIn + 1;
        }
      } else {
        upperBound = curIn - 1; // its in the lower

        if (lowerBound > upperBound) {
          return curIn;
        }
      }
    }
  }

  private int binaryInsertDesc(float value) {
    if (this.Count == 0) {
      return 0;
    }

    int lowerBound = 0;
    int upperBound = this.Count - 1;
    int curIn = 0;

    while (true) {
      curIn = (upperBound + lowerBound) / 2;

      if (this[curIn] == value) {
        return curIn;
      } else if (this[curIn] > value) {
        lowerBound = curIn + 1; // its in the upper

        if (lowerBound > upperBound) {
          return curIn + 1;
        }
      } else {
        upperBound = curIn - 1; // its in the lower

        if (lowerBound > upperBound) {
          return curIn;
        }
      }
    }
  }

  public float Get(int index) {
    return mList[index];
  }

  public int Count {
    get {
      return mList.Count;
    }
  }

  public float this[int index] {
    get {
      return Get(index);
    }
  }

  public IEnumerator GetEnumerator() {
    return mList.GetEnumerator();
  }

  public string ToJson() {
    string message = "[";

    for (int i = 0; i < Count; ++i) {
      message += Get(i);

      if (i + 1 != Count) {
        message += ",";
      } else {
        message += "]";
      }
    }

    return message;
  }

  public void Clear() {
    mList.Clear();
  }

  /** Aggregation Helpers **/

  /**
   * @returns the median of the list (or average of the two median values if an even number of elements)
   */
  public float median {
    get {
      switch (this.Count) {
        case 0:
          return 0f;
        case 1:
          return this[0];
      }

      if (this.Count % 2 == 0) {
        return ((this[Mathf.FloorToInt(this.Count / 2f)] + this[Mathf.FloorToInt(this.Count / 2f) - 1]) / 2);
      } else {
        return this[Mathf.FloorToInt(this.Count / 2f)];
      }
    }
  }

  /**
   * @returns the average of the list
   */
  public float average {
    get {
      if (this.Count == 0) {
        return 0f;
      }

      float sum = 0f;

      foreach (float f in this) {
        sum += f;
      }

      return sum / this.Count;
    }
  }

  /**
   * @returns the range of the list
   */
  public float range {
    get {
      if (this.Count == 0) {
        return 0f;
      }

      return Mathf.Abs(this[this.Count - 1] - this[0]);
    }
  }

  /**
   * @returns the max value from the list
   */
  public float max {
    get {
      if (ascending) {
        if (this.Count == 0) {
          return 0f;
        }

        return this[this.Count - 1];
      } else {
        return this[0];
      }
    }
  }

  /**
   * @returns the min value from the list
   */
  public float min {
    get {
      if (ascending) {
        return this[0];
      } else {
        if (this.Count == 0) {
          return 0f;
        }

        return this[this.Count - 1];
      }
    }
  }
}
