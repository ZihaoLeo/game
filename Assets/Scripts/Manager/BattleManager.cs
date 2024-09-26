using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Character state
/// </summary>
public enum PlayerType
{
    None = 0,
    Captain = 1,   
    Aviator = 2,   
    Engineer = 3, 
    Gunner = 4,     
}

/// <summary>
/// Ship status
/// </summary>
public enum SpaceshipType
{
    None = 0,
    FlySpaceship = 1,   // Flying a spaceship
    RepairShip = 2,     // Repairing the ship
    ControlBattery = 3, // At the control fort
}

public class BattleManager : MonoSingleton<BattleManager>
{
    public Transform BattleScene;
    public Transform MainCamera;
    public Transform Airship;
    public Transform PlayerParent;
    public Transform Weapon;
    public Transform Weapon1;
    public Transform Weapon2;
    public Transform StarParent;
    public Transform WarcraftParent;
    public Transform GuidedParent;
    public Transform GemParent;
    public Transform BulletParent;
    public Transform Canvas;
    public Text ShipIntegrity;
    public Text Funds;

    [HideInInspector]
    public SpaceshipType weaponType;
    [HideInInspector]
    public float starShootLimit = 9f; // Set planetary firing limits
    [HideInInspector]
    public float planeShootLimit = 18f; // Set limits on aircraft firing
    [HideInInspector]
    public float guidedShootLimit = 26f; // Set the missile line
    [HideInInspector]
    public float chaseThreshold = 56.0f; // Set tracking limits
    [HideInInspector]
    private int shipIntegrityNum = 100;
    private int shipIntegrityMaxNum = 100;
    private int fundsNum = 0;
    private Transform nowWeapon;
    private string weaponTag;
    private int recoveryTime = 1;// Ship health recovery time£¬1s/1 
    private int timeId;

    public float bounceDistance = 2.0f; // How far it bounces after impact
    public float bounceSpeed = 5.0f;    // The speed of rebound after impact
    private Rigidbody rb; 
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        AirshipDataCenter.Instance.Init();
        StarManager.Instance.Init(StarParent, WarcraftParent, GuidedParent, GemParent);
        PlayerManager.Instance.CreatePlayer(PlayerParent);
        AddMessage();
        ShipIntegrity.text = "Ship Integrity:" + shipIntegrityNum + "%";
        Funds.text = "Funds:" + fundsNum;
    }

    public void AirshipMove(Transform obstacleTransform)
    {
        BattleManager.Instance.SetState();
        // Calculate the direction of movement, here simplified as the vector from obstacle to object
        Vector3 movementDirection = (obstacleTransform.position - transform.position).normalized;

        // Calculate new position
        Vector3 newPosition = transform.position - movementDirection * bounceDistance;

        // moving object
        transform.position = newPosition;
    }

    public void OpenTimer()
    {
        TimerManager.Instance.Remove(timeId);
        timeId = TimerManager.Instance.Add(CallBack, recoveryTime);
    }

    public void RemoveTimer()
    {
        TimerManager.Instance.Remove(timeId);
    }

    private void CallBack(int time)
    {
        if (shipIntegrityNum >= shipIntegrityMaxNum)
            TimerManager.Instance.Remove(timeId);
        else
            SocketTcpClientManager.Instance.SendMessage(MessageType.SyncShipHealth);
        // Debug.LogError("time:" + time);
    }

    void AddMessage()
    {
        DynamicDataCenter.AddMessage(EmDataType.EmSyncWeaponData, OnEmSyncWeaponData);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncAirshipData, OnEmSyncAirshipData);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncShipHealth, OnEmSyncShipHealth);
    }

    private void OnEmSyncShipHealth(object[] paras)
    {
        AddShipIntegrity(recoveryTime);
    }

    private void OnEmSyncAirshipData(object[] paras)
    {
        PlayerDataTf playerData = (PlayerDataTf)paras[0];
        if (playerData == null)
        {
            return;
        }
        // Shield oneself 
        if (playerData.playerId == PlayerDataCenter.Instance.GetPlayerId())
        {
            // Debug.LogError("Shield oneself");
            return;
        }

        if (Airship != null)
        {
            GameUtils.SetLocalPosTF(Airship, playerData.posX, playerData.posY, playerData.posZ);
            GameUtils.SetRotationTF(Airship, playerData.rotateX, playerData.rotateY, playerData.rotateZ, playerData.rotateW);
        }
    }
    private void OnEmSyncWeaponData(object[] paras)
    {
        WeaponDataRa weaponData = (WeaponDataRa)paras[0];
        if (weaponData == null)
        {
            return;
        }
        if (weaponData.playerId == PlayerDataCenter.Instance.GetPlayerId())
        {
            return;
        }
        nowWeapon = null;
        switch (weaponData.firearmType.ToString())
        {
            case "Weapon":
                nowWeapon = Weapon;
                break;
            case "Weapon1":
                nowWeapon = Weapon1;
                break;
            case "Weapon2":
                nowWeapon = Weapon2;
                break;
            default:
                break;
        }
        if (nowWeapon != null)
        {
            GameUtils.SetRotationTF(nowWeapon, weaponData.rotateX, weaponData.rotateY, weaponData.rotateZ, weaponData.rotateW);
        }
    }

    /// <summary>
    /// Is it OK to shoot
    /// </summary>
    /// <param name="weaponName"></param>
    /// <returns></returns>
    public bool IsAttack(string weaponName)
    {
        return weaponName != null && IsControlBattery() && weaponType == SpaceshipType.ControlBattery && weaponTag != null && weaponName == weaponTag;
    }

    /// <summary>
    /// Whether you can control the fort
    /// </summary>
    /// <returns></returns>
    public bool IsControlBattery()
    {
        return PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain || PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Aviator;
    }

    /// <summary>
    /// Whether you can move the ship
    /// </summary>
    /// <returns></returns>
    public bool IsMoveSpaceship()
    {
        return IsFlySpaceship() && weaponType == SpaceshipType.FlySpaceship;
    }

    /// <summary>
    /// Whether you can control the ship
    /// </summary>
    /// <returns></returns>
    public bool IsFlySpaceship()
    {
        return PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain || PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Gunner;
    }

    /// <summary>
    /// Whether the ship can be repaired
    /// </summary>
    /// <returns></returns>
    public bool IsRepairShip()
    {
        return PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain || PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Engineer;
    }

    public bool PlayerIsMove()
    {
        return weaponType == SpaceshipType.None;
    }

    public void SetWeaponType(SpaceshipType type)
    {
        weaponType = type;
    }

    public void ResetWeaponType()
    {
        weaponType = SpaceshipType.None;
        weaponTag = null;
    }

    public void ChangeMainCamera(string tag = null)
    {
        float posY = 0;
        if (weaponType == SpaceshipType.ControlBattery)
        {
            weaponTag = tag;
            switch (tag)
            {
                case "Weapon":
                    posY = 5.84f;
                    break;
                case "Weapon1":
                    posY = -6.36f;
                    break;
                case "Weapon2":
                    posY = 6.11f;
                    break;
                default:
                    break;
            }
        }
        GameUtils.SetLocalPosTF(MainCamera, new Vector3(0, posY, -5));
    }

    public void ChangeBattleScene()
    {
        float scale = 1;
        if (weaponType == SpaceshipType.FlySpaceship)
        {
            scale = 0.5f;
        }
        GameUtils.SetScaleTF(BattleScene, scale, scale, scale);
    }

    public void ReduceShipIntegrity(int value)
    {
        if (!PlayerManager.Instance.invincible)
        {
            shipIntegrityNum -= value;
            shipIntegrityNum = shipIntegrityNum < 0 ? 0 : shipIntegrityNum;
            if (shipIntegrityNum <= 0)
                UIManager.Instance.OpenUIPanel<SettlementInterfacePanel>(Canvas, WindowName.SettlementInterfacePanel, "Lost");
            ShipIntegrity.text = "Ship Integrity:" + shipIntegrityNum + "%";
        }
    }

    public void SetShipIntegrityMaxNum(int value)
    {
        shipIntegrityMaxNum += value;
        AddShipIntegrity(value);
    }

    public void AddShipIntegrity(int value)
    {
        shipIntegrityNum += value;
        shipIntegrityNum = shipIntegrityNum > shipIntegrityMaxNum ? shipIntegrityMaxNum : shipIntegrityNum;
        ShipIntegrity.text = "Ship Integrity:" + shipIntegrityNum + "%";
    }

    public void ReducFunds(int value)
    {
        fundsNum -= value;
        Funds.text = "Funds:" + fundsNum;
        PlayerManager.Instance.SetPlayerLevel();
        if (PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain && fundsNum >= 100)
            UIManager.Instance.OpenUIPanel<UpGradePanel>(Canvas, WindowName.UpGradePanel);
    }

    public void Addunds(int value)
    {
        fundsNum += value;
        Funds.text = "Funds:" + fundsNum;
        if (PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain && fundsNum >= 100)
            UIManager.Instance.OpenUIPanel<UpGradePanel>(Canvas, WindowName.UpGradePanel);
    }

    public void CreateFlameItem(Transform trans)
    {
        FlameController gemController;
        GameObject obj = GameUtils.CreateObj(trans, "Prefab/FlameItem");
        if (obj != null)
        {
            gemController = obj.GetComponent<FlameController>();
        }
    }

    private void Update()
    {
        OpenMiniMapPanel();
    }

    int miniMapPanelType = 0;
    void OpenMiniMapPanel()
    {
       if (PlayerDataCenter.Instance.GetPlayerType() == PlayerType.Captain && Input.GetKeyDown(KeyCode.P))
        {
            if (miniMapPanelType == 0)
            {
                miniMapPanelType = 1;
                UIManager.Instance.OpenUIPanel<MiniMapPanel>(Canvas, WindowName.MiniMapPanel);
            }
            else
            {
                miniMapPanelType = 0;
                UIManager.Instance.CloseUIPanel(WindowName.MiniMapPanel);
            }
        }
    }

    public void SetState()
    {
        string tipsContent = "";
        switch (BattleManager.Instance.weaponType)
        {
            case SpaceshipType.None:
                break;
            case SpaceshipType.FlySpaceship:
                BattleManager.Instance.ResetWeaponType();
                PlayerManager.Instance.SetBoundary();
                tipsContent = "Press E to control the rudder";
                break;
            case SpaceshipType.RepairShip:
                BattleManager.Instance.RemoveTimer();
                BattleManager.Instance.ResetWeaponType();
                tipsContent = "Press E to repair the ship";
                break;
            case SpaceshipType.ControlBattery:
                BattleManager.Instance.ResetWeaponType();
                BattleManager.Instance.ChangeMainCamera();
                tipsContent = "Press E to control the battery";
                break;
            default:
                break;
        }
        PlayerController.Instance.SetTipsText(tipsContent);
    }
}
