using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerTrans
{
    public Transform playerObj;
}

public class PlayerManager : Singleton<PlayerManager>
{
    private Dictionary<string, Transform> playerModelDic = new Dictionary<string, Transform>();
    public DifficultyType difficultyType;

    [HideInInspector]
    public SendStartData sendStartData;
    private ReceiveStartData receiveStartData;

    [HideInInspector]
    public int maxStarModelCount;
    private int defaultMaxStarModelCount = 24; // The number of planets generated
    [HideInInspector]
    public int maxEnemyPlaneModelCount;
    private int defaultMaxEnemyPlaneModelCount = 24; // Generate the number of enemy aircraft
    [HideInInspector]
    public int maxGuidedMissileModelCount;
    private int defaultMaxGuidedMissileModelCount = 10; // Generates the number of missiles
    [HideInInspector]
    public int defaultMaxGemModelCount = 60; // The amount of gem resources generated

    [HideInInspector]
    public float minX;
    [HideInInspector]
    public float maxX;
    [HideInInspector]
    public float minY;
    [HideInInspector]
    public float maxY;

    [HideInInspector]
    public int playerLevel;
    public int maxPlayerLevel;

    private float defaultMinX = -110;
    private float defaultMaxX = 110f;
    private float defaultMinY = -110;
    private float defaultMaxY = 110f;
    [HideInInspector]
    public bool invincible = false;
    public void Init()
    {
        SetBoundary();
        playerLevel = 1; // 
        maxPlayerLevel = 10;
        sendStartData = new SendStartData();
        SetRandoem(DifficultyType.Simpleness);
        DynamicDataCenter.AddMessage(EmDataType.EmStartGame, OnEmStartGame);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncPlayerData, OnEmSyncPlayerData);
    }

    public void SetPlayerLevel()
    {
        playerLevel += 1;
        if (playerLevel >= maxPlayerLevel)
            UIManager.Instance.OpenUIPanel<SettlementInterfacePanel>(BattleManager.Instance.Canvas, WindowName.SettlementInterfacePanel, "Win");
    }
    public SendStartData GetSendStartData()
    {
        sendStartData.playerData = PlayerDataCenter.Instance.GetPlayerData();
        return sendStartData;
    }

    float radius;
    public void SetBoundary(bool flag = true)
    {
        minX = flag ? defaultMinX : defaultMinX / 2;
        maxX = flag ? defaultMaxX : defaultMaxX / 2;
        minY = flag ? defaultMinY : defaultMinY / 2;
        maxY = flag ? defaultMaxY : defaultMaxY / 2;
        radius = flag ? 18f : 9f;
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.starShootLimit = flag ? 9f : 4.5f;
            BattleManager.Instance.planeShootLimit = flag ? 18f : 9f;
            BattleManager.Instance.chaseThreshold = flag ? 56f : 28f;
            BattleManager.Instance.guidedShootLimit = flag ? 26f : 13;
        }
        StarGenerator.Instance.Init(minX, maxX, minY, maxY, radius);
    }

    private void OnEmStartGame(object[] paras)
    {
        receiveStartData = (ReceiveStartData)paras[0];
        SceneManager.LoadSceneAsync("BattleScene");
    }

    public void SetRandoem(DifficultyType type)
    {
        difficultyType = type;
        // Initializes the random number generator at some start point in the program
        switch (difficultyType)
        {
            case DifficultyType.Simpleness:
                maxStarModelCount = (int)(defaultMaxStarModelCount * 0.5f);
                maxEnemyPlaneModelCount = (int)(defaultMaxEnemyPlaneModelCount * 0.5f);
                maxGuidedMissileModelCount = (int)(defaultMaxGuidedMissileModelCount * 0.5f);
                break;
            case DifficultyType.Common:
                maxStarModelCount = (int)(defaultMaxStarModelCount * 1f);
                maxEnemyPlaneModelCount = (int)(defaultMaxEnemyPlaneModelCount * 1f);
                maxGuidedMissileModelCount = (int)(defaultMaxGuidedMissileModelCount * 1f);
                break;
            case DifficultyType.Difficulty:
                maxStarModelCount = (int)(defaultMaxStarModelCount * 1.5f);
                maxEnemyPlaneModelCount = (int)(defaultMaxEnemyPlaneModelCount * 1.5f);
                maxGuidedMissileModelCount = (int)(defaultMaxGuidedMissileModelCount * 1.5f);
                break;
            default:
                break;
        }
        sendStartData.starDic.Clear();
        sendStartData.enemyPlaneDic.Clear();
        sendStartData.guidedMissileDic.Clear();
        sendStartData.gemDic.Clear();
        EnemyDataTf enemyDataTf;
        Vector2 vector2;
        for (int i = 0; i < maxStarModelCount; i++)
        {
            //vector2 = GenerateStars();
            vector2 = StarGenerator.Instance.GenerateStars();
            enemyDataTf = new EnemyDataTf(i, vector2);
            sendStartData.starDic.Add(i, enemyDataTf);
        }

        for (int i = 0; i < maxEnemyPlaneModelCount; i++)
        {
            //vector2 = GenerateStars();
            vector2 = StarGenerator.Instance.GenerateStars();
            enemyDataTf = new EnemyDataTf(i, vector2);
            sendStartData.enemyPlaneDic.Add(i, enemyDataTf);
        }

        for (int i = 0; i < maxGuidedMissileModelCount; i++)
        {
            //vector2 = GenerateStars();
            vector2 = StarGenerator.Instance.GenerateStars();
            enemyDataTf = new EnemyDataTf(i, vector2);
            sendStartData.guidedMissileDic.Add(i, enemyDataTf);
        }

        for (int i = 0; i < defaultMaxGemModelCount; i++)
        {
            //vector2 = GenerateStars();
            vector2 = StarGenerator.Instance.GenerateStars();
            enemyDataTf = new EnemyDataTf(i, vector2);
            sendStartData.gemDic.Add(i, enemyDataTf);
        }
    }

    Vector2 GenerateStars()
    {
        // The location of the spawning planet
        Vector2 starPosition = new Vector2(
            UnityEngine.Random.Range(minX, maxX),
            UnityEngine.Random.Range(minY, maxY)
        );
        // Debug.LogError(starPosition.ToString());
        return starPosition;
    }

    public void SetTransformPosition(int type, int key, Transform trans)
    {
        switch (type)
        {
            case 1:
                if (receiveStartData.starDic.ContainsKey(key))
                {
                    trans.position = new Vector2(receiveStartData.starDic[key].posX, receiveStartData.starDic[key].posY);
                }
                break;
            case 2:
                if (receiveStartData.enemyPlaneDic.ContainsKey(key))
                {
                    trans.position = new Vector2(receiveStartData.enemyPlaneDic[key].posX, receiveStartData.enemyPlaneDic[key].posY);
                }
                break;
            case 3:
                if (receiveStartData.guidedMissileDic.ContainsKey(key))
                {
                    trans.position = new Vector2(receiveStartData.guidedMissileDic[key].posX, receiveStartData.guidedMissileDic[key].posY);
                }
                break;
            case 4:
                if (receiveStartData.gemDic.ContainsKey(key))
                {
                    //Debug.LogError(receiveStartData.gemDic[key].posX + "   " + receiveStartData.gemDic[key].posY);
                    trans.position = new Vector2(receiveStartData.gemDic[key].posX, receiveStartData.gemDic[key].posY);
                }
                break;
            default:
                break;
        }
    }

    private void OnEmSyncPlayerData(object[] paras)
    {
        PlayerDataTf playerData = (PlayerDataTf)paras[0];
        if (playerData == null)
        {
            return;
        }
        if (playerData.playerId == PlayerDataCenter.Instance.GetPlayerId())
        {
            return;
        }

        Transform playerTransform;
        if (playerModelDic.TryGetValue(playerData.playerId, out playerTransform))
        {
            GameUtils.SetLocalPosTF(playerTransform, playerData.posX, playerData.posY, playerData.posZ);
            GameUtils.SetRotationTF(playerTransform, playerData.rotateX, playerData.rotateY, playerData.rotateZ, playerData.rotateW);
        }
    }

    public void CreatePlayer(Transform parent)
    {
        string str = null;
        PlayerData data = null;
        for (int i = 0; i < receiveStartData.list.playerDatas.Count; i++)
        {
            data = receiveStartData.list.playerDatas[i];
            switch (data.playerType)
            {
                case PlayerType.None:
                    break;
                case PlayerType.Captain:
                    str = "Prefab/PlayerC";
                    break;
                case PlayerType.Aviator:
                    str = "Prefab/PlayerF";
                    break;
                case PlayerType.Engineer:
                    str = "Prefab/PlayerG";
                    break;
                case PlayerType.Gunner:
                    str = "Prefab/PlayerP";
                    break;
                default:
                    break;
            }
            GameObject obj = null;
            if (str != null)
            {
                obj = GameUtils.CreateObj(parent, str);
                obj.transform.Find("Canvas/Text (Legacy)").GetComponent<Text>().text = data.playerName;
                PlayerController playerController = obj.GetComponent<PlayerController>();
                playerController.SetPlayerId(data.playerId);
                playerController.enabled = data.playerId == PlayerDataCenter.Instance.GetPlayerId();
                playerModelDic.Add(data.playerId, obj.transform);
            }
            else
                Debug.LogError("str is null" + data.playerType);
        }
    }

    public (Dictionary<int, EnemyDataTf>, Dictionary<int, EnemyDataTf>, Dictionary<int, EnemyDataTf>, Dictionary<int, EnemyDataTf>) GetReceiveStartData()
    {
        return (receiveStartData.starDic, receiveStartData.enemyPlaneDic, receiveStartData.guidedMissileDic, receiveStartData.gemDic);
    }
}
