using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketUdpClient : Singleton<SocketUdpClient>
{
    static Socket client;
    Thread myThread;
    public bool isRunning;
    public void StartClient()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        SocketUdpClientManager.Instance.udpClient = client;
        client.Bind(new IPEndPoint(IPAddress.Parse(SocketUdpManager.Instance.localIp), SocketUdpManager.Instance.udpClientPort));
        myThread = new Thread(SocketUdpClientManager.Instance.ReceiveMsg);
        isRunning = true;
        myThread.IsBackground = true;
        myThread.Start();
        Debug.Log("The client has been started");
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
