using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class EnemyPlaneController : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet preform
    public Transform firePoint; // Muzzle position
    [HideInInspector]
    public Transform bulletParent; // Bullet parent
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public int enemyPlaneId;
    private int health;
    private float speed = 1f;
    EnemyPlaneData enemyPlaneData;

    SpriteRenderer spriteRenderer;
    string spriteName = null;
    private EnemyShootData enemyShootData;
    private int timeId;

    private void Start()
    {
        DynamicDataCenter.AddMessage(EmDataType.EmSyncPlaneShootData, OnEmSyncPlaneShootData);
    }

    private void OnEmSyncPlaneShootData(object[] paras)
    {
        if (isDie)
            return;
        EnemyShootData shootData = (EnemyShootData)paras[0];
        if (shootData != null)
        {
            // Debug.LogError("enemyPlaneId:" + enemyPlaneId + "  enemyId:" + shootData.enemyId);
            if (enemyPlaneId == shootData.enemyId)
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
        player = airship;
        bulletParent = parent;
        enemyPlaneId = i;
        health = 100;
        int index = i % 5;
        spriteRenderer = transform.Find("Warcraft").GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteName = "Art/Warcraft/" + index;
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
        enemyShootData.enemyId = enemyPlaneId;
        enemyShootData.damage = 5;
    }

    float syncAttackTime;
    float distanceToPlayer;
    float fireangle;//Emission Angle
    Vector3 gunPos;
    Vector2 targetDir;
    Vector2 direction;
    void Update()
    {
        // Calculate the distance between the enemy and the player
        distanceToPlayer = Vector2.Distance(player.position, transform.position);
        //Debug.LogError("distanceToPlayer:" + distanceToPlayer + "  " + BattleManager.Instance.planeShootLimit);

        Attack();
        RotateTurret();
    }
    void Attack()
    {
        // If the distance exceeds the limit, do not track
        if (distanceToPlayer > BattleManager.Instance.planeShootLimit)
        {
            return;
        }
        if (syncAttackTime >= 5f)
        {
            SocketUdpClientManager.Instance.SendMessage(MessageType.SyncPlaneShootData, enemyShootData);
            syncAttackTime = 0;
        }
        syncAttackTime += Time.fixedDeltaTime;
    }

    void RotateTurret()
    {
        // If the distance exceeds the limit, do not track
        if (distanceToPlayer > BattleManager.Instance.chaseThreshold)
        {
            return;
        }
        // Object location
        gunPos = this.transform.position;
        float fireangle;// Emission Angle
                        // Calculates the Angle between the mouse position and the object position
        targetDir = player.position - gunPos;
        fireangle = Vector2.Angle(targetDir, Vector3.up);
        if (player.position.x > gunPos.x)
        {
            fireangle = -fireangle;
        }
        transform.eulerAngles = new Vector3(0, 0, fireangle);

        if (distanceToPlayer <= BattleManager.Instance.starShootLimit)
        {
            return;
        }
        // Calculate the direction towards the player£¨2D£©
        direction = player.position - transform.position;
        // Move enemies and ignore the Z-axis
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        // Make sure the enemy's Z coordinate is always 0
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
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
        // Debug.LogError(collider.transform.name);

        // Check to see if it was a bullet that hit the enemy
        if (collider.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(AirshipDataCenter.Instance.GetWeaponDamage());
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
        BattleManager.Instance.Addunds(100);
        // You can add animations, sounds, etc. of enemies dying here
        Destroy(gameObject); // Destroy enemy game objects
    }
}
