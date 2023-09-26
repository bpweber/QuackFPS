using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{

    [SerializeField]
    private Transform wepHolder;
    [SerializeField] 
    private WeaponSwitcher weaponSwitcher;
    [SerializeField]
    private int kills = 0;
    [SerializeField]
    private int deaths = 0;
    [SerializeField]
    private float currentHealth = 100;
    [SerializeField]
    private float maxHealth = 100;

    public GameObject GetItemInHand()
    {
        return wepHolder.GetChild(weaponSwitcher.activeWep).gameObject;
    }

    public void SetKills(int newKills)
    {
        kills = newKills;
    }

    public int GetKills() 
    { 
        return kills; 
    }

    public void SetDeaths(int newDeaths)
    {
        deaths = newDeaths;
    }

    public int GetDeaths() 
    {  
        return deaths; 
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
