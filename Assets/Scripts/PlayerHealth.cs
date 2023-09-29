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

    private Player player;
    private Color originalColorHead;
    private Color originalColorBody;

    public void Start()
    {
        player = transform.root.GetComponent<Player>();

        originalColorHead = transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
        originalColorBody = transform.GetChild(1).GetComponent<Renderer>().material.color;

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
        StartCoroutine("DamageFlash");
        Vector3 respawnLoc = new Vector3(Random.Range(-10f, 10f), 1, Random.Range(-10f, 10f));
        DamageServerRpc(damageAmt, respawnLoc);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(float damageAmt, Vector3 respawnLoc)
    {
        if (!IsServer)
        {
            if (damageAmt >= player.GetHealth())
            {
                transform.GetComponent<CharacterController>().enabled = false;
                transform.position = respawnLoc;
                transform.GetComponent<CharacterController>().enabled = true;
                StartCoroutine(ResetHealth());
            }
            else
                player.SetHealth(player.GetHealth() - damageAmt);
        }
        DamageClientRpc(damageAmt, respawnLoc);
    }

    [ClientRpc]
    public void DamageClientRpc(float damageAmt, Vector3 respawnLoc)
    {
        if (damageAmt >= player.GetHealth())
        {
            transform.GetComponent<CharacterController>().enabled = false;
            transform.position = respawnLoc;
            transform.GetComponent<CharacterController>().enabled = true;
            StartCoroutine(ResetHealth());
        }
        else
            player.SetHealth(player.GetHealth() - damageAmt);
    }


    IEnumerator ResetHealth()
    {
        player.SetHealth(player.GetMaxHealth());
        player.SetDeaths(player.GetDeaths() + 1);
        displayHealth.FlashDamageIndicator();
        yield return new WaitForSeconds(0.1f);
        player.SetHealth(player.GetMaxHealth());
        foreach (RaycastShoot rcs in player.GetItemInHand().transform.parent.GetComponentsInChildren<RaycastShoot>())
        {
            rcs.recoilAnim.SetTrigger("SlideForward");
            rcs.ammo = rcs.maxAmmo;
        }

    }


    IEnumerator DamageFlash()
    {
        //transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.red;
        //transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red;
        //transform.GetChild(2).GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.03f);
        hitmarkerAudio1.volume = 0.125f;
        hitmarkerAudio2.volume = 0.25f;
        hitmarkerAudio1.Play();
        hitmarkerAudio2.Play();
        yield return new WaitForSeconds(0.02f);
        //transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = originalColorHead;
        //transform.GetChild(1).GetComponent<Renderer>().material.color = originalColorBody;
        //transform.GetChild(2).GetComponent<Renderer>().material.color = originalColorHead;
        StopCoroutine("DamageFlash");
    }
}
