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
    public float maxInnacuracyMultiplier = 3f;
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

    private Player player;
    private GameObject muzzleFlash;
    private LineRenderer laserLine;
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

        if (!PauseMenu.GameIsPaused && (Input.GetButtonDown("Fire1") || (fullAuto && Input.GetButton("Fire1") && ammo > 0)) && Time.time > nextFire /*&& ammo > 0*/ && !isReloading)
        {
            if(ammo < 1)
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

            float innacuracyMultiplier = -1f;

          
            if(numShotsInBurst <= 2)
            {
                innacuracyMultiplier = 0;
            }
            else if(numShotsInBurst > 2 && numShotsInBurst <= 5)
            {
                innacuracyMultiplier = maxInnacuracyMultiplier / 3;
            }
            else if (numShotsInBurst > 5 && numShotsInBurst <= 10)
            {
                innacuracyMultiplier = maxInnacuracyMultiplier / 2;
            }
            else if (numShotsInBurst > 10)
            {
                innacuracyMultiplier = maxInnacuracyMultiplier;
            }

            float randX = Random.Range(0.01f * innacuracyMultiplier, -0.01f * innacuracyMultiplier);
            float randY = Random.Range(0.02f * innacuracyMultiplier, -0.01f * innacuracyMultiplier);
            DirectionRay = playerCam.transform.TransformDirection(randX, randY, 1);

            if (Physics.Raycast(rayOrigin, DirectionRay, out hit, float.MaxValue, ~Physics.IgnoreRaycastLayer))
            {
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
                          
                        StartCoroutine(DropWeapon(damagedPlayer.GetComponent<WeaponSwitcher>().activeWep, deathPos));
                    }
                    else
                        hitmarkerAnim.SetTrigger("FlashDamage");
                    damagedPlayer.GetComponent<PlayerHealth>().Damage(dmg);       
                }
                else if(bulletHole != null)
                {
                    GameObject decal = Instantiate(bulletHole, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                    bulletHole.transform.up = hit.normal;
                    Destroy(decal, 5);
                }
            }    
            timeOfLastShot = Time.time;
        }

        if (laserLine != null && (Time.time > nextFire || fireRate < 0.1f))
        {
            Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
            RaycastHit hit;
            laserLine.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(rayOrigin, playerCam.transform.forward, out hit, float.MaxValue, ~Physics.IgnoreRaycastLayer))
                laserLine.SetPosition(1, hit.point);
            else
                laserLine.SetPosition(1, rayOrigin + (playerCam.transform.forward * 1000));

        }

        if(!PauseMenu.GameIsPaused && canReloadWithR && Input.GetKeyDown(KeyCode.R) && !isReloading && (ammo < maxAmmo))
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator DropWeapon(int wepDropIndex, Vector3 pos)
    {
        yield return new WaitForSeconds(0.1f);
        if (wepDropIndex == 1)
        {
            GameObject wepDrop = Instantiate(lgDropPrefab, pos, Quaternion.identity);
            Destroy(wepDrop, 10);
        }
        else if (wepDropIndex == 2)
        {
            GameObject wepDrop = Instantiate(railDropPrefab, pos, Quaternion.identity);
            Destroy(wepDrop, 10);
        }
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();

        if (laserLine != null)
            laserLine.enabled = true;
        if (muzzleFlash != null)
            muzzleFlash.SetActive(true);

        recoilAnim.SetTrigger("SlideBack");
        if (ammo > 0)
            recoilAnim.SetTrigger("SlideForward");

        yield return shotDuration;

        if (laserLine != null)
            laserLine.enabled = false;
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
                recoilAnim.SetTrigger("SlideBack");
            yield return new WaitForSeconds(0.5f);
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
}
