using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

public class SocketTcpClient : Singleton<SocketTcpClient>
{
    public void OnClose()
    {
        Application.Quit();
    }

    // Use this for initialization
    Socket socketSend;

    public void Init()
    {
        // Enable the timer to periodically send heartbeat messages to the client
        TimerManager.Instance.Add(CallBack, 2, 30);
        StartTcpClient();
    }

    public void StartTcpClient()
    {
        try
        {
            int _port = SocketTcpManager.Instance._port;
            string _ip = SocketTcpManager.Instance._ip;
            SocketTcpClientManager.Instance.clientInfo = new ClientInfo();
            // Create the client Socket and obtain the remote ip address and port number
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketTcpClientManager.Instance.clientInfo.socket = socketSend;
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint point = new IPEndPoint(ip, _port);

            socketSend.BeginConnect(point, new AsyncCallback(ConnectCallback), socketSend);

        }
        catch (Exception)
        {
            Debug.LogError("IP Or the port number is incorrect...");
        }
    }

    // Asynchronous connection callback function
    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Complete connection operation
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            Debug.Log("The client connection is successful. Procedure!");
            SocketTcpClientManager.Instance.isConnect = true;
            SocketTcpClientManager.Instance.clientInfo.lastHeartbeatTime = DateTime.Now.Second;
            SocketTcpClientManager.Instance.clientInfo.state = true;
            // Start a new thread and keep receiving messages from the server
            Thread receiveThread = new Thread(SocketTcpClientManager.Instance.Received);
            receiveThread.IsBackground = false;
            receiveThread.Start();
            // send a message
            SocketTcpClientManager.Instance.SendMessage(MessageType.CreateClientId);
        }
        catch (Exception)
        {
            Debug.LogError("An error occurred while connecting to the server...");
        }
    }

    private void CallBack(int time)
    {
        //Debug.Log("The client sends a heartbeat:" + DateTime.Now.Second);
        SocketTcpClientManager.Instance.SendMessage(MessageType.SyncHeartbeat, null);
    }
}
