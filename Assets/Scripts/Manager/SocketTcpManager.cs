using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Client information
/// </summary>
public class ClientInfo
{
    // client Socket
    public Socket socket;
    // If the last heartbeat time is longer than five seconds, the client synchronizes data with the server every two seconds
    public int lastHeartbeatTime;
    // Status false Disconnect
    public bool state;
}

public class SocketTcpManager : Singleton<SocketTcpManager>
{
    public int _port;
    public string _ip;
    public bool isServer;
    public Dictionary<string, ClientInfo> sockets;
    public void Init()
    {
        sockets = new Dictionary<string, ClientInfo>();
        if (isServer)
        {
            SocketTcpServer.Instance.StartTcpServer();
            SocketUdpServer.Instance.StartServer();
        }
    }
    public void ServerQuit()
    {
        foreach (var item in sockets.Values)
        {
            if (item.state && item.socket.Connected)
                item.socket.Close();
        }
        ClientQuit();
    }
    public void ClientQuit()
    {
        SocketTcpClientManager.Instance.isConnect = false;
        SocketTcpClientManager.Instance.clientInfo.socket.Close();
        // ServerDataCenter.Instance.SaveData();
        Debug.Log("quit a game");
    }

    public void AddSocket(Socket socket)
    {
        ClientInfo clientInfo = new ClientInfo();
        clientInfo.socket = socket;
        clientInfo.lastHeartbeatTime = DateTime.Now.Second;
        clientInfo.state = true;
        sockets.Add(socket.RemoteEndPoint.ToString(), clientInfo);
    }

    public void RemoveSocket(Socket socket)
    {
        if (socket != null && socket.Connected)
        {
            ClientInfo clientInfo;
            string key = socket.RemoteEndPoint.ToString();
            if (sockets.TryGetValue(key, out clientInfo))
            {
                clientInfo.lastHeartbeatTime = DateTime.Now.Second;
                clientInfo.state = false;
                clientInfo.socket.Close();
                clientInfo.socket.Close();
                sockets.Remove(key);
            }
        }
    }

    public bool GetSocketByKey(Socket socket)
    {
        if (socket != null && socket.Connected)
        {
            ClientInfo clientInfo;
            string key = socket.RemoteEndPoint.ToString();
            // Debug.LogError("key:" + key);
            if (sockets.TryGetValue(key, out clientInfo))
            {
                return clientInfo.state;
            }
        }
        return false;
    }

    public void ChangeSocket(Socket socket)
    {
        ClientInfo clientInfo;
        if (sockets.TryGetValue(socket.RemoteEndPoint.ToString(), out clientInfo))
        {
            clientInfo.lastHeartbeatTime = DateTime.Now.Second;
            clientInfo.state = true;
        }
    }

    public void SendMessage(MessageBase sendData)
    {
        byte[] sendDataBy = SerializeData(sendData);
        foreach (var item in sockets.Values)
        {
            //Debug.LogError(item.socket.RemoteEndPoint.ToString() + "     " + item.socket.Connected + "   " + item.state);
            if (item.state && item.socket.Connected)
            {
                item.socket.Send(sendDataBy);
            }
        }
    }

    public List<Socket> GetSockets()
    {
        List<Socket> list = new List<Socket>();
        return list;
    }

    public byte[] SerializeData(MessageBase data)
    {
        // Converts a data class instance to a byte array
        byte[] messageBytes = data.Serialize();
        byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

        // Merge message length and message content
        byte[] sendData = new byte[lengthBytes.Length + messageBytes.Length];
        Array.Copy(lengthBytes, 0, sendData, 0, lengthBytes.Length);
        Array.Copy(messageBytes, 0, sendData, lengthBytes.Length, messageBytes.Length);
        //Debug.Log("data length SerializeData" + messageBytes.Length + "   " + lengthBytes.Length + "   " + BitConverter.ToInt32(lengthBytes, 0));
        return sendData;
    }
}
