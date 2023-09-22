using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAmmo : NetworkBehaviour//MonoBehaviour
{
    public int currentAmmo = 100;
    public int maxAmmo = 100;

    void Update()
    {
        
    }
}
