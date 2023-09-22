using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DisplayAmmo : NetworkBehaviour//MonoBehaviour
{
    public GameObject wepHolder;
    public WeaponSwitcher weaponSwitcher;

    private TMP_Text ammoText;

    // Start is called before the first frame update
    void Start()
    {
        ammoText = GetComponent<TMP_Text>();
        if(!IsOwner)
            ammoText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
        ammoText.SetText("" + wepHolder.transform.GetChild(weaponSwitcher.activeWep).GetComponent<RaycastShoot>().ammo);
    }
}
