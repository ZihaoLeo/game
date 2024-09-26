using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketUdpClientManager : Singleton<SocketUdpClientManager>
{
    public Socket udpClient;
    private IPEndPoint serverEndPoint;

    /// <summary>
    /// An error occurred while processing received data sending a datagram to the port of a host with a specific IP address
    /// </summary>
    public void SendMessage(MessageType id, object data)
    {
        if (serverEndPoint == null)
            serverEndPoint = new IPEndPoint(IPAddress.Parse(SocketTcpManager.Instance._ip), SocketUdpManager.Instance.udpServerPort);
        //udpClient.SendTo(Encoding.UTF8.GetBytes(msg), serverEndPoint);

        MessageBase dataClass = new MessageBase();
        dataClass.messageId = id;
        dataClass.data = data;

        byte[] sendData = SocketUdpManager.Instance.SerializeData(dataClass);
        // Debug.Log("UDP The client sends data to the server" + dataClass.messageId + "  Byte length:" + sendData.Length + "  serverEndPoint:" + serverEndPoint.ToString());
        udpClient.SendTo(sendData, serverEndPoint);
    }

    /// <summary>
    /// Receives the datagram sent to the port number corresponding to the local IP address
    /// </summary>
    public void ReceiveMsg()
    {
        while (true)//SocketUdpClient.Instance.isRunning)
        {
            try
            {
                // Used to store the IP address and port number of the sender
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0); 

                byte[] headerBuffer = new byte[8192];
                int headerBytesReceived = udpClient.ReceiveFrom(headerBuffer, ref clientEndPoint);

                // Deserialize the received data
                MessageBase receivedMessage = MessageBase.Deserialize(headerBuffer);

                // Process the received data
                // Debug.Log("UDP The client received the message. Procedure: " + receivedMessage.messageId + ", data: " + receivedMessage.data.ToString() + "   Byte length:" + headerBytesReceived);

                // Process the received data
                DeserializeData(headerBuffer, headerBytesReceived);
            }
            catch (SocketException e)
            {
                Debug.LogError("UDP An error occurred when the client received the message: " + e.ToString());
                break; // If an exception occurs, the loop exits and the message is received
            }
        }
    }

    /// <summary>
    /// Deserialize different data classes based on different message ids
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="data"></param>
    private void DeserializeData(byte[] data, int bytesRead)
    {
        MessageBase messageBase = MessageBase.Deserialize(data);
        // Debug.Log("UDP The client receives the message. Procedure ID: " + messageBase.messageId + ", data: " + messageBase.data + "   byte:" + bytesRead);
        switch (messageBase.messageId)
        {
            case MessageType.SyncWeaponData:
                if (UnityMainThreadDispatcher.Exists())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        WeaponDataRa weaponData = messageBase.data as WeaponDataRa;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncWeaponData, weaponData);
                    });
                }
                break;
            case MessageType.SyncPlayerData:
                if (UnityMainThreadDispatcher.Exists())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        PlayerDataTf playerData = messageBase.data as PlayerDataTf;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncPlayerData, playerData);
                    });
                }
                break;
            case MessageType.SyncAirshipData:
                if (UnityMainThreadDispatcher.Exists())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        PlayerDataTf playerData = messageBase.data as PlayerDataTf;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncAirshipData, playerData);
                    });
                }
                break;
            case MessageType.SyncStarShootData:
                if (UnityMainThreadDispatcher.Exists())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        EnemyShootData data = messageBase.data as EnemyShootData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncStarShootData, data);
                    });
                }
                break;
            case MessageType.SyncPlaneShootData:
                if (UnityMainThreadDispatcher.Exists())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        EnemyShootData data = messageBase.data as EnemyShootData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncPlaneShootData, data);
                    });
                }
                break;
            case MessageType.SyncGuidedData:
                if (UnityMainThreadDispatcher.Exists())
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        EnemyShootData data = messageBase.data as EnemyShootData;
                        DynamicDataCenter.SendMessage(EmDataType.EmSyncGuidedShootData, data);
                    });
                }
                break;
            default:
                break;
        }
    }
}
