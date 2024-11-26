using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;
    public Tree tree;

    Vector3 newDirection;

    private void Awake()
    {
        Instance = this;
    }

    public void Fire()
    {
        tree.Fire(); 
        FireDirection(3);
    }

    public void FireDirection(float direction)
    {
        switch (direction)
        {
            case 1:
                newDirection = Vector3.right * 45;
                break;
            case 2: 
                newDirection = Vector3.back  * 45;
                break;
            case 3: 
                newDirection = Vector3.left  * 45;
                break;
            case 4: 
                newDirection = Vector3.forward  * 45;
                break;
        }

        FireEvents.TriggerWindDirection(newDirection);
    }
}
