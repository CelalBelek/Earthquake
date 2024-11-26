using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPhysicsEvents
{
    public static event Action OnEarthquake;
    public static event Action<float> OnPower;

    public static void TriggerEarthquake() => OnEarthquake?.Invoke();
    public static void TriggerPower(float value) => OnPower?.Invoke(value);

    public static void ClearAllEvents()
    {
        OnEarthquake = null;
        OnPower = null;
    }
}
