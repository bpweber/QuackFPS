using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;

public class WeaponSwitcher : NetworkBehaviour
{
    public int activeWep = 0;

    public GameObject G19;
    public GameObject LG;
    public GameObject Rail;

    private GameObject[] weps = new GameObject[3];
    private bool isSwitching = false;

    private void Start()
    {
        weps[0] = G19;
        weps[1] = LG;
        weps[2] = Rail;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))       
            if (!isSwitching)
                SwitchWeapon(0);       
        if (Input.GetKeyDown(KeyCode.Alpha2))
            if (!isSwitching)
                SwitchWeapon(1); 
        if (Input.GetKeyDown(KeyCode.Alpha3))  
            if (!isSwitching)
                SwitchWeapon(2);       
    }

    public void SwitchWeapon(int wep)
    {
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
        if (!weps[wepIndex].GetComponent<Renderer>().enabled)
        {
            isSwitching = true;
            DeselectAllWeapons();
            yield return new WaitForSeconds(0.5f);
            activeWep = wepIndex;
            weps[wepIndex].GetComponent<Renderer>().enabled = true;
            weps[wepIndex].transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            weps[wepIndex].GetComponent<RaycastShoot>().enabled = true;
            isSwitching = false;
        }
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
