using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ServerDataCenter : Singleton<ServerDataCenter>
{
    private PlayerDataList playerDataList = new PlayerDataList();// Initializing Weapon data Initializing Player data Stores all player data
    private PlayerDataList roomPlayerDataList = new PlayerDataList();// Store all player data in the room
    public PlayerDataList GetAllPlayerData()
    {
        return playerDataList;
    }

    /// <summary>
    /// Add player data
    /// </summary>
    /// <param name="data"></param>
    public void AddPlayerData(PlayerData data)
    {
        foreach (var item in playerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
                return;
        }
        playerDataList.playerDatas.Add(data);
    }

    /// <summary>
    /// Get player data
    /// </summary>
    /// <param name="data"></param>
    public PlayerData GetPlayerData(PlayerData data)
    {
        foreach (var item in playerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
                return item;
        }
        return null;
    }

    /// <summary>
    /// Check that the room exists
    /// </summary>
    /// <returns></returns>
    public bool CheckRoomExists()
    {
        return roomPlayerDataList.playerDatas.Count > 0;
    }

    /// <summary>
    /// Player joins room
    /// </summary>
    /// <returns></returns>
    public void AddRoomPlayer(PlayerData data)
    {
        PlayerType type = PlayerType.None;
        switch (roomPlayerDataList.playerDatas.Count)
        {
            case 0:
                type = PlayerType.Captain;
                break;
            case 1:
                type = PlayerType.Aviator;
                break;
            case 2:
                type = PlayerType.Engineer;
                break;
            case 3:
                type = PlayerType.Gunner;
                break;
            default:
                break;
        }
        SetPlayerTypeData(data, type);
    }

    /// <summary>
    /// Set player types
    /// </summary>
    /// <param name="data"></param>
    public void SetPlayerTypeData(PlayerData data, PlayerType type)
    {
        foreach (var item in playerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
            {
                item.playerType = type;
                break;
            }
        }
        foreach (var item in roomPlayerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
            {
                item.playerType = data.playerType;
                break;
            }
        }
    }

    /// <summary>
    /// Change player stats
    /// </summary>
    /// <param name="data"></param>
    public void ChangePlayerData(PlayerData data)
    {
        foreach (var item in playerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
            {
                item.playerName = data.playerName;
                break;
            }
        }

        foreach (var item in roomPlayerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
            {
                item.playerName = data.playerName;
                break;
            }
        }
    }

    /// <summary>
    /// Add player data in the room
    /// </summary>
    /// <param name="data"></param>
    public void AddRoomPlayerData(PlayerData data)
    {
        foreach (var item in roomPlayerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
                return;
        }
        roomPlayerDataList.playerDatas.Add(data);
    }

    /// <summary>
    /// Exit the room
    /// </summary>
    /// <returns></returns>
    public bool QuitRoom(PlayerData data)
    {
        foreach (var item in roomPlayerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
            {
                if (item.playerType == PlayerType.Captain)
                {
                    if (roomPlayerDataList.playerDatas.Count > 1)
                        return false;
                    else
                    {
                        RemoveRoomPlayerData(data);
                        return true;
                    }
                }
                else
                {
                    RemoveRoomPlayerData(data);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Delete player data in the room
    /// </summary>
    /// <param name="data"></param>
    public void RemoveRoomPlayerData(PlayerData data)
    {
        foreach (var item in roomPlayerDataList.playerDatas)
        {
            if (item.playerId == data.playerId)
            {
                roomPlayerDataList.playerDatas.Remove(item);
                break;
            }
        }
        SetPlayerTypeData(data, PlayerType.None);
    }

    public PlayerDataList GetRoomAllPlayerData()
    {
        return roomPlayerDataList;
    }

    /// <summary>
    /// Whether it's okay to fire
    /// </summary>
    /// <returns></returns>
    public bool IsShoot(WeaponData data)
    {
        foreach (var item in roomPlayerDataList.playerDatas)
        {
            if (item.playerType == PlayerType.Captain || item.playerType == PlayerType.Gunner)
                return true;
        }
        return false;
    }
}
