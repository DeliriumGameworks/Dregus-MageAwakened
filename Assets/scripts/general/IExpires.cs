using UnityEngine;
using System.Collections;

public interface IExpires {
  float birth { get; set; }
  float lifespan { get; set; }

  void OnExpires();
}
