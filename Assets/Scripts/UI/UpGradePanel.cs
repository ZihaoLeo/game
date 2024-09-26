using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpGradePanel : UIBase
{
    public Text level;
    protected override void OnShow(params object[] parameters)
    {
        base.OnShow();
        level.text = PlayerManager.Instance.playerLevel.ToString();
        BattleManager.Instance.SetState();
    }

    public void OnClickBtn1()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.SyncIncreaseFitness);
        OnHide();
    }
    public void OnClickBtn2()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.SyncIncreasedWeaponDamage);
        OnHide();
    }
    public void OnClickBtn3()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.SyncIncreasedWeaponEnchantment);
        OnHide();
    }
    public void OnClickBtn4()
    {
        SocketTcpClientManager.Instance.SendMessage(MessageType.SyncIncreaseShipSpeed);
        OnHide();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Debug.Log("MainPanel Panel Hidden.");
    }
}
