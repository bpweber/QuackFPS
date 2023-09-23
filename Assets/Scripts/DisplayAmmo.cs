using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DisplayAmmo : NetworkBehaviour
{
    public GameObject wepHolder;
    public WeaponSwitcher weaponSwitcher;

    private TMP_Text ammoText;

    void Start()
    {
        ammoText = GetComponent<TMP_Text>();
        if(!IsOwner)
            ammoText.enabled = false;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        ammoText.SetText($"{wepHolder.transform.GetChild(weaponSwitcher.activeWep).GetComponent<RaycastShoot>().ammo}");
        if (wepHolder.transform.GetChild(weaponSwitcher.activeWep).GetComponent<RaycastShoot>().ammo < 1)
            ammoText.color = Color.red;
        else 
            ammoText.color = Color.white;
    }
}
