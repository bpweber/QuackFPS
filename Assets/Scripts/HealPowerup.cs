using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealPowerup : MonoBehaviour
{
    private Player player;
    public float powerupRespawnTimer = 10f;
    private float nextPowerupTime;
    private bool powerupEnabled = true;

    private void Update()
    {
        if(Time.time > nextPowerupTime && !powerupEnabled)
        {
            GetComponent<BoxCollider>().enabled = true;
            GetComponentInChildren<MeshRenderer>().enabled = true;
            powerupEnabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(powerupEnabled)
        {;
            player = other.GetComponent<Player>();
            if (player.GetHealth() >= player.GetMaxHealth())
                return;
            nextPowerupTime = Time.time + powerupRespawnTimer;
            player.SetHealth(player.GetMaxHealth());
            other.GetComponent<PlayerHealth>().playHealSound();
            GetComponent<BoxCollider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            powerupEnabled = false;
        }

    }
}
