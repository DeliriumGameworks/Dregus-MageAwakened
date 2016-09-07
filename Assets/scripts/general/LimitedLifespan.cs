using UnityEngine;
using System.Collections;

public class LimitedLifespanExpirationEvent : UnityEngine.Events.UnityEvent<LimitedLifespan> { }

public class LimitedLifespan : MonoBehaviour {
  public enum CreationType {
    undefined,
    pooling,
    instantiation
  }

  public LimitedLifespanExpirationEvent limitedLifespanExpirationEvent = new LimitedLifespanExpirationEvent();

  public const float DEFAULT_LIFESPAN = float.MaxValue;
  public float lifespan { get; set; }
  float birth { get; set; }
  public bool destroyed { get; private set; }
  public CreationType creationType { get; private set; }

  // Covers pooling
  void OnEnable() {
    Init(CreationType.pooling);
  }

  // Covers instantiation
  void Awake() {
    Init(CreationType.instantiation);
  }

  void Init(CreationType pCreationType) {
    creationType = pCreationType;

    lifespan = DEFAULT_LIFESPAN;
    birth = Time.time;
  }

  // Update is called once per frame
  void Update() {
    if (!destroyed && lifespan != DEFAULT_LIFESPAN && birth + lifespan < Time.time) {
      if (creationType == CreationType.pooling) {
        this.gameObject.SetActive(false);
        limitedLifespanExpirationEvent.Invoke(this);
      } else {
        Destroy(this.gameObject);
      }
    }
  }
}
