using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPhysicsEvents
{
    public static event Action OnPlay;
    public static event Action<float> OnEarth;

    public static void TriggerPlay() => OnPlay?.Invoke();
    public static void TriggerEarth(float value) => OnEarth?.Invoke(value);
}
