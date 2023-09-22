using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;

public class WeaponSwitcher : NetworkBehaviour//MonoBehaviour
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
        {
            if (!isSwitching)
                SwitchWeapon(0);
                //StartCoroutine(SwitchToWeapon(0));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!isSwitching)
                SwitchWeapon(1);
            //StartCoroutine(SwitchToWeapon(1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!isSwitching)
                SwitchWeapon(2);
            //StartCoroutine(SwitchToWeapon(2));
        }
    }


    public void SwitchWeapon(int wep)
    {
        SwitchWeaponServerRpc(wep);
    }
    
    [ClientRpc]
    public void SwitchWeaponClientRpc(int wep)
    {
        StartCoroutine(SwitchToWeapon(wep));
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchWeaponServerRpc(int wep)
    {
        SwitchWeaponClientRpc(wep);
    }
 

    //private void SwitchToWeapon(GameObject wep)
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
        //GameObject[] wepList = { G19, LG, Rail };

        foreach (GameObject wep in weps)
        {
            wep.GetComponent<Renderer>().enabled = false;
            wep.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            wep.GetComponent<RaycastShoot>().enabled = false;
        }
    }
}
