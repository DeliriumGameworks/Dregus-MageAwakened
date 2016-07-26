using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : Weapon {
  [Tooltip("Optional audio source to play once when the script starts.")]
  public AudioSource audioSource;

  [Tooltip("The collider object to use for collision and physics.")]
  public GameObject projectileColliderObject;

  [Tooltip("The speed of the collider.")]
  public float projectileColliderSpeed = 450.0f;

  [Tooltip("An optional delay before the collider is sent off, in case the effect has a pre fire animation.")]
  public float projectileColliderDelay = 0.0f;


  [Tooltip("How long the script takes to fully start. This is used to fade in animations and sounds, etc.")]
  public float startTime = 1.0f;

  [Tooltip("How long the script takes to fully stop. This is used to fade out animations and sounds, etc.")]
  public float stopTime = 3.0f;

  [Tooltip("How long the effect lasts. Once the duration ends, the script lives for StopTime and then the object is destroyed.")]
  public float duration = 2.0f;

  protected bool collided;

  protected float startTimeMultiplier;
  protected float startTimeIncrement;

  protected float stopTimeMultiplier;
  protected float stopTimeIncrement;

  // Use this for initialization
  void Start() {
    start();
  }

  protected override void start() {
    base.start();

    if (audioSource != null) {
      audioSource.Play();
    }

    // precalculate so we can multiply instead of divide every frame
    stopTimeMultiplier = 1.0f / stopTime;
    startTimeMultiplier = 1.0f / startTime;

    collided = false;

    /*
    // If we implement the ICollisionHandler interface, see if any of the children are forwarding
    // collision events. If they are, hook into them.
    ICollisionHandler handler = (this as ICollisionHandler);
    if (handler != null) {
      FireCollisionForwardScript collisionForwarder = GetComponentInChildren<FireCollisionForwardScript>();
      if (collisionForwarder != null) {
        collisionForwarder.CollisionHandler = handler;
      }
    }
    */
  }

  // Update is called once per frame
  void Update() {

  }

  protected IEnumerator SendCollisionAfterDelay() {
    yield return new WaitForSeconds(projectileColliderDelay);

    Vector3 dir = Vector3.forward * projectileColliderSpeed;
    dir = projectileColliderObject.transform.rotation * dir;
    projectileColliderObject.GetComponent<Rigidbody>().velocity = dir;
  }

  public abstract void handleCollision(GameObject obj, Collision c);

  public virtual void stop() {
    if (stopping) {
      return;
    }
    stopping = true;

    // cleanup particle systems
    foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>()) {
      p.Stop();
    }

    StartCoroutine(CleanupEverythingCoRoutine());
  }

  private IEnumerator CleanupEverythingCoRoutine() {
    // 2 extra seconds just to make sure animation and graphics have finished ending
    yield return new WaitForSeconds(stopTime + 2.0f);

    GameObject.Destroy(gameObject);
  }

  public bool starting {
    get;
    protected set;
  }

  public float startPercent {
    get;
    protected set;
  }

  public bool stopping {
    get;
    protected set;
  }

  public float stopPercent {
    get;
    protected set;
  }

  public void OnCollisionEnter(Collision col) {
    base.OnTriggerEnter(col.collider);
    handleCollision(gameObject, col);
  }
}
