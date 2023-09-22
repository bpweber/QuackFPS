using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class DisplayHealth : NetworkBehaviour
{
    public GameObject player;
    public Image damageIndicator;
    public Image healthBarFill;

    private PlayerHealth playerHealth;
    private AudioSource damageSound;
    private TMP_Text hpText;
    private GameObject sliderObject;
    private Slider healthBar;

    void Start()
    {
        hpText = GetComponent<TMP_Text>();
        sliderObject = transform.GetChild(0).gameObject;
        healthBar = sliderObject.GetComponent<Slider>();
        damageSound = GetComponent<AudioSource>();
        playerHealth = player.GetComponent<PlayerHealth>();

        if (!IsOwner)
        {
            hpText.enabled = false;
            sliderObject.SetActive(false);
            damageIndicator.enabled = false;
            damageSound.enabled = false;
        }         
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if(!hpText.text.Equals("+ " + Math.Round(playerHealth.currentHealth, 0) + " | " + playerHealth.maxHealth))
            StartCoroutine(FlashDamageIndicator());

        hpText.SetText("+ " + Math.Round(playerHealth.currentHealth, 0) + " | " + playerHealth.maxHealth);
        healthBar.value = playerHealth.currentHealth / playerHealth.maxHealth;   
        if(playerHealth.currentHealth < 25)
        {
            hpText.color = Color.red;
            healthBarFill.color = Color.red;
        }
        else
        {
            hpText.color = Color.white;
            healthBarFill.color = Color.white;
        }
    }

    IEnumerator FlashDamageIndicator()
    {
        damageSound.Play();
        damageIndicator.enabled = true;
        yield return new WaitForSeconds(0.1f);
        damageIndicator.enabled = false;
    }
}
