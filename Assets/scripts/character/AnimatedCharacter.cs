using UnityEngine;
using System.Collections;

public class AnimatedCharacter : MonoBehaviour {
  Character character;
  NavMeshAgent navMeshAgent;

  public bool deathAnimated { get; protected set; }

  public string defaultAnimation;
  public string combatIdleAnimation;
  public string panicAnimation;
  public string walkAnimation;
  public string runAnimation;
  public string searchingAnimation;
  public string deathAnimation;

  Hashtable animWrapMode;

  public float wanderProbability = 0.01f;
  public float maxWanderTime = 10f;
  public float wanderTimeout { get; protected set; }
  public float initialWanderTimeout;

  public bool panicked { get; private set; }

  // Use this for initialization
  void OnEnable() {
    character = GetComponent<Character>();
    navMeshAgent = GetComponent<NavMeshAgent>();
    character.deathEvent.AddListener(handleDeathEvent);

    deathAnimated = false;
    wanderTimeout = initialWanderTimeout;

    animWrapMode = new Hashtable();

    animWrapMode[defaultAnimation] = WrapMode.Loop;
    animWrapMode[combatIdleAnimation] = WrapMode.Loop;
    animWrapMode[panicAnimation] = WrapMode.Loop;
    animWrapMode[walkAnimation] = WrapMode.Loop;
    animWrapMode[runAnimation] = WrapMode.Loop;
    animWrapMode[searchingAnimation] = WrapMode.Loop;
    animWrapMode[deathAnimation] = WrapMode.Once;

    foreach (Attack attack in character.attacks) {
      foreach (string animName in attack.validAnimationNames) {
        animWrapMode[animName] = WrapMode.Once;
      }
    }

    setAnim(defaultAnimation);
  }

  void handleDeathEvent() {
    StartCoroutine(animateDeath());
  }

  // Update is called once per frame
  void Update() {

  }

  public bool wander() {
    if (wanderTimeout > Time.time && Random.value <= wanderProbability) {
      wanderTimeout = Time.time + maxWanderTime;

      return true;
    } else {
      return false;
    }
  }

  public void chill() {
    panicked = false;
  }

  public void panic() {
    panicked = true;
  }

  public void idle() {
    setAnim(getIdleAnim());
  }

  public Animation getAnimation() {
    return this.GetComponent<Animation>();
  }

  public void search() {
    setAnim(searchingAnimation);
  }

  public string getMoveAnim() {
    return (panicked ? runAnimation : walkAnimation);
  }

  public void setAnim(string animationName) {
    Animation animation = getAnimation();

    animation.wrapMode = (WrapMode) animWrapMode[animationName];
    animation.CrossFade(animationName);
  }

  public void blendAnim(string animationName) {
    Animation animation = getAnimation();

    animation.wrapMode = (WrapMode) animWrapMode[animationName];
    animation.Blend(animationName);
  }

  public string getIdleAnim() {
    return (panicked ? defaultAnimation : combatIdleAnimation);
  }

  public void animateAttack(Attack attack) {
    StartCoroutine(performAnimateAttack(attack));
  }

  protected IEnumerator performAnimateAttack(Attack attack) {
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

    if (attack.weapon != null) {
      attack.weapon.clearListeners();
    }
  }

  public IEnumerator animateDeath() {
    Animation animation = getAnimation();

    setAnim(deathAnimation);
    getAnimation()[deathAnimation].speed = 2.0f;

    deathAnimated = true;

    do {
      yield return null;
    } while (animation.isPlaying);

    navMeshAgent.Stop();
    navMeshAgent.enabled = false;
    gameObject.SetActive(false);
  }
}
