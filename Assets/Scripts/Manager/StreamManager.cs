using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StreamManager : Singleton<StreamManager>
{
    public string serverPath = "Server.txt";

    public string GetTextForStreamingAssets(string path)
    {
        string localPath = "file:///" + Application.streamingAssetsPath + "/" + path;
        WWW www = new WWW(localPath);

        if (www.error != null)
        {
            Debug.Log("error while reading files : " + localPath);
            return ""; //Error reading file
        }
        while (!www.isDone) { }
        Debug.Log("File content :  " + www.text);//www There are also properties to get the byte array
        return www.text;
    }

    public string[] LoadConfig(string path)
    {
        string[] config = null;
        if (File.Exists(Path.Combine(Application.streamingAssetsPath, path)))
            config = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, path));
        return config;
    }

    public void InitTxtData()
    {
        InitServerData();
    }

    public void InitServerData()
    {
        string[] config = LoadConfig(serverPath);
        if (config != null)
        {
            string[] arr1;
            for (int i = 0; i < config.Length; i++)
            {
                arr1 = config[i].Split("=");
                if (arr1[0] == "serverIp")
                    SocketTcpManager.Instance._ip = arr1[1];
                else if (arr1[0] == "tcpServerPort")
                    SocketTcpManager.Instance._port = int.Parse(arr1[1]);
                else if (arr1[0] == "localIp")
                    SocketUdpManager.Instance.localIp = arr1[1];
                else if (arr1[0] == "udpServerPort")
                    SocketUdpManager.Instance.udpServerPort = int.Parse(arr1[1]);
                else if (arr1[0] == "udpClientPort")
                {
                    SocketUdpManager.Instance.udpClientPort = int.Parse(arr1[1]);
                }
                else if (arr1[0] == "server")
                    SocketTcpManager.Instance.isServer = arr1[0] == "server" && arr1[1] == "1";
            }
        }
    }
}
