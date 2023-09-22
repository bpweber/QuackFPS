using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{

    public GameObject enemyPrefab;

    public float currentHealth = 100f;
    private Color originalColor;
    public AudioSource hitmarkerAudio1;
    public AudioSource hitmarkerAudio2;



    public void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        //hitmarkerAudio1 = GetComponent<AudioSource>();
       // hitmarkerAudio2 = GetComponent<AudioSource>();
    }

    public void Damage(float damageAmt)
    {
        currentHealth -= damageAmt;
        StartCoroutine("DamageFlash");
        
    }

    IEnumerator DamageFlash()
    {

        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
            r.material.color = Color.red;
        yield return new WaitForSeconds(0.03f);
        if (currentHealth <= 0)
        {
            hitmarkerAudio1.volume = 0.33f;
            hitmarkerAudio2.volume = 0.5f;
        }
        else 
        {
            hitmarkerAudio1.volume = 0.165f;
            hitmarkerAudio2.volume = 0.25f;
        }
        hitmarkerAudio1.Play();
        hitmarkerAudio2.Play();
        yield return new WaitForSeconds(0.02f);
        if (currentHealth <= 0)
        {
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            gameObject.GetComponent<EnemyMovement>().enabled = false;
            yield return new WaitForSeconds(1f);

            enemyPrefab.GetComponent<EntityHealth>().currentHealth = 100f;
            enemyPrefab.GetComponent<EnemyMovement>().enabled = true;
            Rigidbody rb = enemyPrefab.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            Instantiate(enemyPrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2f, gameObject.transform.position.z), Quaternion.identity);

            gameObject.SetActive(false);

        }
        foreach (Renderer r in rs)
            r.material.color = originalColor;
        StopCoroutine("DamageFlash");
    }
}
