using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : UIBase
{
    PlayerData playerData;

    protected override void OnShow(params object[] parameters)
    {
        base.OnShow();
        DynamicDataCenter.AddMessage(EmDataType.EmCreateRoom, OnEmCreateRoom);
        DynamicDataCenter.AddMessage(EmDataType.EmAddRoom, OnEmAddRoom);
    }

    protected override void OnHide()
    {
        base.OnHide();
        // Here you can do panel-specific logic, such as saving Settings
        Debug.Log("MainPanel Panel Hidden.");
    }

    private void OnEmCreateRoom(object[] paras)
    {
        playerData = (PlayerData)paras[0];
        if (PlayerDataCenter.Instance.CheckPlayerId(playerData.playerId))
        {
            PlayerDataCenter.Instance.SetPlayerTypeData(playerData.playerType);
            UIManager.Instance.OpenUIPanel<StartGamePanel>(StartManager.Instance.panelParent, WindowName.StartGamePanel);
        }
    }

    private void OnEmAddRoom(object[] paras)
    {
        playerData = (PlayerData)paras[0];
        if (PlayerDataCenter.Instance.CheckPlayerId(playerData.playerId))
        {
            PlayerDataCenter.Instance.SetPlayerTypeData(playerData.playerType);
            UIManager.Instance.OpenUIPanel<StartGamePanel>(StartManager.Instance.panelParent, WindowName.StartGamePanel);
        }
    }

    public void StartGame()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.CreateRoomData, PlayerDataCenter.Instance.GetPlayerData());
    }

    public void JoinGame()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.AddRoomData, PlayerDataCenter.Instance.GetPlayerData());
    }
    public void Settings()
    {
        UIManager.Instance.OpenUIPanel<SettingPanel>(StartManager.Instance.panelParent, WindowName.SettingPanel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
