using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using TMPro;
using System;

public class SettingsMenu : MonoBehaviour
{
    public static int targetFrameRate = 1000;
    public float volume = 5f;

    public TMP_Text volumeText;
    public TMP_Text sensText;
    public TMP_Text fpsText;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    private void Update()
    {
        volumeText.SetText($"{volume}");
        sensText.SetText($"{Math.Round(MouseLook.mouseSensitivity, 2)}");
        fpsText.SetText($"{targetFrameRate}");
    }

    public void SetSens(float sens)
    {
        MouseLook.mouseSensitivity = sens;
    }

    public void SetTargetFramerate(float newTargetFrameRate)
    {
        targetFrameRate = (int) newTargetFrameRate;
        Application.targetFrameRate = targetFrameRate;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        Debug.Log(newVolume);
    }
}
