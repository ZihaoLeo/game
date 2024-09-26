using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SocketTcpClientManager : Singleton<SocketTcpClientManager>
{
    public ClientInfo clientInfo;
    public bool isConnect = false;

    /// <summary>
    /// Generate the location of the planet The client receives the message returned by the server
    /// </summary>
    public void Received()
    {
        while (true)
        {
            try
            {
                // Receive the message length first
                byte[] lengthBytes = new byte[4];
                int bytesReceived = clientInfo.socket.Receive(lengthBytes, 0, 4, SocketFlags.None);
                if (bytesReceived == 0)
                {
                    break;
                }

                int messageLength = BitConverter.ToInt32(lengthBytes, 0);
                byte[] receiveBuffer = new byte[messageLength];

                // Receive data based on message length
                int totalBytesReceived = 0;
                while (totalBytesReceived < messageLength)
                {
                    bytesReceived = clientInfo.socket.Receive(receiveBuffer, totalBytesReceived, messageLength - totalBytesReceived, SocketFlags.None);
                    if (bytesReceived == 0)
                    {
                        break;
                    }
                    totalBytesReceived += bytesReceived;
                }

                // Process the received data
                DeserializeData(receiveBuffer, messageLength);
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving message: " + e.Message + DateTime.Now);
                isConnect = false;
                clientInfo.socket.Close();
                break; // If an exception occurs, the loop exits and the message is received
            }
        }
    }

    /// <summary>
    /// Deserialize different data classes based on different message ids
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="data"></param>
    private void DeserializeData(byte[] byteData, int bytesRead)
    {
        MessageBase messageBase = MessageBase.Deserialize(byteData);
        PlayerData playerData = null;
        Debug.Log("TCPThe client receives the message. Procedure ID: " + messageBase.messageId + ", feedback: " + messageBase.feedback + "   byte:" + bytesRead);
        if (messageBase.feedback == 1000)
        {
            switch (messageBase.messageId)
            {
                case MessageType.CreateClientId:
                    playerData = messageBase.data as PlayerData;
                    PlayerDataCenter.Instance.InitPlayerData(playerData);
                    break;
                case MessageType.CreateRoomData:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        playerData = messageBase.data as PlayerData;
                        DynamicDataCenter.SendMessage(EmDataType.EmCreateRoom, playerData);
                    });
                    break;
                case MessageType.AddRoomData:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        playerData = messageBase.data as PlayerData;
                        DynamicDataCenter.SendMessage(EmDataType.EmAddRoom, playerData);
                    });
                    break;
                case MessageType.GetRoomPlayerData:
                case MessageType.SyncPlayerNameData:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        PlayerDataList dataList = messageBase.data as PlayerDataList;
                        DynamicDataCenter.SendMessage(EmDataType.EmStartGameDataRequest, dataList);
                    });
                    break;
                case MessageType.SelectPlayerOccupation:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        playerData = messageBase.data as PlayerData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSelectOccupation, playerData);
                    });
                    break;
                case MessageType.QuitRoom:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        playerData = messageBase.data as PlayerData;
                        DynamicDataCenter.SendMessage(EmDataType.EmQuitRoom, playerData);
                    });
                    break;
                case MessageType.StartGame:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        ReceiveStartData data = messageBase.data as ReceiveStartData;
                        DynamicDataCenter.SendMessage(EmDataType.EmStartGame, data);
                    });
                    break;
                case MessageType.SyncShoot:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        WeaponData data = messageBase.data as WeaponData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncShoot, data);
                    });
                    break;
                case MessageType.SyncStarHealth:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        StarData data = messageBase.data as StarData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncStarHealth, data);
                    });
                    break;
                case MessageType.SyncEnemyPlane:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        EnemyPlaneData data = messageBase.data as EnemyPlaneData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncEnemyPlane, data);
                    });
                    break;
                case MessageType.SyncGuidedMissile:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GuidedMissileData data = messageBase.data as GuidedMissileData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncGuidedMissile, data);
                    });
                    break;    
                case MessageType.SyncShipHealth:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncShipHealth);
                    });
                    break;
                case MessageType.SyncIncreaseFitness:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncIncreaseFitness);
                    });
                    break;
                case MessageType.SyncIncreasedWeaponDamage:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncIncreasedWeaponDamage);
                    });
                    break;
                case MessageType.SyncIncreasedWeaponEnchantment:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncIncreasedWeaponEnchantment);
                    });
                    break;
                case MessageType.SyncIncreaseShipSpeed:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncIncreaseShipSpeed);
                    });
                    break;
                case MessageType.SyncSettlementInterface:
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncSettlementInterface);
                    });
                    break;
                case MessageType.ClientOnApplicationQuit:
                    playerData = messageBase.data as PlayerData;
                    if (playerData.playerId == PlayerDataCenter.Instance.GetPlayerId())
                    {
                        SocketTcpManager.Instance.ClientQuit();
                    }
                    break;
                case MessageType.ServerOnApplicationQuit:
                    SocketTcpManager.Instance.ServerQuit();
                    break;
                default:
                    break;
            }
        }
    }

    public bool IsConnect()
    {
        return isConnect && clientInfo.socket != null && clientInfo.socket.Connected && clientInfo.state;
    }
    // 向服务器发送消息
    public void SendMessage(MessageType id, object data = null)
    {
        try
        {
            if (!IsConnect())
            {
                Debug.LogError("Unable to send message, socket closed or not connected" + DateTime.Now);
                return;
            }
            MessageBase dataClass = new MessageBase();
            dataClass.messageId = id;
            dataClass.data = data;
            // 发送数据

            byte[] sendData = SocketTcpManager.Instance.SerializeData(dataClass);
            //Debug.Log("客户端发送数据给服务器" + dataClass.messageId + "  字节长度:" + sendData.Length);
            clientInfo.socket.Send(sendData);
        }
        catch (Exception e)
        {
            isConnect = false;
            Debug.LogError("TCPError when client sends message to server: " + e.Message + DateTime.Now);
            clientInfo.socket.Close();
        }
    }
}
