using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

public class StarManager : Singleton<StarManager>
{
    private Transform StarParent;
    private Transform EnemyPlaneParent;
    private Transform GuidedMissileParent;
    private Transform GemParent;
    private Dictionary<int, StarController> starModelDic = new Dictionary<int, StarController>();
    private Dictionary<int, EnemyPlaneController> enemyPlaneModelDic = new Dictionary<int, EnemyPlaneController>();
    private Dictionary<int, GuidedMissileController> guidedMissileModelDic = new Dictionary<int, GuidedMissileController>();
    private Dictionary<int, GemController> gemModelDic = new Dictionary<int, GemController>();

    public void Init(Transform parent, Transform parent1, Transform parent2, Transform parent3)
    {
        StarParent = parent;
        EnemyPlaneParent = parent1;
        GuidedMissileParent = parent2;
        GemParent = parent3;
        CreateStar();
        CreateEnemyPlane();
        CreateGuidedMissile();
        CreateGem();

        DynamicDataCenter.AddMessage(EmDataType.EmSyncStarHealth, OnEmSyncStarHealth);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncEnemyPlane, OnEmSyncEnemyPlane);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncGuidedMissile, OnEmSyncGuidedMissile);
    }

    private void OnEmSyncStarHealth(object[] paras)
    {
        StarData data = (StarData)paras[0];
        if (starModelDic.ContainsKey(data.StarId))
        {
            starModelDic[data.StarId].TakeDamage(data.damage);
        }
    }

    private void OnEmSyncEnemyPlane(object[] paras)
    {
        EnemyPlaneData data = (EnemyPlaneData)paras[0];
        if (enemyPlaneModelDic.ContainsKey(data.EnemyPlaneId))
        {
            enemyPlaneModelDic[data.EnemyPlaneId].TakeDamage(data.damage);
        }
    }

    private void OnEmSyncGuidedMissile(object[] paras)
    {
        GuidedMissileData data = (GuidedMissileData)paras[0];
        if (guidedMissileModelDic.ContainsKey(data.GuidedMissileId))
        {
            guidedMissileModelDic.Remove(data.GuidedMissileId);
        }
    }

    public void CreateStar()
    {
        StarController starController;
        for (int i = 0; i < PlayerManager.Instance.maxStarModelCount; i++)
        {
            GameObject obj = GameUtils.CreateObj(StarParent, "Prefab/StarItem");
            if (obj != null)
            {
                PlayerManager.Instance.SetTransformPosition(1, i, obj.transform);
                starController = obj.GetComponent<StarController>();
                starController.Init(BattleManager.Instance.Airship, BattleManager.Instance.BulletParent, i);
                starModelDic.Add(i, starController);
            }
        }
    }

    public void CreateEnemyPlane()
    {
        EnemyPlaneController enemyController;
        for (int i = 0; i < PlayerManager.Instance.maxEnemyPlaneModelCount; i++)
        {
            GameObject obj = GameUtils.CreateObj(EnemyPlaneParent, "Prefab/WarcraftItem");
            if (obj != null)
            {
                PlayerManager.Instance.SetTransformPosition(2, i, obj.transform);
                enemyController = obj.GetComponent<EnemyPlaneController>();
                enemyController.Init(BattleManager.Instance.Airship, BattleManager.Instance.BulletParent, i);
                enemyPlaneModelDic.Add(i, enemyController);
            }
        }
    }

    public void CreateGuidedMissile()
    {
        GuidedMissileController guidedMissileController;
        for (int i = 0; i < PlayerManager.Instance.maxGuidedMissileModelCount; i++)
        {
            GameObject obj = GameUtils.CreateObj(GuidedMissileParent, "Prefab/GuidedItem");
            if (obj != null)
            {
                PlayerManager.Instance.SetTransformPosition(3, i, obj.transform);
                guidedMissileController = obj.GetComponent<GuidedMissileController>();
                guidedMissileController.Init(BattleManager.Instance.Airship, i);
                guidedMissileModelDic.Add(i, guidedMissileController);
            }
        }
    }

    public void CreateGem()
    {
        GemController gemController;
        for (int i = 0; i < PlayerManager.Instance.defaultMaxGemModelCount; i++)
        {
            GameObject obj = GameUtils.CreateObj(GemParent, "Prefab/GemItem");
            if (obj != null)
            {
                PlayerManager.Instance.SetTransformPosition(4, i, obj.transform);
                gemController = obj.GetComponent<GemController>();
                gemModelDic.Add(i, gemController);
            }
        }
    }
}
