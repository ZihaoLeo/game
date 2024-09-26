using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;

public class SocketUdpServer : Singleton<SocketUdpServer>
{
    static Socket server;
    Thread myThread;
    public bool isRunning;

    public void StartServer()
    {
        server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        SocketUdpServerManager.Instance.udpServer = server;
        server.Bind(new IPEndPoint(IPAddress.Parse(SocketTcpManager.Instance._ip), SocketUdpManager.Instance.udpServerPort));
        Debug.Log("The server has been started");

        myThread = new Thread(SocketUdpServerManager.Instance.ReceiveMsg);
        isRunning = true;
        myThread.IsBackground = true;
        myThread.Start();
    }

    public void OnClose()
    {
        // When the object is destroyed, the thread is stopped
        isRunning = false;
        myThread.Abort();
        //if (myThread != null && myThread.IsAlive)
        //{
        //    myThread.Join();
        //}
        //myThread = null;
    }
}
