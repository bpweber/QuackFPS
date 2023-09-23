using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DisplayFramerate : NetworkBehaviour//MonoBehaviour
{

    public float updateInterval = 0.1f;
    private float nextUpdate = 0f;
    private TMP_Text fpsText;

    void Start()
    {
        fpsText = GetComponent<TMP_Text>();
        if(!IsOwner)
            fpsText.enabled = false;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        if(Time.time > nextUpdate)
        {
            fpsText.SetText($"{(int)(1 / Time.deltaTime)} fps");
            nextUpdate = Time.time + updateInterval;
        }
    }
}
