using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class SocketUdpServerManager : Singleton<SocketUdpServerManager>
{
    public Socket udpServer;
    List<EndPoint> clientEndPoints = new List<EndPoint>(); // Stores the address and port information of all clients

    public void ReceiveMsg()
    {
        while (true)//SocketUdpServer.Instance.isRunning)
        {
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0); // Used to store the IP address and port number of the sender
            try
            {
                // Process the received data
                byte[] headerBuffer = new byte[8192];
                // RXD

                int headerBytesReceived = udpServer.ReceiveFrom(headerBuffer, ref clientEndPoint);
                if (headerBytesReceived > 0)
                {
                    if (!clientEndPoints.Contains(clientEndPoint))
                    {
                        // When a message is received, if the client address is not in the list, it is added to the list
                        AddClientEndPoint(clientEndPoint);
                    }
                    // Deserialize the received data
                    MessageBase receivedMessage = MessageBase.Deserialize(headerBuffer);

                    // Process the received data
                    SendMsgToAllClients(headerBuffer);
                }
            }
            catch (Exception e)
            {
                Debug.Log("UDP Error occurred while the server was processing a client message: " + e.Message);
                continue; // 出现异常后跳出循环，结束接收消息
            }
        }
    }

    public void AddClientEndPoint(EndPoint endPoint)
    {
        clientEndPoints.Add(endPoint);
    }

    public void RemoveClientEndPoint(EndPoint endPoint)
    {
        clientEndPoints.Remove(endPoint);
    }

    public void SendMsgToAllClients(byte[] sendDataBy)
    {
        foreach (EndPoint endPoint in clientEndPoints)
        {
            udpServer.SendTo(sendDataBy, endPoint);
        }
    }
}
