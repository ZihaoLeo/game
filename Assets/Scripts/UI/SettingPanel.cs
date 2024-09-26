using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum DifficultyType
{
    Simpleness = 0, // simpleness
    Common = 1, // common
    Difficulty = 2, // difficulty
}

public class SettingPanel : UIBase
{
    public Slider slider;
    private DifficultyType type;

    protected override void OnShow(params object[] parameters)
    {
        base.OnShow();
        type = DifficultyType.Simpleness;
        SetToggleType();
        slider.value = AudioManager.Instance.audioSource.volume;
        slider.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnValueChanged(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }
    private void OnToggleValueChanged(bool isOn)
    {
        if (type == DifficultyType.Simpleness)
        {
            return;
        }
        if (isOn) // Change the difficulty type only if Toggle is selected
        {
            type = DifficultyType.Simpleness;
            SetToggleType();
        }
    }
    private void OnToggleValueChanged1(bool isOn)
    {
        if (type == DifficultyType.Common)
        {
            return;
        }
        if (isOn)
        {
            type = DifficultyType.Common;
            SetToggleType();
        }
    }
    private void OnToggleValueChanged2(bool isOn)
    {
        if (type == DifficultyType.Difficulty)
        {
            return;
        }
        if (isOn)
        {
            type = DifficultyType.Difficulty;
            SetToggleType();
        }
    }
    private void SetToggleType()
    {
        PlayerManager.Instance.difficultyType = type;
        Debug.LogError("type:" + type);
    }
    protected override void OnHide()
    {
        base.OnHide();
        // Here you can do panel-specific logic, such as saving Settings
        Debug.Log("MainPanel Panel Hidden.");
    }

    public void Difficulty()
    {
        Debug.LogError("Difficulty");
    }

    public void OnClickClose()
    {
        UIManager.Instance.OpenUIPanel<StartPanel>(StartManager.Instance.panelParent, WindowName.StartPanel);
    }
}
