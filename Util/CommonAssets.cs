using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonAssets : MonoBehaviour
{
    public static CommonAssets Current { get; private set; }

    public GameObject selector3;
    public GameObject selector4;

    private void Awake()
    {
        Current = this;
    }
}
