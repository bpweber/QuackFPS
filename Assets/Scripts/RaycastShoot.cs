using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

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

    private LineRenderer laserLine;
    private WaitForSeconds shotDuration = new WaitForSeconds(.05f);
    private float nextFire;
    private float timeOfLastShot = 0;
    private int numShotsInBurst = 1;
    private Vector3 slidePos;
    private bool isReloading = false;

    void Start()
    {
        if(GetComponent<LineRenderer>() != null)
            laserLine = GetComponent<LineRenderer>();
        slidePos = transform.GetChild(0).localPosition;
    }
    
    void Update()
    {
        if (!IsLocalPlayer) return;

        if (!PauseMenu.GameIsPaused && ((!fullAuto && Input.GetButtonDown("Fire1")) || (fullAuto && Input.GetButton("Fire1"))) && Time.time > nextFire && ammo > 0 && !isReloading)
        {
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
                PlayerHealth health = hit.collider.GetComponentInParent<PlayerHealth>();

                //Debug.Log(hit.collider);

                bool headShot = hit.collider.tag.Equals("PlayerHead");
                if (health != null)
                {
                    if(headShot)
                        health.Damage(headShotDamage);
                    else
                        health.Damage(gunDamage);                   
                }
                else if(laserLine == null)
                {
                    GameObject decal = Instantiate(bulletHole, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                    bulletHole.transform.up = hit.normal;
                    Destroy(decal, 5);
                }
            }    
            timeOfLastShot = Time.time;
        }    
        
        if (laserLine != null)
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

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        if (laserLine != null)
        {
            laserLine.enabled = true;
            transform.Rotate(new Vector3(0f, 0f, 1.5f));
            transform.position = new Vector3(transform.position.x, transform.position.y + .0125f, transform.position.z);
        }
        else
        {
            transform.Rotate(new Vector3(0f, 0f, 3f));
            transform.position = new Vector3(transform.position.x, transform.position.y + .025f, transform.position.z);
        }
        transform.GetChild(0).localPosition = new Vector3(slidePos.x -25, slidePos.y, slidePos.z);
        yield return shotDuration;
        transform.GetChild(0).localPosition = slidePos;
        if (laserLine != null)
        {
            transform.Rotate(new Vector3(0f, 0f, -1.5f));
            transform.position = new Vector3(transform.position.x, transform.position.y - .0125f, transform.position.z);
            laserLine.enabled = false;
        }
        else
        {
            transform.Rotate(new Vector3(0f, 0f, -3f));
            transform.position = new Vector3(transform.position.x, transform.position.y - .025f, transform.position.z);
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        reloadSound.Play();
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(0).localPosition = new Vector3(slidePos.x - 25, slidePos.y, slidePos.z);
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(0).localPosition = slidePos;
        yield return new WaitForSeconds(0.3f);
        ammo = maxAmmo;
        isReloading = false;
    }
}
