using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : NetworkBehaviour//MonoBehaviour
{
    public Image serverPanel;

    void Start()
    {
        //serverPanel = GetComponent<Image>();
        if(IsServer)
            serverPanel.enabled = true;
        else
            serverPanel.enabled = false;  

    }

    private void Update()
    {
        if (IsClient)        
            serverPanel.enabled = false;
        else if (IsServer)
            serverPanel.enabled = true;

    }
}
