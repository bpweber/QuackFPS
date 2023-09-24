using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealPowerup : MonoBehaviour
{
    public float powerupRespawnTimer = 10f;
    private float nextPowerupTime;
    private PlayerHealth health;
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
            health = other.GetComponent<PlayerHealth>();
            if (health.currentHealth >= health.maxHealth)
                return;
            nextPowerupTime = Time.time + powerupRespawnTimer;
            health.currentHealth = health.maxHealth;
            health.playHealSound();
            GetComponent<BoxCollider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            powerupEnabled = false;
        }

    }
}
