using UnityEngine;
using System.Collections;

public class SpawnerPool : MonoBehaviour {
  public GameObject[] prefabs = new GameObject[0];
  public int quantity = 24;

  private GameObject[][] pool;

  // Use this for initialization
  void Start() {
    pool = new GameObject[prefabs.Length][];

    for (int i = 0; i < prefabs.Length; ++i) {
      pool[i] = new GameObject[quantity];

      for (int j = 0; j < quantity; ++j) {
        pool[i][j] = Instantiate(prefabs[i]);

        pool[i][j].GetComponent<NavMeshAgent>().enabled = false;
        pool[i][j].SetActive(false);
      }
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public GameObject borrowPrefab(GameObject prefab) {
    GameObject[] prefabPool = getPrefabPool(prefab);

    for (int i = 0; i < quantity; ++i) {
      if (prefabPool[i].activeSelf == false) {
        prefabPool[i].SetActive(true);

        return prefabPool[i];
      }
    }

    //TODO Autogrow pool
    throw new System.Exception("Not enough prefabs, consider increasing the quantity in the pool.");
  }

  GameObject[] getPrefabPool(GameObject prefab) {
    for (int i = 0; i < prefabs.Length; ++i) {
      if (prefabs[i] == prefab) {
        return pool[i];
      }
    }

    return null;
  }
}
