using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int wepIndex = -1;

    private Transform wepHolder;
    private RaycastShoot rcs;

    private void OnTriggerEnter(Collider other)
    {
        wepHolder = other.GetComponent<Player>().GetItemInHand().transform.parent;
        rcs = wepHolder.GetChild(wepIndex).GetComponent<RaycastShoot>();
        if(rcs.ammo < rcs.maxAmmo * 2)
            StartCoroutine(rcs.Reload());
    }
}
