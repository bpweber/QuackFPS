using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : NetworkBehaviour
{
    public Image serverPanel;

    void Start()
    {
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
