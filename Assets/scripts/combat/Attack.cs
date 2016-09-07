using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class AttackLandedEvent : UnityEvent<Character, Character, Attack, float> {

}

public class Attack : MonoBehaviour {
  [HideInInspector]
  public Character attackingCharacter { get; set; }
  [HideInInspector]
  public AttackLandedEvent attackEvent = new AttackLandedEvent();
  [HideInInspector]
  public UnityEvent initialized = new UnityEvent();
  [HideInInspector]
  public UnityEvent wielded = new UnityEvent();
  [HideInInspector]
  public UnityEvent swung = new UnityEvent();
  [HideInInspector]
  public UnityEvent swingFinished = new UnityEvent();
  [HideInInspector]
  public UnityEvent finalized = new UnityEvent();

  [Tooltip("Whether or not the attack can damage teammates of the player performing the attack")]
  public bool friendlyFire = false;

  [Tooltip("Whether or not the attack can damage the player performing the attack")]
  public bool selfFire = false;

  public enum Element { undefined, True, Physical, Acid, Fire, Frost, Lightning }

  public float baseMaximumDamage = 0;
  public float variance = 0;

  public Weapon weapon;
  public BaseProjectile projectile;

  public string attackName = "";

  public float minimumRange = 0;
  public float maximumRange = 5;

  public string[] validAnimationNames;

  AudioSource audioSource;

  public AudioClip[] initSfx;
  public AudioClip[] continuousSfx;
  public AudioClip[] wieldSfx;
  public AudioClip[] swingSfx;
  public AudioClip[] landAttackSfx;
  public AudioClip[] finalizeSfx;

  void Awake() {
    init();
  }

  void OnEnable() {
    init();
  }

  public void init() {
    if (audioSource == null) {
      audioSource = this.gameObject.GetComponent<AudioSource>();

      if (audioSource == null) {
        audioSource = this.gameObject.AddComponent<AudioSource>();
      }
    }

    initialized.Invoke();

    playOnce(getRandomAudioClip(initSfx));
    playContinuously(getRandomAudioClip(continuousSfx));
  }

  public void swing() {
    swung.Invoke();
    playOnce(getRandomAudioClip(swingSfx));
  }

  public void wield() {
    wielded.Invoke();
    playOnce(getRandomAudioClip(wieldSfx));
  }

  public void finishSwing() {
    swingFinished.Invoke();
  }

  public void finalize() {
    playOnce(getRandomAudioClip(finalizeSfx));
  }

  public void landAttack(Character attacker, Character target, Element damageType) {
    if (attacker == null) {
      if (attackingCharacter == null) {
        throw new System.Exception("Unable to find the attacking character.");
      }

      attacker = attackingCharacter;
    }

    if (attacker == target && !selfFire) return;

    float damage = getBaseDamage() * attacker.damageMultiplier;

    if (target.applyDamage(damage, damageType, attacker.gameObject)) {
      attacker.onKill(target);
    }

    attackEvent.Invoke(attacker, target, this, damage);

    playOnce(getRandomAudioClip(landAttackSfx));
  }

  private float getBaseDamage() {
    return Random.Range(baseMaximumDamage * (1 - variance), baseMaximumDamage);
  }

  AudioClip getRandomAudioClip(AudioClip[] audioClips) {
    if (audioClips == null || audioClips.Length == 0) {
      return null;
    }

    return audioClips[Random.Range(0, audioClips.Length - 1)];
  }

  void playOnce(AudioClip sfx) {
    if (sfx == null) {
      return;
    }

    audioSource.clip = sfx;
    audioSource.loop = false;

    audioSource.Play();
  }

  void playContinuously(AudioClip sfx) {
    if (sfx == null) {
      return;
    }

    audioSource.clip = sfx;
    audioSource.loop = true;

    audioSource.Play();
  }
}
