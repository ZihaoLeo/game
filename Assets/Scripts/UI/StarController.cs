using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class StarController : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet preform
    public Transform firePoint; // Muzzle position
    public Transform weapon;
    [HideInInspector]
    public Transform bulletParent; // Bullet parent
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public int starId;
    private int health;
    private float speed = 1f;

    private SpriteRenderer spriteRenderer;
    private string spriteName = null;
    private StarData starData;
    private EnemyShootData enemyShootData;
    private int timeId;

    private void Start()
    {
        DynamicDataCenter.AddMessage(EmDataType.EmSyncStarShootData, OnEmSyncStarShootData);
    }

    private void OnEmSyncStarShootData(object[] paras)
    {
        if (isDie)
            return;
        EnemyShootData shootData = (EnemyShootData)paras[0];
        if (shootData != null)
        {
            if (starId == shootData.enemyId)
                Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the bullet preform
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        GameUtils.ChangeObjValue2(bullet, bulletParent);
        // Set the direction of movement of the bullet, assuming the muzzle is facing forward
        EnemyBulletController bulletMovement = bullet.GetComponent<EnemyBulletController>();
        if (bulletMovement != null)
        {
            Vector3 angle = transform.eulerAngles;
            float z = angle.z;
            if (angle.z > 180)
            {
                z = angle.z - 360f;
            }
            // Debug.LogError("Fire Point Direction: " + firePoint.up);
            bulletMovement.Initialize(firePoint.up);
        }
    }

    public void Init(Transform airship, Transform parent, int i)
    {
        starData = new StarData();
        starData.StarId = i;
        player = airship;
        bulletParent = parent;
        starId = i;
        health = 100;
        int index = i % 6;
        spriteRenderer = transform.Find("Star").GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteName = "Art/Star/" + index;
            Sprite newSprite = Resources.Load<Sprite>(spriteName);
            if (newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("Sprite not found in Resources folder: " + spriteName);
            }
        }

        enemyShootData = new EnemyShootData();
        enemyShootData.enemyId = starId;
        enemyShootData.damage = 5;
    }

    float syncAttackTime = 0;
    float fireangle;// Emission Angle
    float distanceToPlayer;
    Vector3 gunPos;
    Vector2 targetDir;
    void Update()
    {
        // Calculate the distance between the enemy and the player
        distanceToPlayer = Vector2.Distance(player.position, transform.position);
        // Debug.LogError("distanceToPlayer:" + distanceToPlayer);
        // If the distance exceeds the limit, do not track
        if (distanceToPlayer > BattleManager.Instance.starShootLimit)
        {
            syncAttackTime = 0;
            return;
        }
        Attack();
        RotateTurret();
    }

    void Attack()
    {
        if (syncAttackTime >= 5f)
        {
            SocketUdpClientManager.Instance.SendMessage(MessageType.SyncStarShootData, enemyShootData);
            syncAttackTime = 0;
        }
        syncAttackTime += Time.fixedDeltaTime;
    }

    void RotateTurret()
    {
        // Object location
        gunPos = this.transform.position;
        // Calculates the Angle between the mouse position and the object position
        targetDir = player.position - gunPos;
        fireangle = Vector2.Angle(targetDir, Vector3.up);
        if (player.position.x > gunPos.x)
        {
            fireangle = -fireangle;
        }
        weapon.eulerAngles = new Vector3(0, 0, fireangle);
    }

    bool isOpenTime = false;
    int time = 1; // 1s/1
    public void OpenTimer()
    {
        if (!isOpenTime)
        {
            isOpenTime = true;
            BattleManager.Instance.CreateFlameItem(transform);
            TimerManager.Instance.Remove(timeId);
            timeId = TimerManager.Instance.Add(CallBack, time);
        }
    }

    private void CallBack(int time)
    {
        TakeDamage(time);
        // Debug.LogError("time:" + time);
    }

    // Called when an enemy is hit by a bullet
    private void OnTriggerEnter(Collider collider)
    {
        // Check to see if it was a bullet that hit the enemy
        if (collider.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(AirshipDataCenter.Instance.GetWeaponDamage());
        }
        else if (collider.gameObject.CompareTag("Airship"))
        {
            BattleManager.Instance.AirshipMove(transform);
        }
    }
    bool isDie = false;
    // Deal with enemy damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        // If Health drops to 0 or below, destroy enemies
        if (health <= 0 && !isDie)
        {
            Die();
        }
    }

    // The way the enemy dies
    private void Die()
    {
        isDie = true;
        BattleManager.Instance.Addunds(150);
        TimerManager.Instance.Remove(timeId);
        // You can add animations, sounds, etc. of enemies dying here
        Destroy(gameObject); // Destroy enemy game objects
    }
}
