using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public void SetSens(float sens)
    {
        MouseLook.mouseSensitivity = sens;
    }
}
