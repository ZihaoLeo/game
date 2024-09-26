using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapPanel : UIBase
{
    public Transform StarParent;
    public Transform PlaneParent;
    public Transform GuidedParent;
    public Transform GemParent;

    Dictionary<int, EnemyDataTf> starDic;
    Dictionary<int, EnemyDataTf> enemyPlaneDic;
    Dictionary<int, EnemyDataTf> guidedMissileDic;
    Dictionary<int, EnemyDataTf> gemDic;

    private float limit = 27 * 10.24f / 2;
    private float uiLimit = 1080 / 2;
    private List<GameObject> listObj = new List<GameObject>();
    protected override void OnShow(params object[] parameters)
    {
        base.OnShow();
        (starDic, enemyPlaneDic, guidedMissileDic, gemDic) = PlayerManager.Instance.GetReceiveStartData();
        GameObject obj = null;
        foreach (var param in starDic.Values)
        {
            obj = GameUtils.CreateObj(StarParent, "Prefab/UIStarItem");
            // Debug.LogError("posX:" + param.posX + "   posY:" + param.posY + "   limit:" + limit + "  uiLimit: " + uiLimit);
            if (obj != null)
                SetData(obj, param.posX, param.posY);
        }
        foreach (var param in enemyPlaneDic.Values)
        {
            obj = GameUtils.CreateObj(PlaneParent, "Prefab/UIPlaneItem");
            if (obj != null)
                SetData(obj, param.posX, param.posY);
        }
        foreach (var param in guidedMissileDic.Values)
        {
            obj = GameUtils.CreateObj(GuidedParent, "Prefab/UIGuidedItem");
            if (obj != null)
                SetData(obj, param.posX, param.posY);
        }
        foreach (var param in gemDic.Values)
        {
            obj = GameUtils.CreateObj(GemParent, "Prefab/UIGemItem");
            if (obj != null)
                SetData(obj, param.posX, param.posY);
        }

        obj = GameUtils.CreateObj(transform, "Prefab/UIShipItem");
        if (obj != null)
            SetData(obj, BattleManager.Instance.Airship.localPosition.x, BattleManager.Instance.Airship.localPosition.y);
    }
    float percentageX = 0;
    float percentageY = 0;
    void SetData(GameObject obj, float posX, float posY)
    {
        percentageX = posX / limit * uiLimit;
        percentageY = posY / limit * uiLimit;
        GameUtils.SetLocalPosTF(obj.transform, percentageX, percentageY, 0);
        listObj.Add(obj);
    }

    protected override void OnHide()
    {
        base.OnHide();
        // Destruction of enemy game objects Here you can perform panel-specific logic, such as saving Settings
        Debug.Log("MainPanel Panel Hidden.");
        for (int i = listObj.Count - 1; i >= 0; i--)
        {
            Destroy(listObj[i]);
        }
    }
}
