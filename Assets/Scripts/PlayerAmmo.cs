using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int wepIndex = -1;
    public Transform wepHolder;

    private RaycastShoot rcs;

    private void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        wepHolder = other.transform.GetChild(0).GetChild(4).GetChild(3);
        rcs = wepHolder.GetChild(wepIndex).GetComponent<RaycastShoot>();
        if(rcs.ammo < rcs.maxAmmo)
            StartCoroutine(rcs.Reload());
        Debug.Log("Reload" + wepIndex);
    }
}
