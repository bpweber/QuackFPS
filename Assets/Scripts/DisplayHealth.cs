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
    public Image damageIndicator;
    public Image healthBarFill;
    public Image healthBarBG;
    public Animator anim;

    private Player player;
    private PlayerHealth playerHealth;
    private AudioSource damageSound;
    private TMP_Text hpText;
    private GameObject sliderObject;
    private Slider healthBar;
    private Color defaultColor;


    void Start()
    {
        if (IsOwner)
            player = transform.root.GetComponent<Player>();
        hpText = GetComponent<TMP_Text>();
        sliderObject = transform.GetChild(0).gameObject;
        healthBar = sliderObject.GetComponent<Slider>();
        damageSound = GetComponent<AudioSource>();

        defaultColor = hpText.color;

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

        int prevHealth = Int32.Parse(hpText.text);

        if(Math.Round(player.GetHealth(), 0) < prevHealth)
            FlashDamageIndicator(false);

        hpText.SetText($"{Math.Round(player.GetHealth(), 0)}");
        healthBar.value = player.GetHealth() / player.GetMaxHealth();
        if (player.GetHealth() <= 25)
        {
            hpText.color = Color.red;
            healthBarFill.color = Color.red;
            healthBarBG.color = new Color(255, 0, 0, 0.25f);
        }
        else
        {
            hpText.color = defaultColor;
            healthBarFill.color = defaultColor;
            healthBarBG.color = new Color(defaultColor.r, defaultColor.b, defaultColor.g, 0.25f);
        }
    }

    public void FlashDamageIndicator(bool killShot)
    {
        if (damageSound.enabled)
        {
            if(killShot)
                damageSound.volume = 1f;
            else
                damageSound.volume = 0.5f;
            damageSound.Play();
        }
        anim.SetTrigger("FlashDamage");
    }
}
