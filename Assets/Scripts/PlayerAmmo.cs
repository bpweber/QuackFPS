using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int wepIndex = -1;
    public Transform wepHolder;

    private RaycastShoot rcs;

    private void OnTriggerEnter(Collider other)
    {
        wepHolder = other.transform.GetChild(0).GetChild(4).GetChild(3);
        rcs = wepHolder.GetChild(wepIndex).GetComponent<RaycastShoot>();
        if(rcs.ammo < rcs.maxAmmo * 2)
            StartCoroutine(rcs.Reload());
    }
}
