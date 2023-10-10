using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAmmo : NetworkBehaviour
{
    private Player player;
    private WeaponSwitcher weaponSwitcher;
    private TMP_Text ammoText;
    private Image gunImg;
    private Image awpImg;
    private Image engImg;
    private Color defaultColor;
    private RaycastShoot rcs;

    void Start()
    {
        if (IsOwner)
            player = transform.root.GetComponent<Player>();
        weaponSwitcher = player.GetComponent<WeaponSwitcher>();
        ammoText = GetComponent<TMP_Text>();
        gunImg = ammoText.transform.GetChild(0).GetComponent<Image>();
        awpImg = ammoText.transform.GetChild(1).GetComponent<Image>();
        engImg = ammoText.transform.GetChild(2).GetComponent<Image>();
        defaultColor = ammoText.color;
        if (!IsOwner)
        {
            ammoText.enabled = false;
            gunImg.enabled = false;
            awpImg.enabled = false;
            engImg.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner)
            return;
        awpImg.enabled = weaponSwitcher.activeWep == 3;
        gunImg.enabled = weaponSwitcher.activeWep < 3;
        engImg.enabled = weaponSwitcher.activeWep == 4;


        ammoText.enabled = weaponSwitcher.activeWep != 4;

        rcs = player.GetItemInHand().GetComponent<RaycastShoot>();
        ammoText.SetText($"{rcs.ammo}");
        if (rcs.ammo <= (0.25 * (float) rcs.maxAmmo))
        {
            ammoText.color = Color.red;
            gunImg.color = Color.red;
            awpImg.color = Color.red;
        }
        else
        {
            ammoText.color = defaultColor;
            gunImg.color = defaultColor;
            awpImg.color = defaultColor;
        }
    }
}
