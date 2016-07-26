using UnityEngine;
using System.Collections;

public class BaddyCharacter : Character {
  public float visibilityH;
  //public float visibilityV;

  public float startleRange = 0f;
  public float approachToRange = 2f;
  public float detectionRange = 10f;

  public string defaultAnimation;
  public string combatIdleAnimation;
  public string panicAnimation;
  public string walkAnimation;
  public string runAnimation;
  public string searchingAnimation;
  public string deathAnimation;

  public float baseMoveSpeed = 1f;
  public float panicMoveSpeed = 1.5f;

  public float wanderVariance = 0.2f;
  public float wanderRadius = 20f;
  public float wanderProbability = 0.01f;

  public float maxWanderTime = 10f;
  float wanderTimeout;

  public float panicTimeout = 10f;
  bool panicked = false;

  public float lastTargetTime = 0f;

  bool searching = false;

  float globalAttackCooldown = 1f;
  float lastAttackTime = 0;

  bool deathAnimated = false;

  Hashtable animWrapMode;

  // Use this for initialization
  void Start() {
    start();
  }

  protected override void start() {
    base.start();

    animWrapMode = new Hashtable();

    animWrapMode[defaultAnimation] = WrapMode.Loop;
    animWrapMode[combatIdleAnimation] = WrapMode.Loop;
    animWrapMode[panicAnimation] = WrapMode.Loop;
    animWrapMode[walkAnimation] = WrapMode.Loop;
    animWrapMode[runAnimation] = WrapMode.Loop;
    animWrapMode[searchingAnimation] = WrapMode.Loop;
    animWrapMode[deathAnimation] = WrapMode.Once;

    foreach (Attack attack in attacks) {
      foreach (string animName in attack.validAnimationNames) {
        animWrapMode[animName] = WrapMode.Once;
      }
    }

    setAnim(defaultAnimation);
  }

  // Update is called once per frame
  void Update() {
    if (!isAlive()) {
      if (!deathAnimated) {
        stopMoving();
        setAnim(deathAnimation);
        getAnimation()[deathAnimation].speed = 2.0f;

        deathAnimated = true;
      }


      return;
    }

    GameObject target = findTarget();

    if (lastTargetTime + panicTimeout < Time.time) {
      panicked = false;
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
        setAnim(getIdleAnim());
      }
    }
  }

  void stopMoving() {
    moveTo(this.transform.position);
  }

  bool amMoving() {
    NavMeshAgent navMeshAgent = getNavMeshAgent();

    //Debug.Log(distanceFrom(navMeshAgent.destination) + " <= " + navMeshAgent.stoppingDistance + 0.25f);

    return distanceFrom(navMeshAgent.destination) > navMeshAgent.stoppingDistance + 0.25f;
  }

  bool search() {
    if (searching) {
      return true;
    }

    searching = true;

    setAnim(searchingAnimation);

    return true;
  }

  bool wander() {
    NavMeshAgent navMeshAgent = getNavMeshAgent();

    if (wanderTimeout > Time.time) {
      return true;
    }

    if (Random.value <= wanderProbability) {
      Vector3 destination = randomWanderDestination();
      moveTo(destination);

      navMeshAgent.stoppingDistance = 0;

      wanderTimeout = Time.time + maxWanderTime;

      return true;
    }

    return false;
  }

  void setAnim(string animationName) {
    Animation animation = getAnimation();

    animation.wrapMode = (WrapMode) animWrapMode[animationName];
    animation.CrossFade(animationName);
  }

  void blendAnim(string animationName) {
    Animation animation = getAnimation();

    animation.wrapMode = (WrapMode) animWrapMode[animationName];
    animation.Blend(animationName);
  }

  string getIdleAnim() {
    return (panicked ? defaultAnimation : combatIdleAnimation);
  }

  string getMoveAnim() {
    return (panicked ? runAnimation : walkAnimation);
  }

  float getSpeed() {
    return (panicked ? panicMoveSpeed : baseMoveSpeed);
  }

  bool moveTo(Vector3 destination) {
    NavMeshAgent navMeshAgent = getNavMeshAgent();

    if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) {
      setAnim(getMoveAnim());

      navMeshAgent.speed = getSpeed();
      navMeshAgent.destination = destination;
      navMeshAgent.stoppingDistance = approachToRange;

      return true;
    }

    return false;
  }

  Vector3 randomWanderDestination() {
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

  void onHitListener(Attack attack, Collider collider) {
    Character character = findCharacter(collider);
    float damage = attack.getBaseDamage() * attack.weapon.damageMultiplier;

    if (character != null) {
      if (character.applyDamage(damage, attack.weapon.damageType)) {
        onKill(character);
      }
    } else {
      Debug.Log("Would have dealt " + damage + " damage.");
    }

    attack.weapon.clearListeners();
  }

  bool move(GameObject target) {
    if (target != null) {
      return moveTo(target.transform.position);
    }

    return false;
  }

  bool attack(GameObject target) {
    if (lastAttackTime + globalAttackCooldown < Time.time) {
      if (target != null) {
        panicked = true;

        Attack attack = randomValidAttack(target);

        if (attack == null) {
          return false;
        }

        if (attack.weapon != null) {
          attack.weapon.addOnHitListener(onHitListener, attack);
        }

        lastAttackTime = float.MaxValue;

        // This spawns a coroutine managing the animation, we won't continue this function again until the animation completes.
        StartCoroutine(animateAttack(attack));

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

  Animation getAnimation() {
    return this.GetComponent<Animation>();
  }

  NavMeshAgent getNavMeshAgent() {
    return this.GetComponent<NavMeshAgent>();
  }

  IEnumerator animateAttack(Attack attack) {
    Animation animation = getAnimation();
    int animIndex;

    if (animation == null || attack == null || attack.validAnimationNames.Length <= 0) {
      yield return null;
    }

    animIndex = Random.Range(0, attack.validAnimationNames.Length);

    blendAnim(attack.validAnimationNames[animIndex]);

    do {
      yield return null;
    } while (animation.isPlaying);

    setAnim(combatIdleAnimation);

    attack.weapon.clearListeners();

    lastAttackTime = Time.time;
  }

  Attack randomValidAttack(GameObject target) {
    int validAttacksCount = 0;

    int[] validAttackIndices = new int[attacks.Length];

    foreach (Attack attack in attacks) {
      if (isValidAttack(attack, target)) {
        ++validAttacksCount;
      }
    }

    if (validAttacksCount == 0) {
      return null;
    }

    return attacks[validAttackIndices[Random.Range(0, validAttacksCount)]];
  }

  bool isValidAttack(Attack attack, GameObject target) {
    float dFromT = distanceFrom(target);

    return (dFromT <= attack.maximumRange && dFromT >= attack.minimumRange);
  }

  GameObject findTarget() {
    GameObject target = null;

    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
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
}
