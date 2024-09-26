using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SocketTcpServerManager : Singleton<SocketTcpServerManager>
{
    public List<Socket> sendSockets = new List<Socket>();
    private int playerId = 1;
    // data length The server processes client messages
    public void HandleClient(object clientSocketObj)
    {
        Socket socket = (Socket)clientSocketObj;
        try
        {
            while (true)
            {
                try
                {
                    if (socket != null && SocketTcpManager.Instance.GetSocketByKey(socket))
                    {
                        // 先接收消息长度
                        byte[] lengthBytes = new byte[4];
                        int bytesReceived = socket.Receive(lengthBytes, 0, 4, SocketFlags.None);
                        if (bytesReceived == 0)
                        {
                            Debug.LogError($"client {socket.RemoteEndPoint} disconnect");
                            SocketTcpManager.Instance.RemoveSocket(socket);
                            return;
                        }

                        int messageLength = BitConverter.ToInt32(lengthBytes, 0);
                        // 根据消息长度接收数据
                        byte[] receiveBuffer = new byte[messageLength];
                        int totalBytesReceived = 0;

                        //Debug.Log("bytesReceived:" + bytesReceived + "  messageLength:" + messageLength);
                        while (totalBytesReceived < messageLength)
                        {
                            bytesReceived = socket.Receive(receiveBuffer, totalBytesReceived, messageLength - totalBytesReceived, SocketFlags.None);
                            if (bytesReceived == 0)
                            {
                                Debug.LogError($"client {socket.RemoteEndPoint} disconnect");
                                SocketTcpManager.Instance.RemoveSocket(socket);
                                return;
                            }
                            totalBytesReceived += bytesReceived;
                        }
                        // 处理接收到的数据
                        byte[] messageBytes = new byte[messageLength];
                        Array.Copy(receiveBuffer, messageBytes, messageLength);
                        HandleReceivedData(socket, receiveBuffer, messageLength);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error occurred while the server was processing a client message: " + e.Message);
                    SocketTcpManager.Instance.RemoveSocket(socket);
                    break;
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Client " + socket.RemoteEndPoint + " disconnected: " + e.Message);
            SocketTcpManager.Instance.RemoveSocket(socket);
        }
    }

    // 服务器处理接收到的数据
    private void HandleReceivedData(Socket socket, byte[] receivedData, int bytesRead)
    {
        try
        {
            MessageBase sendData = new MessageBase();
            // 将字节数组反序列化为数据类实例
            MessageBase messageBase = MessageBase.Deserialize(receivedData);
            messageBase.feedback = 1000;
            sendData = messageBase;
            Debug.Log("The server receives the message ID: " + messageBase.messageId + ", data: " + messageBase.data + "   Byte length:" + bytesRead);
            sendSockets.Clear();
            PlayerData playerData;
            switch (messageBase.messageId)
            {
                case MessageType.CreateClientId:
                    playerData = new PlayerData();
                    playerData.playerId = playerId.ToString();
                    playerData.playerName = playerId.ToString();
                    playerData.playerType = PlayerType.None;
                    playerId += 1;
                    sendData.data = playerData;
                    ServerDataCenter.Instance.AddPlayerData(playerData);
                    break;
                case MessageType.CreateRoomData:
                    if (ServerDataCenter.Instance.CheckRoomExists())
                        messageBase.feedback = 1001;
                    else
                    {
                        playerData = messageBase.data as PlayerData;
                        sendData.data = ServerDataCenter.Instance.GetPlayerData(playerData);
                        ServerDataCenter.Instance.AddRoomPlayer(playerData);
                    }
                    break;
                case MessageType.AddRoomData:
                    if (ServerDataCenter.Instance.CheckRoomExists())
                    {
                        playerData = messageBase.data as PlayerData;
                        sendData.data = ServerDataCenter.Instance.GetPlayerData(playerData);
                        ServerDataCenter.Instance.AddRoomPlayer(playerData);
                    }
                    else
                        messageBase.feedback = 1001;
                    break;
                case MessageType.GetRoomPlayerData:
                    playerData = messageBase.data as PlayerData;
                    ServerDataCenter.Instance.AddRoomPlayerData(playerData);
                    sendData.data = ServerDataCenter.Instance.GetRoomAllPlayerData();
                    break;
                case MessageType.SyncPlayerNameData:
                    playerData = messageBase.data as PlayerData;
                    ServerDataCenter.Instance.ChangePlayerData(playerData);
                    sendData.data = ServerDataCenter.Instance.GetRoomAllPlayerData();
                    break;
                case MessageType.SelectPlayerOccupation:
                    playerData = messageBase.data as PlayerData;
                    ServerDataCenter.Instance.ChangePlayerData(playerData);
                    sendData.data = ServerDataCenter.Instance.GetPlayerData(playerData);
                    break;
                case MessageType.QuitRoom:
                    playerData = messageBase.data as PlayerData;
                    if (ServerDataCenter.Instance.QuitRoom(playerData))
                        sendData.data = ServerDataCenter.Instance.GetPlayerData(playerData);
                    else
                        messageBase.feedback = 1001;
                    break;
                case MessageType.StartGame:
                    SendStartData startData = messageBase.data as SendStartData;
                    if (startData.playerData.playerType != PlayerType.Captain)
                        messageBase.feedback = 1001;
                    else
                    {
                        ReceiveStartData receiveStartData = new ReceiveStartData();
                        receiveStartData.list = ServerDataCenter.Instance.GetRoomAllPlayerData();
                        receiveStartData.starDic = startData.starDic;
                        receiveStartData.enemyPlaneDic = startData.enemyPlaneDic;
                        receiveStartData.guidedMissileDic = startData.guidedMissileDic;
                        receiveStartData.gemDic = startData.gemDic;
                        sendData.data = receiveStartData;
                    }
                    break;
                case MessageType.SyncShoot:
                    WeaponData data = messageBase.data as WeaponData;
                    if (!ServerDataCenter.Instance.IsShoot(data))
                        messageBase.feedback = 1001;
                    break;
                case MessageType.SyncSettlementInterface:
                    sendSockets = SocketTcpManager.Instance.GetSockets();
                    break;
                case MessageType.ClientOnApplicationQuit:
                    SocketTcpManager.Instance.RemoveSocket(socket);
                    break;
                case MessageType.ServerOnApplicationQuit:
                    SocketTcpManager.Instance.RemoveSocket(socket);
                    break;
                default:
                    break;
            }
            SocketTcpManager.Instance.SendMessage(sendData);
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing received data: " + socket.RemoteEndPoint.ToString() + "        " + e.Message);
            SocketTcpManager.Instance.RemoveSocket(socket);
        }
    }
}
