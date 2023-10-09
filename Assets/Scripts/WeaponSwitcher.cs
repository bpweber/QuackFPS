using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitcher : NetworkBehaviour
{
    public int activeWep = 0;

    public GameObject G19;
    public GameObject LG;
    public GameObject Rail;
    public GameObject AWP;
    public GameObject EngSword;
    public bool[] hasPickedUp = new bool[5];
    public Image crosshair;
    public Animator zoomAnim;
    public Animator scopeAnim;

    private GameObject[] weps = new GameObject[5];
    public bool isSwitching = false;

    private void Start()
    {
        crosshair.enabled = IsOwner;

        weps[0] = G19;
        weps[1] = LG;
        weps[2] = Rail;
        weps[3] = AWP;
        weps[4] = EngSword;

        hasPickedUp[0] = true;
        //hasPickedUp[3] = true;
        hasPickedUp[4] = true;
    }

    void Update()
    {
        if (!IsOwner) return;

        if(activeWep == 3)
            crosshair.enabled = false;
        else 
            crosshair.enabled = true;

        if (Input.GetKeyDown(KeyCode.Alpha1))       
            if (!isSwitching && activeWep != 0 && hasPickedUp[0])
                SwitchWeapon(0);       
        if (Input.GetKeyDown(KeyCode.Alpha2))
            if (!isSwitching && activeWep != 1 && hasPickedUp[1])
                SwitchWeapon(1); 
        if (Input.GetKeyDown(KeyCode.Alpha3))  
            if (!isSwitching && activeWep != 2 && hasPickedUp[2])
                SwitchWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            if (!isSwitching && activeWep != 3 && hasPickedUp[3])
                SwitchWeapon(3);
        if (Input.GetKeyDown(KeyCode.Q))
            if (!isSwitching && activeWep != 4 && hasPickedUp[4])
                SwitchWeapon(4);
    }

    public void SwitchWeapon(int wep)
    {
        if (Input.GetButton("Fire2"))
        {
            zoomAnim.SetTrigger("ZoomOut");
            if (activeWep == 3)
                scopeAnim.SetTrigger("UnScope");
        }
        SwitchWeaponServerRpc(wep);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchWeaponServerRpc(int wep)
    {
        SwitchWeaponClientRpc(wep);
    }

    [ClientRpc]
    public void SwitchWeaponClientRpc(int wep)
    {
        StartCoroutine(SwitchToWeapon(wep));
    }

    private IEnumerator SwitchToWeapon(int wepIndex)
    {
        isSwitching = true;
        DeselectAllWeapons();
        yield return new WaitForSeconds(0.5f);
        DeselectAllWeapons();
        activeWep = wepIndex;
        weps[wepIndex].GetComponent<Renderer>().enabled = true;
        weps[wepIndex].transform.GetChild(0).GetComponent<Renderer>().enabled = true;
        weps[wepIndex].GetComponent<RaycastShoot>().enabled = true;
        isSwitching = false;
    }

    private void DeselectAllWeapons()
    {
        foreach (GameObject wep in weps)
        {
            wep.GetComponent<Renderer>().enabled = false;
            wep.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            wep.GetComponent<RaycastShoot>().enabled = false;
        }
    }
}
