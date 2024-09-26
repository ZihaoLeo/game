using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageData
{

}


[Serializable]
public class PlayerDataList
{
    public List<PlayerData> playerDatas = new List<PlayerData>();

}

[Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;
    public PlayerType playerType;
    public PlayerDataTf playerDataTf;
    public WeaponDataRa weaponDataRa;
}

[Serializable]
public class PlayerDataTf
{
    public string playerId;
    public float posX;
    public float posY;
    public float posZ;
    public float rotateX;
    public float rotateY;
    public float rotateZ;
    public float rotateW;

    public PlayerDataTf(Vector3 pos, Quaternion rot)
    {
        this.playerId = PlayerDataCenter.Instance.GetPlayerId();
        this.posX = pos.x;
        this.posY = pos.y;
        this.posZ = pos.z;
        this.rotateX = rot.x;
        this.rotateY = rot.y;
        this.rotateZ = rot.z;
        this.rotateW = rot.w;
    }
}

[Serializable]
public class WeaponDataRa
{
    public string playerId;
    public FirearmType firearmType;
    public float rotateX;
    public float rotateY;
    public float rotateZ;
    public float rotateW;

    public WeaponDataRa(string weaponName, Quaternion rot)
    {
        this.playerId = PlayerDataCenter.Instance.GetPlayerId();
        switch (weaponName)
        {
            case "Weapon":
                this.firearmType = FirearmType.Weapon;
                break;
            case "Weapon1":
                this.firearmType = FirearmType.Weapon1;
                break;
            case "Weapon2":
                this.firearmType = FirearmType.Weapon2;
                break;
            default:
                break;
        }
        this.rotateX = rot.x;
        this.rotateY = rot.y;
        this.rotateZ = rot.z;
        this.rotateW = rot.w;
    }
}

[Serializable]
public class SendStartData
{
    public PlayerData playerData;
    public Dictionary<int, EnemyDataTf> starDic = new Dictionary<int, EnemyDataTf>();
    public Dictionary<int, EnemyDataTf> enemyPlaneDic = new Dictionary<int, EnemyDataTf>();
    public Dictionary<int, EnemyDataTf> guidedMissileDic = new Dictionary<int, EnemyDataTf>();
    public Dictionary<int, EnemyDataTf> gemDic = new Dictionary<int, EnemyDataTf>();
}

[Serializable]
public class ReceiveStartData
{
    public PlayerDataList list;
    public Dictionary<int, EnemyDataTf> starDic = new Dictionary<int, EnemyDataTf>();
    public Dictionary<int, EnemyDataTf> enemyPlaneDic = new Dictionary<int, EnemyDataTf>();
    public Dictionary<int, EnemyDataTf> guidedMissileDic = new Dictionary<int, EnemyDataTf>();
    public Dictionary<int, EnemyDataTf> gemDic = new Dictionary<int, EnemyDataTf>();
}

[Serializable]
public class EnemyDataTf
{
    public int id;
    public float posX;
    public float posY;

    public EnemyDataTf(int id, Vector2 pos)
    {
        this.id = id;
        this.posX = pos.x;
        this.posY = pos.y;
    }
}

[Serializable]
public class WeaponData
{
    public string PlayerId;           
    public FirearmType firearmType; 
    public int damage; 

    public WeaponData(int type)
    {
        this.PlayerId = PlayerDataCenter.Instance.GetPlayerId();
        this.firearmType = (FirearmType)type;
        this.damage = 20;
    }
}

public enum FirearmType
{
    Weapon = 0,
    Weapon1 = 1,
    Weapon2 = 2,
}

[Serializable]
public class StarData
{
    public int StarId;
    public int damage;
}

[Serializable]
public class EnemyPlaneData
{
    public int EnemyPlaneId;
    public int damage;
}

[Serializable]
public class GuidedMissileData
{
    public int GuidedMissileId;
}

[Serializable]
public class EnemyShootData
{
    public int enemyId;
    public int damage;
}

