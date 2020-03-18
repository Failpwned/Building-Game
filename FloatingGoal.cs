using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingGoal : MonoBehaviour, IWinCondition
{
    private Collider otherCollider = null;
    public bool IsFulfilled
    {
        get
        {
            if(otherCollider == null)
            {
                return false;
            }
            else
            {
                Solid3D solid = otherCollider.GetComponentInParent<Solid3D>();
                if(solid != null)
                {
                    return solid.IsPowered;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private void Start()
    {
        LevelManager.Current.WinConditions.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        otherCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        otherCollider = null;
    }

}
