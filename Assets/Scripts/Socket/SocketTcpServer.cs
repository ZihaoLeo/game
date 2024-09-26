using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

public class SocketTcpServer : Singleton<SocketTcpServer>
{
    public void StartTcpServer()
    {
        try
        {
            int _port = SocketTcpManager.Instance._port;
            string _ip = SocketTcpManager.Instance._ip;

            // When the client sends a heartbeat click to start listening, a Socket is created on the server that is responsible for listening to the IP and port number
            Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(_ip);
            // Create object port
            IPEndPoint point = new IPEndPoint(ip, _port);

            socketWatch.Bind(point);// Binding port number
            Debug.Log("¼àÌý³É¹¦!");
            socketWatch.Listen(50);// Set listening£¬A maximum of 10 devices can be connected simultaneously

            // Create a listening thread
            Thread thread = new Thread(Listen);
            thread.IsBackground = false;
            thread.Start(socketWatch);
        }
        catch { }

    }

    /// <summary>
    /// Wait for the client to connect and create a Socket to communicate with it
    /// </summary>
    Socket socketSend;
    void Listen(object o)
    {
        try
        {
            Socket socketWatch = o as Socket;
            while (true)
            {
                socketSend = socketWatch.Accept();// Wait to receive a client connection
                SocketTcpManager.Instance.AddSocket(socketSend);
                Debug.Log(socketSend.RemoteEndPoint.ToString() + ":" + "The connection to the TCP server succeeded. Procedure!");
                // Start a new thread that executes the receiving message method
                Thread r_thread = new Thread(SocketTcpServerManager.Instance.HandleClient);
                r_thread.IsBackground = false;
                r_thread.Start(socketSend);
            }
        }
        catch { }
    }
}
