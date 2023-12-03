using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RaycastShoot : NetworkBehaviour
{
    public float gunDamage = 10f;
    public float headShotDamage = 20f; 
    public float fireRate = .1f;
    public float range = float.MaxValue;
    public float maxInnacuracyMultiplier = 3f;
    public bool isAccurateWhileJumping = true;
    public bool fullAuto = false;
    public int ammo = 100;
    public int maxAmmo = 100;
    public bool canReloadWithR = false;
    public Transform gunEnd;
    public GameObject bulletHole;
    public Camera playerCam;
    public AudioSource gunAudio;
    public AudioSource reloadSound;
    public AudioSource dryFire;
    public Animator recoilAnim;
    public Animator hitmarkerAnim;
    public Animator killmarkerAnim;
    public GameObject lgDropPrefab;
    public GameObject railDropPrefab;
    public GameObject deadPlayerPrefab;
    public LineRenderer laserLine;

    private Player player;
    private GameObject muzzleFlash;
    private WaitForSeconds shotDuration = new WaitForSeconds(.05f);
    private float nextFire;
    private float timeOfLastShot = 0;
    private int numShotsInBurst = 1;
    private bool isReloading = false;

    void Start()
    {
        if (IsOwner)
            player = transform.root.GetComponent<Player>();
        if (GetComponent<LineRenderer>() != null)
            laserLine = GetComponent<LineRenderer>();
        recoilAnim = GetComponent<Animator>();
        if (gunEnd.transform.childCount > 0)
            muzzleFlash = gunEnd.transform.GetChild(0).gameObject;
        if (fireRate >= 0.5)
            shotDuration = new WaitForSeconds(fireRate);
    }
    
    void Update()
    {
        if (!IsLocalPlayer) return;

        if (!PauseMenu.GameIsPaused && (Input.GetButtonDown("Fire1") || (fullAuto && Input.GetButton("Fire1") && ammo > 0)) && Time.time > nextFire && !isReloading)
        {
            if (ammo < 1)
            {
                dryFire.Play();
                return;
            }

            if ((Time.time - timeOfLastShot) < 0.25)
                numShotsInBurst++;
            else
                numShotsInBurst = 1;

            ammo--;

            nextFire = Time.time + fireRate;

            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
            RaycastHit hit;

            Vector3 DirectionRay;

            float innacuracyMultiplier = 0 ;

            switch (numShotsInBurst)
            {
                case > 10:
                    innacuracyMultiplier = maxInnacuracyMultiplier;
                    break;
                case > 5:
                    innacuracyMultiplier = maxInnacuracyMultiplier / 2;
                    break;
                case > 2:
                    innacuracyMultiplier = maxInnacuracyMultiplier / 3;
                    break;
                default:
                    innacuracyMultiplier = 0;
                    break;
            }

            if (!player.transform.GetComponent<CharacterController>().isGrounded && !isAccurateWhileJumping)
                innacuracyMultiplier = maxInnacuracyMultiplier;

            float randX = Random.Range(0.01f * innacuracyMultiplier, -0.01f * innacuracyMultiplier);
            float randY = Random.Range(0.02f * innacuracyMultiplier, -0.01f * innacuracyMultiplier);
            DirectionRay = playerCam.transform.TransformDirection(randX, randY, 1);

            if(laserLine != null)           
                RenderShotTrail(true, 0, gunEnd.position.x, gunEnd.position.y, gunEnd.position.z);

            if (Physics.Raycast(rayOrigin, DirectionRay, out hit, range, ~Physics.IgnoreRaycastLayer))
            {
                if(laserLine != null)
                    RenderShotTrail(false, 1, hit.point.x, hit.point.y, hit.point.z);

                Player damagedPlayer = hit.collider.GetComponentInParent<Player>();

                bool headShot = hit.collider.tag.Equals("PlayerHead");
                if (damagedPlayer != null)
                {
                    float dmg = headShot ? headShotDamage : gunDamage;
                    if (dmg >= damagedPlayer.GetHealth())
                    {
                        player.SetKills(player.GetKills() + 1);
                        killmarkerAnim.SetTrigger("FlashDamage");

                        Vector3 deathPos = damagedPlayer.transform.position;
                        Quaternion deathRot = damagedPlayer.transform.rotation;

                        Vector3 deathForce = -hit.normal * 250;

                        StartCoroutine(SpawnRagdoll(deathPos, deathRot, deathForce));

                        StartCoroutine(DropWeapon(damagedPlayer.GetComponent<WeaponSwitcher>().activeWep, deathPos));
                    }
                    else
                    {
                        hitmarkerAnim.SetTrigger("FlashDamage");
                    }
                    damagedPlayer.GetComponent<PlayerHealth>().Damage(dmg);       
                }
                else if(bulletHole != null)
                {
                    GameObject decal = Instantiate(bulletHole, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                    bulletHole.transform.up = hit.normal;
                    Destroy(decal, 5);
                }
            }
            else if (laserLine != null)
            {
                Vector3 endpos = rayOrigin + (DirectionRay * range);
                RenderShotTrail(false, 1, endpos.x, endpos.y, endpos.z);
            }

            timeOfLastShot = Time.time;
        }

        if (laserLine != null && fireRate < 0.1f)
        {
            Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
            RaycastHit hit;
            RenderShotTrail(false, 0, gunEnd.position.x, gunEnd.position.y, gunEnd.position.z);
            if (Physics.Raycast(rayOrigin, playerCam.transform.forward, out hit, range, ~Physics.IgnoreRaycastLayer))
            {
                RenderShotTrail(false, 1, hit.point.x, hit.point.y, hit.point.z);
            }
            else
            {
                Vector3 endpos = rayOrigin + (playerCam.transform.forward * range);
                RenderShotTrail(false,1, endpos.x, endpos.y, endpos.z);
            }
        }

        if(!PauseMenu.GameIsPaused && canReloadWithR && Input.GetKeyDown(KeyCode.R) && !isReloading && (ammo < maxAmmo))
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator SpawnRagdoll(Vector3 pos, Quaternion rot, Vector3 deathForce)
    {
        GameObject deadPlayer = Instantiate(deadPlayerPrefab, pos, rot);
        deadPlayer.GetComponent<Rigidbody>().AddForce(deathForce);
        yield return new WaitForSeconds(0.1f);
        foreach (Collider cs in deadPlayer.GetComponentsInChildren<Collider>())       
            if (!cs.enabled)       
                cs.enabled = true;    
        Destroy(deadPlayer, 10);
    }

    private IEnumerator DropWeapon(int wepDropIndex, Vector3 pos)
    {
        yield return new WaitForSeconds(0.1f);
        if (wepDropIndex == 1)
        {
            GameObject wepDrop = Instantiate(lgDropPrefab, pos, Quaternion.identity);
            Destroy(wepDrop, 10);
        }
        else if (wepDropIndex == 3)
        {
            GameObject wepDrop = Instantiate(railDropPrefab, pos, Quaternion.identity);
            Destroy(wepDrop, 10);
        }
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();

        if (muzzleFlash != null)
            muzzleFlash.SetActive(true);

        if (recoilAnim != null)
        {
            recoilAnim.SetTrigger("SlideBack");
            if (ammo > 0)
                recoilAnim.SetTrigger("SlideForward");
        }

        yield return shotDuration;

        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
    }

    public IEnumerator Reload()
    {
        if (!isReloading) {
            isReloading = true;
            if(IsLocalPlayer)
                reloadSound.Play();

            yield return new WaitForSeconds(0.05f);

            if(ammo > 0)
                if(recoilAnim != null)  
                    recoilAnim.SetTrigger("SlideBack");

            yield return new WaitForSeconds(0.5f);

            if (recoilAnim != null)
                recoilAnim.SetTrigger("SlideForward");

            yield return new WaitForSeconds(0.3f);

            if (ammo < maxAmmo)
                ammo = maxAmmo;
            else if (ammo < maxAmmo * 2)
                ammo += (maxAmmo / 10) > 1 ? (maxAmmo / 10) : 1;

            if (ammo > maxAmmo * 2)
                ammo = maxAmmo * 2;
            isReloading = false;
        }
    }

    public void RenderShotTrail(bool enable, int posIndex, float x, float y, float z)
    {
        if(IsOwner)
        {
            if (enable)
                StartCoroutine(EnableAndDisableTrail());
            if (laserLine != null)
                laserLine.SetPosition(posIndex, new Vector3(x, y, z));
        }
        RenderShotTrailServerRpc(enable, posIndex, x, y, z);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RenderShotTrailServerRpc(bool enable, int posIndex, float x, float y, float z)
    {
        RenderShotTrailClientRpc(enable, posIndex, x, y, z);
    }

    [ClientRpc]
    public void RenderShotTrailClientRpc(bool enable, int posIndex, float x, float y, float z)
    {
        if(!IsOwner)
        {
            if (enable)
                StartCoroutine(EnableAndDisableTrail());
            if (laserLine != null)
                laserLine.SetPosition(posIndex, new Vector3(x, y, z));
        }
    }

    private IEnumerator EnableAndDisableTrail()
    {
        if (laserLine != null)
            laserLine.enabled = true;
        yield return shotDuration;
        if (laserLine != null)
            laserLine.enabled = false;
    }

}
