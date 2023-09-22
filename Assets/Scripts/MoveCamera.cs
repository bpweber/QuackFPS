using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Vector3 ogCamPos;
    public Transform camPos;
    public float dist = 1;
    public float moveSecs = 2;
    private float lastChange = 0;
    Vector3 newPos;

    void Start()
    {
        lastChange = Time.time;
        ogCamPos = transform.position;
    }

    void Update()
    {
        if(Time.time > (lastChange + moveSecs))
        {
            float randX = Random.Range(-10f, 10f);
            float randY = Random.Range(-10f, 10f);
            float randZ = Random.Range(-10f, 10f);
            newPos = new Vector3(ogCamPos.x + randX, ogCamPos.y + randY, ogCamPos.z + randZ);
            lastChange = Time.time;
        }
        camPos.position = Vector3.MoveTowards(camPos.position, newPos, dist * Time.deltaTime);
    }
}
