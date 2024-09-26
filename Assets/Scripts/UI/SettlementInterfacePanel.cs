using UnityEngine;
using UnityEngine.UI;

public class SettlementInterfacePanel : UIBase
{
    public Text level;
    protected override void OnShow(params object[] parameters)
    {
        level.text = (string)parameters[0];
    }

    protected override void OnHide()
    {
        base.OnHide();
        // Here you can do panel-specific logic, such as saving Settings
        Debug.Log("MainPanel Panel Hidden.");
    }


}
