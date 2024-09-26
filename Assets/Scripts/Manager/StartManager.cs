using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class StartManager : MonoSingleton<StartManager>
{
    public Transform panelParent;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        StreamManager.Instance.InitTxtData();
        PlayerManager.Instance.Init();
        SocketTcpManager.Instance.Init();
        SocketTcpClient.Instance.Init();
        SocketUdpClient.Instance.StartClient();

        UIManager.Instance.OpenUIPanel<StartPanel>(panelParent, WindowName.StartPanel);
    }

    private void Update()
    {
        TimerManager.Instance.Update();
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
