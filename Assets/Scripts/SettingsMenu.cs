using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using TMPro;
using System;
using Unity.Netcode;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static int targetFrameRate = -1;
    public static float volume = 50f;

    public TMP_Text volumeText;
    public TMP_Text sensText;
    public TMP_Text zoomSensText;
    public TMP_Text fpsText;

    public Slider volumeSlider;
    public Slider sensSlider;
    public Slider zoomSensSlider;
    public Slider framerateSlider;


    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 100f);
        sensSlider.value = PlayerPrefs.GetFloat("sens", 1.5f);
        zoomSensSlider.value = PlayerPrefs.GetFloat("zoomSens", 1f);
        framerateSlider.value = PlayerPrefs.GetFloat("targetFramerate", 240f);
    }

    private void Update()
    {
        volumeText.SetText($"{volume}");
        sensText.SetText($"{Math.Round(MouseLook.mouseSensitivity, 2)}");
        zoomSensText.SetText($"{Math.Round(MouseLook.zoomSens, 2)}");
        if (targetFrameRate == -1)
            fpsText.SetText("Inf");
        else
            fpsText.SetText($"{targetFrameRate}");
    }

    public void SetSens(float sens)
    {
        PlayerPrefs.SetFloat("sens", sens);
        PlayerPrefs.Save();

        MouseLook.mouseSensitivity = sens;
        MouseLook.effectiveSens = sens;
    }

    public void SetZoomSens(float zoomSens)
    {
        PlayerPrefs.SetFloat("zoomSens", zoomSens);
        PlayerPrefs.Save();

        MouseLook.zoomSens = zoomSens;
    }

    public void SetTargetFramerate(float newTargetFrameRate)
    {
        PlayerPrefs.SetFloat("targetFramerate", newTargetFrameRate);
        PlayerPrefs.Save();

        targetFrameRate = (int) newTargetFrameRate > 999 ? -1 : (int) newTargetFrameRate;

        Application.targetFrameRate = targetFrameRate;
    }

    public void SetVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("volume", newVolume);
        PlayerPrefs.Save();

        volume = newVolume;
        AudioListener.volume = volume / 75;
    }
}
