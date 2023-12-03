using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ShotgunShoot : NetworkBehaviour
{
    public float gunDamage = 6f;
    public float headShotDamage = 12f; 
    public float fireRate = .5f;
    public float range = float.MaxValue;
    public float maxSpread = 3f;
    public int numPelletsPerShot = 12;
    //public float maxInnacuracyMultiplier = 3f;
    //public bool isAccurateWhileJumping = true;
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
    public GameObject lineRendererParent;   
    public LineRenderer[] laserLines;

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
        if (lineRendererParent != null )
            laserLines = lineRendererParent.GetComponentsInChildren<LineRenderer>();
        //if (GetComponent<LineRenderer>() != null)
            //laserLine = GetComponent<LineRenderer>();
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
            RaycastHit hit = new RaycastHit();
            Dictionary<Player, float> damageMap = new Dictionary<Player, float>();

            Vector3[] DirectionRay = new Vector3[numPelletsPerShot];


            for (int i = 0; i < numPelletsPerShot; i++)
            {
                float randX = Random.Range(0.01f * maxSpread, -0.01f * maxSpread);
                float randY = Random.Range(0.01f * maxSpread, -0.01f * maxSpread);
                DirectionRay[i] = playerCam.transform.TransformDirection(randX, randY, 1);
            


                if (laserLines != null)
                    RenderShotTrail(true, 0, gunEnd.position.x, gunEnd.position.y, gunEnd.position.z, i);


                if (Physics.Raycast(rayOrigin, DirectionRay[i], out hit, range, ~Physics.IgnoreRaycastLayer))
                {
                    if (laserLines != null)
                        RenderShotTrail(false, 1, hit.point.x, hit.point.y, hit.point.z, i);

                    Player damagedPlayer = hit.collider.GetComponentInParent<Player>();

                    bool headShot = hit.collider.tag.Equals("PlayerHead");
                    if (damagedPlayer != null)
                    {
                        float dmg = headShot ? headShotDamage : gunDamage;

                        if (damageMap.ContainsKey(damagedPlayer))
                        {
                            damageMap[damagedPlayer] += dmg;
                        }
                        else
                        {
                            damageMap.Add(damagedPlayer, dmg);
                        }


                        
                    }
                    else if (bulletHole != null)
                    {
                        GameObject decal = Instantiate(bulletHole, hit.point + hit.normal * .0001f, Quaternion.LookRotation(hit.normal));
                        bulletHole.transform.up = hit.normal;
                        Destroy(decal, 5);
                    }
                }
                else if (laserLines != null)
                {
                    Vector3 endpos = rayOrigin + (DirectionRay[i] * range);
                    RenderShotTrail(false, 1, endpos.x, endpos.y, endpos.z, i);
                }
            }
        
            foreach(Player playerToDamage in damageMap.Keys ) {
                DoQueuedDamageToPlayer(damageMap[playerToDamage], playerToDamage, hit);
            }

            timeOfLastShot = Time.time;
        }

        if(!PauseMenu.GameIsPaused && canReloadWithR && Input.GetKeyDown(KeyCode.R) && !isReloading && (ammo < maxAmmo))
        {
            StartCoroutine(Reload());
        }
    }

    private void DoQueuedDamageToPlayer(float dmg, Player damagedPlayer, RaycastHit hit)
    {
        if (dmg >= damagedPlayer.GetHealth())
        {
            Debug.Log("dmg > health");
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
            Debug.Log("else");
            hitmarkerAnim.SetTrigger("FlashDamage");
        }
        damagedPlayer.GetComponent<PlayerHealth>().Damage(dmg);
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

    public void RenderShotTrail(bool enable, int posIndex, float x, float y, float z, int lineIndex)
    {
        if(IsOwner)
        {
            if (enable)
                StartCoroutine(EnableAndDisableTrail());
            if (laserLines != null)
                //foreach (LineRenderer laserLine in laserLines)
                laserLines[lineIndex].SetPosition(posIndex, new Vector3(x, y, z));
        }
        RenderShotTrailServerRpc(enable, posIndex, x, y, z, lineIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RenderShotTrailServerRpc(bool enable, int posIndex, float x, float y, float z, int lineIndex)
    {
        RenderShotTrailClientRpc(enable, posIndex, x, y, z, lineIndex);
    }

    [ClientRpc]
    public void RenderShotTrailClientRpc(bool enable, int posIndex, float x, float y, float z, int lineIndex)
    {
        if(!IsOwner)
        {
            if (enable)
                StartCoroutine(EnableAndDisableTrail());
            if (laserLines != null)
                //foreach(LineRenderer laserLine in laserLines)
                laserLines[lineIndex].SetPosition(posIndex, new Vector3(x, y, z));
        }
    }

    private IEnumerator EnableAndDisableTrail()
    {
        if (laserLines != null)
            foreach (LineRenderer laserLine in laserLines)
                laserLine.enabled = true;
        yield return shotDuration;
        if (laserLines != null)
            foreach (LineRenderer laserLine in laserLines)
                laserLine.enabled = false;
    }

}
