using UnityEngine;
using System.Collections.Generic;

public class GestureController : MonoBehaviour {
  public GameObject fireNovaPrefab;
  public GameObject baseBeamAttackPrefab;
  public GameObject fireBeamAttackPrefab;

  public Kinesiology kinesiology { get; set; }

  public SteamVR_Camera steamVRCamera { get; private set; }

  public float fireNovaLifespan { get; set; }
  public float fireBeamAttackLifespan { get; set; }

  List<GameObject> centeredOnGameObjectList = new List<GameObject>();
  List<GameObject> orientedAlongShoulderToControllerList = new List<GameObject>();

  List<Gesture> gesturesToTest = new List<Gesture>();

  void Awake() {
    kinesiology = null;
    GestureLibrary.novaGestureCompletedEvent.AddListener(fireNova);
    GestureLibrary.beamAttackCompletedEvent.AddListener(fireFireBeamAttack);
    fireNovaLifespan = 1.5f;
    fireBeamAttackLifespan = 1f;

    if (kinesiology == null) {
      kinesiology = DataPersistenceManager.loadedKinesiology;
    }

    gesturesToTest.Add(GestureLibrary.nova());
    gesturesToTest.Add(GestureLibrary.beamAttack());
  }

  void Update() {
    if (kinesiology != null) {
      ensureSaved();
      testAttacks();
      updateAttackPositions();
    }
  }

  void ensureSaved() {
    if (kinesiology.playerName == "") {
      kinesiology.playerName = "Nolfithorp";
    }

    if (!kinesiology.saved) {
      Kinesiology.save(kinesiology, "Nolfithorp");
    }
  }

  void testAttacks() {
    foreach (Gesture gesture in gesturesToTest) {
      gesture.test(kinesiology);
    }
  }

  void updateAttackPositions() {
    foreach (GameObject go in centeredOnGameObjectList) {
      if (go.activeSelf) {
        go.transform.position = getCenterOfControllerPositions();
      }
    }

    foreach (GameObject go in orientedAlongShoulderToControllerList) {
      if (go.activeSelf) {
        go.transform.rotation = getQuaternionBasedOnApproximateTorsoMeetsNeckLineToControllers();
      }
    }
  }

  void fireNova(Gesture gesture) {
    LimitedLifespan limitedLifespan;
    GameObject nova = instantiateAttack(fireNovaPrefab, out limitedLifespan);

    limitedLifespan.lifespan = fireNovaLifespan;

    nova.transform.position = getCenterOfControllerPositions();
    nova.transform.Rotate(Vector3.left, 90);
    nova.transform.position += (Vector3.up * kinesiology.eyesHeight / 2);
  }

  void fireFireBeamAttack(Gesture gesture) {
    LimitedLifespan limitedLifespan;
    GameObject beamAttack = instantiateAttack(fireBeamAttackPrefab, out limitedLifespan);

    limitedLifespan.lifespan = fireBeamAttackLifespan;

    beamAttack.transform.position = getCenterOfControllerPositions();

    centeredOnGameObjectList.Add(beamAttack);
    orientedAlongShoulderToControllerList.Add(beamAttack);
  }

  // TODO: Add a grabAttackFromPool equivalent to instantiateAttack below so we're not constantly
  //       instantiating and destroying the same objects over and over again.
  //       This is some low hanging fruit for optimization and FPS improvements.

  /**
   * An optional instantiation method that automates some of the repetitive tasks for 
   * setting up an attack.
   * 
   * @param go    The prefab to instantiate a copy of.
   */
  GameObject instantiateAttack(GameObject go, out LimitedLifespan limitedLifespan) {
    GameObject instance = (GameObject) Instantiate(go);

    limitedLifespan = instance.GetComponent<LimitedLifespan>();

    if (limitedLifespan == null) {
      limitedLifespan = instance.AddComponent<LimitedLifespan>();
    }

    limitedLifespan.limitedLifespanExpirationEvent.AddListener(handleAttackExpiration);

    Character character = GetComponentInParent<Character>();
    Attack attack = instance.GetComponent<Attack>();

    if (character != null && attack != null) {
      attack.attackingCharacter = character;
    }

    return instance;
  }

  void handleAttackExpiration(LimitedLifespan limitedLifespan) {
    centeredOnGameObjectList.Remove(limitedLifespan.gameObject);
    orientedAlongShoulderToControllerList.Remove(limitedLifespan.gameObject);
  }

  Vector3 getCenterOfControllerPositions() {
    return (kinesiology.left.transform.position + kinesiology.right.transform.position) / 2;

    // Another possible implementation:
    //return Vector3.Lerp(kinesiology.left.transform.position, kinesiology.right.transform.position, 0.5f);
  }

  Quaternion getQuaternionBasedOnApproximateTorsoMeetsNeckLineToControllers() {
    return Quaternion.LookRotation(getCenterOfControllerPositions() - getApproximateTorsoMeetsNeckPosition(), Vector3.up);
  }

  Vector3 getApproximateTorsoMeetsNeckPosition() {
    if (steamVRCamera == null) steamVRCamera = FindObjectOfType<SteamVR_Camera>();

    Vector3 position = steamVRCamera.transform.position;

    position.y -= ((kinesiology.eyesHeight - kinesiology.shoulderHeight) * .75f);

    return position;
  }
}
