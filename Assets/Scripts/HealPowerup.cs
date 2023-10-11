using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealPowerup : MonoBehaviour
{
    private Player player;
    public float powerupRespawnTimer = 60f;
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
        {
            if (other.GetComponent<Player>() == null)
                return;
            player = other.GetComponent<Player>();
            if (player.GetHealth() >= player.GetMaxHealth() * 1.5f)
                return;
            nextPowerupTime = Time.time + powerupRespawnTimer;
            if(player.GetHealth() >= player.GetMaxHealth())
                player.SetHealth(player.GetMaxHealth() * 1.5f);
            else
                player.SetHealth(player.GetMaxHealth());
            other.GetComponent<PlayerHealth>().playHealSound();
            GetComponent<BoxCollider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            powerupEnabled = false;
        }

    }
}
