using UnityEngine;
using System.Collections;

using Controller = VRTK.VRTK_ControllerEvents;

public class DataPersistenceManager : MonoBehaviour {
  public class Controllers {
    private static Controller[] mControllers;
    private static Controller[] controllers {
      get {
        if (mControllers == null || mControllers.Length == 0) {
          mControllers = GameObject.FindObjectsOfType<Controller>();
        }

        return mControllers;
      }
    }

    public static Controller left {
      get {
        if (controllers[0] == null) {
          mControllers = null;
        }

        return controllers[0];
      }
    }

    public static Controller right {
      get {
        if (controllers[1] == null) {
          mControllers = null;
        }

        return controllers[1];
      }
    }
  }
  public static Kinesiology loadedKinesiology { get; set; }
  static DataPersistenceManager instance;

  // Use this for initialization
  void Awake() {
    if (instance != null) {
      Destroy(this.gameObject);
    }

    DontDestroyOnLoad(this.gameObject);
    instance = this;
    loadedKinesiology = null;
  }

  // Update is called once per frame
  void Update() {
  }
}
