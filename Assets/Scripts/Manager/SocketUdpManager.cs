using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketUdpManager : Singleton<SocketUdpManager>
{
    public string localIp;
    public int udpServerPort;
    public int udpClientPort;

    public byte[] SerializeData(MessageBase data)
    {
        // Converts a data class instance to a byte array
        byte[] messageBytes = data.Serialize();
        // Merge message length and message content
        byte[] sendData = new byte[messageBytes.Length];
        Array.Copy(messageBytes, 0, sendData, 0, messageBytes.Length);
        //Debug.LogError("data length SerializeData" + messageBytes.Length + "   " + lengthBytes.Length + "   " + BitConverter.ToInt32(lengthBytes, 0));
        return sendData;
    }
}
