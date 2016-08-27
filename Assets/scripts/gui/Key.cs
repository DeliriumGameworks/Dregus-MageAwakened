using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {
  public Keyboard keyboard { private set; get; }

  public Key(VRTK.VRTK_InteractableObject interactableObject, Keyboard kb) {
    keyboard = kb;
    interactableObject.InteractableObjectUsed += StartUsing;
  }

  public void StartUsing(object sender, VRTK.InteractableObjectEventArgs e) {
    Debug.Log("StartUsing occured on " + this.name);
  }
}
