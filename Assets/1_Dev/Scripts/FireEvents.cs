using System;
using UnityEngine;
public class FireEvents
{
    public static event Action OnFire;
    public static event Action<Vector3> OnWindDirection;

    public static void TriggerFire() => OnFire?.Invoke();
    public static void TriggerWindDirection(Vector3 direction) => OnWindDirection?.Invoke(direction);
}