using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterAttackEvent : UnityEvent<AICharacter, Attack> {

}

public class AICharacter : MonoBehaviour {
  private Character character;
  private AnimatedCharacter animatedCharacter;
  private Collider collider;

  public CharacterAttackEvent characterAttackEvent = new CharacterAttackEvent();

  private GameObject target;
  private float targetStale = 2f;
  private float targetExpiration = 0f;

  public enum BaddyNavStrategy {
    wanderAndLineOfSight,
    chaseClosestNoLineOfSight,
  }

  public GameObject projectileInstantiationPoint;

  public BaddyNavStrategy baddyNavStrategy = BaddyNavStrategy.wanderAndLineOfSight;

  public float baseMoveSpeed = 1f;
  public float panicMoveSpeed = 1.5f;

  public float lastTargetTime = 0f;

  protected float globalAttackCooldown = 1f;

  public float startleRange = 0f;
  public float approachToRange = 2f;
  public float detectionRange = 10f;
  public float wanderRadius = 20f;
  public float wanderVariance = 0.2f;

  public bool searching { get; protected set; }
  public bool panicked { get; protected set; }

  public float panicTimeout = 10f;

  public float lastAttackTime = 0f;

  public float visibilityH;

  void OnEnable() {
    character = GetComponent<Character>();
    animatedCharacter = GetComponent<AnimatedCharacter>();
    getCollider().enabled = true;
    lastAttackTime = 0f;

    searching = false;
    chill();
  }

  // Update is called once per frame
  void Update() {
    if (!character.isAlive()) {
      stopMoving();
      getCollider().enabled = false;

      return;
    }

    if (baddyNavStrategy == BaddyNavStrategy.wanderAndLineOfSight) {
      wanderAndLineOfSight();
    } else if (baddyNavStrategy == BaddyNavStrategy.chaseClosestNoLineOfSight) {
      chaseClosestNoLineOfSight();
    }
  }

  protected float getSpeed() {
    return (panicked ? panicMoveSpeed : baseMoveSpeed);
  }

  protected bool search() {
    if (searching) {
      return true;
    }

    animatedCharacter.search();

    return true;
  }

  protected bool amMoving() {
    NavMeshAgent navMeshAgent = getNavMeshAgent();

    return distanceFrom(navMeshAgent.destination) > navMeshAgent.stoppingDistance + 0.25f;
  }

  protected float distanceFrom(GameObject go) {
    return distanceFrom(go.transform.position);
  }

  protected float distanceFrom(Vector3 position) {
    return (position - this.transform.position).magnitude;
  }

  protected void stopMoving() {
    getNavMeshAgent().destination = transform.position;
    getNavMeshAgent().Stop();
  }

  protected bool wander() {
    if (animatedCharacter.wander()) {
      NavMeshAgent navMeshAgent = getNavMeshAgent();

      moveTo(randomWanderDestination());

      navMeshAgent.stoppingDistance = 0;

      return true;
    }

    return false;
  }

  protected bool moveTo(Vector3 destination) {
    NavMeshAgent navMeshAgent = getNavMeshAgent();

    if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) {
      animatedCharacter.setAnim(animatedCharacter.getMoveAnim());

      navMeshAgent.speed = getSpeed();
      navMeshAgent.destination = destination;
      navMeshAgent.stoppingDistance = approachToRange;

      return true;
    }

    return false;
  }

  protected NavMeshAgent getNavMeshAgent() {
    return GetComponent<NavMeshAgent>();
  }

  protected Collider getCollider() {
    if (collider == null) {
      collider = GetComponent<Collider>();
    }

    return collider;
  }

  protected Vector3 randomWanderDestination() {
    Vector3 destination = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    float x = -1;
    float z = -1;

    if (Random.value < 0.5f) {
      x = 1;
    }

    if (Random.value < 0.5f) {
      z = 1;
    }

    x *= Random.Range(wanderRadius * (1 - wanderVariance), wanderRadius);
    z *= Random.Range(wanderRadius * (1 - wanderVariance), wanderRadius);

    destination.x += x;
    destination.z += z;

    return destination;
  }

  GameObject findTarget() {
    if (target != null) {
      if (Time.time > targetExpiration) {
        targetExpiration = Time.time + targetStale;

        target = null;
      } else {
        return target;
      }
    }

    foreach (PlayerCharacter pc in FindObjectsOfType<PlayerCharacter>()) {
      GameObject go = pc.gameObject;
      if (isValidTarget(go)) {
        if (target == null) {
          target = go;
        } else {
          if (distanceFrom(target) > distanceFrom(go)) {
            target = go;
          }
        }
      }
    }

    return target;
  }

  bool isValidTarget(GameObject go) {
    if (baddyNavStrategy == BaddyNavStrategy.chaseClosestNoLineOfSight) {
      return true;
    }

    if (distanceFrom(go) <= detectionRange) {
      if (panicked) {
        return true;
      } else {
        return withinVisibilityCone(go);
      }
    }

    return false;
  }

  bool withinVisibilityCone(GameObject go) {
    Vector3 forward = transform.forward;
    Vector3 xzdirection = go.transform.position - transform.position;
    //Vector3 yDirection = new Vector3(0, xzdirection.y, 0);
    float angle;

    // Only concerned with x and z first
    xzdirection.y = 0;

    angle = Vector3.Angle(forward, xzdirection);

    return angle <= visibilityH;

    /*
     * TODO Implement a cone?  Might not even be necessary really.  Ramps aren't good in VR anyway.
    if (angle > visibilityH) {
      Debug.Log("vH (" + angle + " > " + visibilityH + ")");
      return false;
    }

    // Only concerned with y now
    angle = Vector3.Angle(forward, yDirection);

    Debug.DrawRay(transform.position, forward * 5, Color.red);

    Debug.Log("vV (" + angle + " > " + visibilityH + ")");
    return angle <= visibilityV;
    */
  }

  protected bool move(GameObject target) {
    if (target != null) {
      Vector3 targetVector;

      /*
      RaycastHit hit;

      if (Physics.Raycast(target.transform.position, -Vector3.up, out hit)) {
        targetVector = new Vector3();

        targetVector.x = target.transform.position.x;
        targetVector.y = hit.collider.gameObject.transform.position.y;
        targetVector.z = target.transform.position.z;
      } else {
        targetVector = target.transform.position;
      }
      */

      if (distanceFrom(target) <= approachToRange) {
        return false;
      }

      targetVector = target.transform.position;

      return moveTo(targetVector);
    }

    return false;
  }

  protected void chaseClosestNoLineOfSight() {
    if (!character.isAlive()) {
      if (!animatedCharacter.deathAnimated) {
        stopMoving();
        animatedCharacter.animateDeath();
      }

      return;
    }

    GameObject target = findTarget();

    if (target != null) {
      searching = false;

      lastTargetTime = Time.time;

      if (!attack(target)) {
        move(target);
      }
    }
  }

  public void chill() {
    panicked = false;
    animatedCharacter.chill();
  }

  public void panic() {
    panicked = true;
    animatedCharacter.panic();
  }

  protected void wanderAndLineOfSight() {
    if (!character.isAlive()) {
      if (!animatedCharacter.deathAnimated) {
        stopMoving();
        animatedCharacter.animateDeath();
      }

      return;
    }

    GameObject target = findTarget();

    if (lastTargetTime + panicTimeout < Time.time) {
      chill();
      searching = false;
    }

    if (target != null) {
      searching = false;

      lastTargetTime = Time.time;

      if (!attack(target)) {
        move(target);
      }
    } else {
      if (!panicked) {
        wander();
      } else {
        stopMoving();
        search();
      }

      if (!amMoving()) {
        animatedCharacter.idle();
      }
    }
  }

  bool attack(GameObject target) {
    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 0.5f);

    if (lastAttackTime + globalAttackCooldown < Time.time) {
      if (target != null) {
        panic();

        Attack attack = randomValidAttack(target);

        if (attack == null) {
          return false;
        }

        if (attack.weapon != null) {
          Weapon weapon = attack.weapon;

          attack.swing();

          weapon.addOnHitListener(onHitListener, attack);
        } else if (attack.projectile != null) {
          BaseProjectile projectile;
          Vector3 originPoint = (projectileInstantiationPoint == null ? transform.position : projectileInstantiationPoint.transform.position);

          projectile = (BaseProjectile) Instantiate(attack.projectile, originPoint, transform.rotation);
          projectile.owner = character;

          projectile.addOnHitListener(onHitListener, attack);
        }

        lastAttackTime = float.MaxValue;

        animatedCharacter.animateAttack(attack);

        characterAttackEvent.Invoke(this, attack);

        animatedCharacter.attackAnimationFinished.AddListener(attackFinish);

        return true;
      }
    } else {
      if (lastAttackTime > Time.time) {
        // still attacking, don't move
        return true;
      } else {
        // cooling down attack, safe to move
        return false;
      }
    }

    return false;
  }

  void attackFinish() {
    lastAttackTime = Time.time;
  }

  void onHitListener(Attack attack, Collider collider) {
    Character colliderCharacter = Character.findCharacter(collider);

    if (colliderCharacter != null) {
      Attack.Element damageType = Attack.Element.Physical;

      if (attack.weapon != null) {
        damageType = attack.weapon.damageType;
      } else if (attack.projectile != null) {
        damageType = attack.weapon.damageType;
      }

      attack.landAttack(GetComponent<Character>(), colliderCharacter, damageType);
    }

    attack.weapon.clearListeners();
  }

  Attack randomValidAttack(GameObject target) {
    int validAttacksCount = 0;

    int[] validAttackIndices = new int[character.attacks.Length];

    foreach (Attack attack in character.attacks) {
      if (isValidAttack(attack, target)) {
        ++validAttacksCount;
      }
    }

    if (validAttacksCount == 0) {
      return null;
    }

    return character.attacks[validAttackIndices[Random.Range(0, validAttacksCount)]];
  }

  bool isValidAttack(Attack attack, GameObject target) {
    float dFromT = distanceFrom(target);

    return (dFromT <= attack.maximumRange && dFromT >= attack.minimumRange);
  }
}
