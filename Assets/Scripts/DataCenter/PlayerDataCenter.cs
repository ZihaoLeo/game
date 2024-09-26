using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class PlayerDataCenter : Singleton<PlayerDataCenter>
{
    private PlayerData playerData;

    /// <summary>
    /// Initializes weapon data Initializes player data
    /// </summary>
    public void InitPlayerData(PlayerData data)
    {
        if (playerData == null)
        {
            playerData = data;
            // Debug.LogError("playerId:" + data.playerId + "   playerName:" + data.playerName);
        }
    }

    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public PlayerData CopyPlayerData(PlayerType type)
    {
        PlayerData data = new PlayerData();
        data.playerId = playerData.playerId;
        data.playerName = playerData.playerName;
        data.playerType = type;
        return data;
    }

    public bool CheckPlayerId(string id)
    {
        return playerData.playerId == id;
    }

    public void SetPlayerTypeData(PlayerType type)
    {
        playerData.playerType = type;
        // Debug.LogError("playerType:" + playerData.playerType);
    }

    public PlayerType GetPlayerType()
    {
        return playerData.playerType;
    }

    public string GetPlayerId()
    {
        return playerData.playerId;
    }
    public string GetPlayerName()
    {
        return playerData.playerName;
    }
    public void ChangePlayerName(string name)
    {
        playerData.playerName = name;
        SocketTcpClientManager.Instance.SendMessage(MessageType.SyncPlayerNameData, playerData);
    }

}
