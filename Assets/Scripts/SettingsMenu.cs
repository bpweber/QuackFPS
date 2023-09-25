using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using TMPro;
using System;
using Unity.Netcode;

public class SettingsMenu : MonoBehaviour
{
    public static int targetFrameRate = -1;
    public static float volume = 50f;

    public TMP_Text volumeText;
    public TMP_Text sensText;
    public TMP_Text fpsText;


    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        AudioListener.volume = volume / 100;
    }

    private void Update()
    {
        volumeText.SetText($"{volume}");
        sensText.SetText($"{Math.Round(MouseLook.mouseSensitivity, 2)}");
        if(targetFrameRate == -1)
            fpsText.SetText("Inf");
        else
            fpsText.SetText($"{targetFrameRate}");
    }

    public void SetSens(float sens)
    {
        MouseLook.mouseSensitivity = sens;
    }

    public void SetTargetFramerate(float newTargetFrameRate)
    {

        targetFrameRate = (int) newTargetFrameRate > 999 ? -1 : (int) newTargetFrameRate;

        //targetFrameRate = (int) newTargetFrameRate;
        //if (targetFrameRate > 999)
        //    targetFrameRate = -1;
        Application.targetFrameRate = targetFrameRate;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        AudioListener.volume = volume / 100;
        Debug.Log(AudioListener.volume);
    }
}
