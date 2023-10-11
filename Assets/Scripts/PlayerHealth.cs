using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public AudioSource hitmarkerAudio1;
    public AudioSource hitmarkerAudio2;
    public AudioSource healSound;
    public GameObject playerBody;
    public GameObject playerBodyFirstPerson;
    public GameObject playerHead;
    public DisplayHealth displayHealth;
    public Transform spawnList;

    private Player player;

    public void Start()
    {
        player = transform.root.GetComponent<Player>();

        if (IsLocalPlayer)
        {
            playerBody.SetActive(false);
            playerBodyFirstPerson.SetActive(true);
            playerHead.GetComponent<MeshCollider>().enabled = false;
        }
    }

    public void playHealSound()
    {
        if (IsLocalPlayer)
            healSound.Play();
    }

    public void Damage(float damageAmt)
    {
        StartCoroutine(DamageFlash(damageAmt >= player.GetHealth()));
        int randSpawnNum = Random.Range(0, spawnList.childCount);
        Vector3 respawnLoc = spawnList.GetChild(randSpawnNum).position;
        DamageServerRpc(damageAmt, respawnLoc);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(float damageAmt, Vector3 respawnLoc)
    {
        DamageClientRpc(damageAmt, respawnLoc);
    }

    [ClientRpc]
    public void DamageClientRpc(float damageAmt, Vector3 respawnLoc)
    {
        if (damageAmt >= player.GetHealth())
        {
            List<Renderer> renderers = new List<Renderer>();
            foreach (Renderer rs in transform.GetComponentsInChildren<Renderer>())
            {
                if (rs.enabled)
                {
                    renderers.Add(rs);
                    rs.enabled = false;
                }
            }
            transform.GetComponent<CharacterController>().enabled = false;            
            transform.position = respawnLoc;
            transform.LookAt(spawnList);
            transform.GetComponent<CharacterController>().enabled = true;
            StartCoroutine(ResetHealth(renderers));
        }
        else
            player.SetHealth(player.GetHealth() - damageAmt);
    }


    IEnumerator ResetHealth(List<Renderer> renderers)
    {
        player.SetHealth(player.GetMaxHealth());
        player.SetDeaths(player.GetDeaths() + 1);
        displayHealth.FlashDamageIndicator(true);

        yield return new WaitForSeconds(0.5f);

        player.SetHealth(player.GetMaxHealth());

        foreach (Renderer rs in renderers)
            rs.enabled = true;

        RaycastShoot[] weps = player.GetItemInHand().transform.parent.GetComponentsInChildren<RaycastShoot>();
        weps[0].ammo = weps[0].maxAmmo;
        for(int i = 1; i < weps.Length; i++)       
            weps[i].ammo = 0;
        weps[4].ammo = weps[4].maxAmmo;

        WeaponSwitcher weaponSwitcher = player.GetComponent<WeaponSwitcher>();
        if(weaponSwitcher.activeWep != 0)
            weaponSwitcher.SwitchWeapon(0);
        weaponSwitcher.hasPickedUp[1] = false;
        weaponSwitcher.hasPickedUp[2] = false;
        weaponSwitcher.hasPickedUp[3] = false;

        foreach (RaycastShoot rcs in player.GetItemInHand().transform.parent.GetComponentsInChildren<RaycastShoot>())      
            if(rcs.recoilAnim != null)
                rcs.recoilAnim.SetTrigger("SlideForward");     
    }


    IEnumerator DamageFlash(bool killShot)
    {
        yield return new WaitForSeconds(0.03f);
        if(killShot)
        {
            hitmarkerAudio1.volume = 0.5f;
            hitmarkerAudio2.volume = 1f;
        }
        else
        {
            hitmarkerAudio1.volume = 0.125f;
            hitmarkerAudio2.volume = 0.25f;
        }
        hitmarkerAudio1.Play();
        hitmarkerAudio2.Play();
        yield return new WaitForSeconds(0.02f);
        StopCoroutine("DamageFlash");
    }
}
