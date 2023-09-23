using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System;

public class DisplayKDA : NetworkBehaviour//MonoBehaviour
{
    public GameObject wepHolder;
    public PlayerHealth playerHealth;
    private TMP_Text kdaText;
    //private int totalKills;
    private int deaths;
    private double kdr = 0;

    void Start()
    {
        kdaText = GetComponent<TMP_Text>();
        if (!IsOwner)
            kdaText.enabled = false;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        int totalKills = 0;
        foreach (RaycastShoot rcs in wepHolder.GetComponentsInChildren<RaycastShoot>())
            totalKills += rcs.killCount;
        deaths = playerHealth.deathCount;
        if (deaths == 0)
            kdr = totalKills;
        else
            kdr = Math.Round(((double) totalKills / (double) deaths), 2);

        kdaText.SetText($"K\tD\tK/D\n" +
                        $"{totalKills}\t{deaths}\t{kdr}");
    }
}
