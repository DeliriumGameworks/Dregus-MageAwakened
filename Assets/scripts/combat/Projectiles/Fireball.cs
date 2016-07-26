using UnityEngine;
using System.Collections;

/// <summary>
/// Handle collision of a fire projectile
/// </summary>
/// <param name="script">Script</param>
/// <param name="pos">Position</param>
public delegate void FireballCollisionDelegate(Fireball fireball, Vector3 pos);

public class Fireball : BaseProjectile {

  [Tooltip("Particle systems that must be manually started and will not be played on start.")]
  public ParticleSystem[] ManualParticleSystems;

  [Tooltip("The particle system to play upon collision.")]
  public ParticleSystem projectileExplosionParticleSystem;

  [Tooltip("Particle systems to destroy upon collision.")]
  public ParticleSystem[] ProjectileDestroyParticleSystemsOnCollision;

  [Tooltip("The sound to play upon collision.")]
  public AudioSource projectileCollisionSound;

  [Tooltip("The radius of the explosion upon collision.")]
  public float projectileExplosionRadius = 50.0f;

  [Tooltip("The force of the explosion upon collision.")]
  public float projectileExplosionForce = 50.0f;

  [Tooltip("How much force to create at the center (explosion), 0 for none.")]
  public float forceAmount;

  [Tooltip("The radius of the force, 0 for none.")]
  public float forceRadius;

  [HideInInspector]
  public FireballCollisionDelegate CollisionDelegate;

  // Use this for initialization
  void Start() {
    start();
  }

  protected override void start() {
    base.start();

    // if this effect has an explosion force, apply that now
    createExplosion(gameObject.transform.position, forceRadius, forceAmount);

    // start any particle system that is not in the list of manual start particle systems
    startParticleSystems();

    StartCoroutine(SendCollisionAfterDelay());
  }

  // Update is called once per frame
  void Update() {
    // reduce the duration
    duration -= Time.deltaTime;
    if (stopping) {
      // increase the stop time
      stopTimeIncrement += Time.deltaTime;
      if (stopTimeIncrement < stopTime) {
        stopPercent = stopTimeIncrement * stopTimeMultiplier;
      }
    } else if (starting) {
      // increase the start time
      startTimeIncrement += Time.deltaTime;
      if (startTimeIncrement < startTime) {
        startPercent = startTimeIncrement * startTimeMultiplier;
      } else {
        starting = false;
      }
    } else if (duration <= 0.0f) {
      // time to stop, no duration left
      stop();
    }
  }

  public override void handleCollision(GameObject obj, Collision c) {
    if (collided) {
      // already collided, don't do anything
      return;
    }

    // stop the projectile
    collided = true;
    stop();

    // destroy particle systems after a slight delay
    if (ProjectileDestroyParticleSystemsOnCollision != null) {
      foreach (ParticleSystem p in ProjectileDestroyParticleSystemsOnCollision) {
        GameObject.Destroy(p, 0.1f);
      }
    }

    // play collision sound
    if (projectileCollisionSound != null) {
      projectileCollisionSound.Play();
    }

    // if we have contacts, play the collision particle system and call the delegate
    if (c.contacts.Length != 0) {
      projectileExplosionParticleSystem.transform.position = c.contacts[0].point;
      projectileExplosionParticleSystem.Play();
      createExplosion(c.contacts[0].point, projectileExplosionRadius, projectileExplosionForce);
      if (CollisionDelegate != null) {
        CollisionDelegate(this, c.contacts[0].point);
      }
    }
  }

  public static void createExplosion(Vector3 pos, float radius, float force) {
    if (force <= 0.0f || radius <= 0.0f) {
      return;
    }

    // find all colliders and add explosive force
    Collider[] objects = UnityEngine.Physics.OverlapSphere(pos, radius);
    foreach (Collider h in objects) {
      Rigidbody r = h.GetComponent<Rigidbody>();
      if (r != null) {
        r.AddExplosionForce(force, pos, radius);
      }
    }
  }

  private void startParticleSystems() {
    foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>()) {
      if (ManualParticleSystems == null || ManualParticleSystems.Length == 0 ||
          System.Array.IndexOf(ManualParticleSystems, p) < 0) {
        if (p.startDelay == 0.0f) {
          // wait until next frame because the transform may change
          p.startDelay = 0.01f;
        }
        p.Play();
      }
    }
  }
}
