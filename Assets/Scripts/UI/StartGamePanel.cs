using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGamePanel : UIBase
{
    public Text nameText;
    public InputField inputField;
    public Toggle toggle;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Toggle toggle4;
    public Toggle toggle5;
    public Toggle toggle6;
    public Toggle toggle7;
    private StringBuilder nameStr;
    private PlayerType playerType;
    private DifficultyType difficultyType;

    private PlayerDataList playerDataList;
    private PlayerData playerData;
    protected override void OnShow(params object[] parameters)
    {
        base.OnShow();
        if (nameStr == null)
            nameStr = new StringBuilder();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        toggle1.onValueChanged.AddListener(OnToggleValueChanged1);
        toggle2.onValueChanged.AddListener(OnToggleValueChanged2);
        toggle4.onValueChanged.AddListener(OnToggleValueChanged4);
        toggle5.onValueChanged.AddListener(OnToggleValueChanged5);
        toggle6.onValueChanged.AddListener(OnToggleValueChanged6);
        toggle7.onValueChanged.AddListener(OnToggleValueChanged7);
        DynamicDataCenter.AddMessage(EmDataType.EmStartGameDataRequest, OnEmStartGameDataRequest);
        DynamicDataCenter.AddMessage(EmDataType.EmSelectOccupation, OnEmSelectOccupation);
        DynamicDataCenter.AddMessage(EmDataType.EmQuitRoom, OnEmQuitRoom);
        SocketTcpClientManager.Instance.SendMessage(MessageType.GetRoomPlayerData, PlayerDataCenter.Instance.GetPlayerData());
    }

    private void OnEmQuitRoom(object[] paras)
    {
        UIManager.Instance.OpenUIPanel<StartPanel>(StartManager.Instance.panelParent, WindowName.StartPanel);
    }

    private void OnEmSelectOccupation(object[] paras)
    {
        playerData = (PlayerData)paras[0];
        if (PlayerDataCenter.Instance.CheckPlayerId(playerData.playerId))
        {
            PlayerDataCenter.Instance.SetPlayerData(playerData);
        }
    }

    private void OnEmStartGameDataRequest(object[] paras)
    {
        playerDataList = (PlayerDataList)paras[0];
        nameStr.Clear();
        foreach (var item in playerDataList.playerDatas)
        {
            nameStr.Append(item.playerName + "\r\n");
        }
        nameText.text = nameStr.ToString();
        SetToggleState();
    }

    /// <summary>
    /// choose an occupation

    /// </summary>
    /// <param name="isOn"></param>
    #region
    private void OnToggleValueChanged(bool isOn)
    {
        if (playerType == PlayerType.Aviator)
        {
            toggle.isOn = true;
            return;
        }
        if (isOn) // Change the difficulty type only if Toggle is selected
        {
            playerType = PlayerType.Aviator;
            SetToggleType();
        }
    }

    private void OnToggleValueChanged1(bool isOn)
    {
        if (playerType == PlayerType.Engineer)
        {
            toggle1.isOn = true;
            return;
        }
        if (isOn)
        {
            playerType = PlayerType.Engineer;
            SetToggleType();
        }
    }

    private void OnToggleValueChanged2(bool isOn)
    {
        if (playerType == PlayerType.Gunner)
        {
            toggle2.isOn = true;
            return;
        }
        if (isOn)
        {
            playerType = PlayerType.Gunner;
            SetToggleType();
        }
    }

    private void SetToggleType()
    {
        toggle.isOn = playerType == PlayerType.Aviator;
        toggle1.isOn = playerType == PlayerType.Engineer;
        toggle2.isOn = playerType == PlayerType.Gunner;
        Debug.LogError("playerType:" + playerType);
        SocketTcpClientManager.Instance.SendMessage(MessageType.SelectPlayerOccupation, PlayerDataCenter.Instance.CopyPlayerData(playerType));
    }

    private void SetToggleState()
    {
        bool flag = true;
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = false;
        foreach (var item in playerDataList.playerDatas)
        {
            if (!PlayerDataCenter.Instance.CheckPlayerId(item.playerId))
            {
                if (item.playerType == PlayerType.Captain)
                {
                    toggle3.isOn = false;
                }
                else if (item.playerType == PlayerType.Aviator)
                {
                    flag = false;
                    toggle.isOn = false;
                }
                else if (item.playerType == PlayerType.Engineer)
                {
                    flag1 = false;
                    toggle1.isOn = false;
                }
                else if (item.playerType == PlayerType.Gunner)
                {
                    flag2 = false;
                    toggle2.isOn = false;
                }
            }
            else
            {
                playerType = item.playerType;
                if (item.playerType == PlayerType.Captain)
                {
                    toggle3.isOn = true;
                    flag = flag1 = flag2 = false;
                }
                if (item.playerType == PlayerType.Aviator)
                    toggle.isOn = true;
                else if (item.playerType == PlayerType.Engineer)
                    toggle1.isOn = true;
                else if (item.playerType == PlayerType.Gunner)
                    toggle2.isOn = true;
            }
        }
        toggle.enabled = flag;
        toggle1.enabled = flag1;
        toggle2.enabled = flag2;
        toggle3.enabled = flag3;
    }
    #endregion

    /// <summary>
    /// Selection difficulty
    /// </summary>
    #region
    private void OnValueChanged(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }
    private void OnToggleValueChanged4(bool isOn)
    {
        if (difficultyType == DifficultyType.Simpleness)
        {
            toggle4.isOn = true;
            return;
        }
        if (isOn) // Change the difficulty type only if Toggle is selected
        {
            difficultyType = DifficultyType.Simpleness;
            SetToggleType2();
        }
    }
    private void OnToggleValueChanged5(bool isOn)
    {
        if (difficultyType == DifficultyType.Common)
        {
            toggle5.isOn = true;
            return;
        }
        if (isOn)
        {
            difficultyType = DifficultyType.Common;
            SetToggleType2();
        }
    }
    private void OnToggleValueChanged6(bool isOn)
    {
        if (difficultyType == DifficultyType.Difficulty)
        {
            toggle6.isOn = true;
            return;
        }
        if (isOn)
        {
            difficultyType = DifficultyType.Difficulty;
            SetToggleType2();
        }
    }
    private void SetToggleType2()
    {
        toggle4.isOn = difficultyType == DifficultyType.Simpleness;
        toggle5.isOn = difficultyType == DifficultyType.Common;
        toggle6.isOn = difficultyType == DifficultyType.Difficulty;
        PlayerManager.Instance.SetRandoem(difficultyType);
        Debug.LogError("difficultyType:" + difficultyType);
    }
    #endregion

    private void OnToggleValueChanged7(bool isOn)
    {
        PlayerManager.Instance.invincible = isOn;
    }
    protected override void OnHide()
    {
        base.OnHide();
        // Here you can do panel-specific logic, such as saving Settings
        Debug.Log("MainPanel Panel Hidden.");
    }
    public void StartGame()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.StartGame, PlayerManager.Instance.GetSendStartData());
    }
    public void GoBack()
    {
        if (PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain)
            SocketTcpClientManager.Instance.SendMessage(MessageType.QuitRoom, PlayerDataCenter.Instance.GetPlayerData());
        else
            UIManager.Instance.OpenUIPanel<StartPanel>(StartManager.Instance.panelParent, WindowName.StartPanel);
    }
    public void EnterName()
    {
        PlayerDataCenter.Instance.ChangePlayerName(inputField.text);
    }
}
