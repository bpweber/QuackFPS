using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System;

public class DisplayKDA : NetworkBehaviour
{
    private Player player;
    private TMP_Text kdaText;
    private int totalKills;
    private int deaths;
    private double kdr = 0;

    void Start()
    {
        kdaText = GetComponent<TMP_Text>();
        if (IsOwner)
            player = transform.root.GetComponent<Player>();
        if (!IsOwner)
            kdaText.enabled = false;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        if (Input.GetKey(KeyCode.Tab))
        {
            kdaText.enabled = true;
            totalKills = player.GetKills();
            deaths = player.GetDeaths();
            if (deaths == 0)
                kdr = totalKills;
            //else if (totalKills == 0)
            //    kdr = -deaths;
            else
                kdr = Math.Round(((double)totalKills / (double)deaths), 2);

            kdaText.SetText($"K\tD\tK/D\n" +
                            $"{totalKills}\t{deaths}\t{kdr}");
        }
        else
        {
            kdaText.enabled = false;
        }
    }
}
