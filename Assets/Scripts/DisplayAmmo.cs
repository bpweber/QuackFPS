using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAmmo : NetworkBehaviour
{
    private Player player;
    private TMP_Text ammoText;
    private Image gunImg;
    private Color defaultColor;
    private RaycastShoot rcs;

    void Start()
    {
        if (IsOwner)
            player = transform.root.GetComponent<Player>();
        ammoText = GetComponent<TMP_Text>();
        gunImg = ammoText.transform.GetChild(0).GetComponent<Image>();
        defaultColor = ammoText.color;
        if (!IsOwner)
        {
            ammoText.enabled = false;
            gunImg.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner)
            return;
        rcs = player.GetItemInHand().GetComponent<RaycastShoot>();
        ammoText.SetText($"{rcs.ammo}");
        if (rcs.ammo <= (0.25 * (float) rcs.maxAmmo))
        {
            ammoText.color = Color.red;
            gunImg.color = Color.red;
        }
        else
        {
            ammoText.color = defaultColor;
            gunImg.color = defaultColor;
        }
    }
}
