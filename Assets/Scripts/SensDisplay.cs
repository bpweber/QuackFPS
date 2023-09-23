using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class SensDisplay : MonoBehaviour
{

    public TMP_Text sensText;

    void Start()
    {
        sensText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        sensText.SetText($"{Math.Round(MouseLook.mouseSensitivity, 2)}");
    }
}
