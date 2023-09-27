using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int wepIndex = -1;
    public float powerupRespawnTimer = 1f;

    private float nextPowerupTime;
    private bool powerupEnabled = true;
    private Transform wepHolder;
    private RaycastShoot rcs;

    private void Update()
    {
        if (Time.time > nextPowerupTime && !powerupEnabled)
        {
            GetComponent<BoxCollider>().enabled = true;
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
                renderer.enabled = true;
            //GetComponentInChildren<MeshRenderer>().enabled = true;
            powerupEnabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (powerupEnabled)
        {
            wepHolder = other.GetComponent<Player>().GetItemInHand().transform.parent;
            rcs = wepHolder.GetChild(wepIndex).GetComponent<RaycastShoot>();
            if (rcs.ammo < rcs.maxAmmo * 2)
                StartCoroutine(rcs.Reload());
            nextPowerupTime = Time.time + powerupRespawnTimer;
            GetComponent<BoxCollider>().enabled = false;
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
                renderer.enabled = false;
            //GetComponentInChildren<MeshRenderer>().enabled = false;
            powerupEnabled = false;
        }
    }
}
