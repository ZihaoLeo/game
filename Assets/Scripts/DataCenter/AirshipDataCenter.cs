using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipDataCenter : Singleton<AirshipDataCenter>
{
    private Dictionary<string, WeaponData> weaponDic = new Dictionary<string, WeaponData>();
    private WeaponData weaponData;
    // Speed: Moving 5 units of length per second
    public float moveSpeed = 3f;
    public int weaponEnchantDamage = 0; // Weapon enchant damage
    /// <summary>
    /// Initializes the weapon data
    /// </summary>
    public void Init()
    {
        WeaponData weaponData;
        for (int i = 0; i < 3; i++)
        {
            weaponData = new WeaponData(i);
            weaponDic.Add(weaponData.firearmType.ToString(), weaponData);
        }
        AddMessage();
    }

    void AddMessage()
    {
        DynamicDataCenter.AddMessage(EmDataType.EmSyncIncreaseFitness, OnEmSyncIncreaseFitness);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncIncreasedWeaponDamage, OnEmSyncIncreasedWeaponDamage);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncIncreasedWeaponEnchantment, OnEmSyncIncreasedWeaponEnchantment);
        DynamicDataCenter.AddMessage(EmDataType.EmSyncIncreaseShipSpeed, OnEmSyncIncreaseShipSpeed);
    }

    private void OnEmSyncIncreaseFitness(object[] paras)
    {
        BattleManager.Instance.SetShipIntegrityMaxNum(50);
        BattleManager.Instance.ReducFunds(100);
    }

    private void OnEmSyncIncreasedWeaponDamage(object[] paras)
    {
        foreach (var item in weaponDic.Values)
        {
            item.damage += 10;
        }
        BattleManager.Instance.ReducFunds(100);
    }

    private void OnEmSyncIncreasedWeaponEnchantment(object[] paras)
    {
        weaponEnchantDamage += 15;
        BattleManager.Instance.ReducFunds(100);
    }

    private void OnEmSyncIncreaseShipSpeed(object[] paras)
    {
        moveSpeed += 2;
        BattleManager.Instance.ReducFunds(100);
    }

    public void SetNowWeapon(string key)
    {
        if (weaponDic.ContainsKey(key))
            weaponData = weaponDic[key];
    }

    public WeaponData GetNowWeapon()
    {
        return weaponData;
    }

    public void RemoveNowWeapon()
    {
        weaponData = null;
    }

    public int GetWeaponDamage()
    {
        foreach (var item in weaponDic.Values)
        {
            return item.damage;
        }
        return 0;
    }
}
