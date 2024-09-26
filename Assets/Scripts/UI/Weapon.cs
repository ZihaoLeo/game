using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet preform
    public Transform firePoint; // Muzzle position
    public Transform bulletParent; // Bullet parent
    public string weaponName;

    private float bulletSpeed = 10f; // Bullet velocity
    public float turnSpeed = 90f; // The Angle of rotation per second
    public float minTurnAngle = -30f; // Minimum rotation Angle
    public float maxTurnAngle = 30f; // Maximum rotation Angle
    private float currentAngle; // The rotation Angle of the current battery

    private Ray MouseRay;
    private RaycastHit MouseRaycastHit;

    private void Start()
    {
        // Initialize the current Angle as the initial rotation Angle of the battery
        currentAngle = transform.eulerAngles.y;

        DynamicDataCenter.AddMessage(EmDataType.EmSyncShoot, OnEmSyncShoot);
    }

    private void OnEmSyncShoot(object[] paras)
    {
        WeaponData weaponData = (WeaponData)paras[0];
        if (weaponData != null)
        {
            if (weaponData.firearmType.ToString() == weaponName)
                Shoot();
        }
    }

    void Update()
    {
        if (!BattleManager.Instance.IsAttack(weaponName))
            return;
        // Debug.LogError("time:" + Time.time);
        // Detect if the launch button is pressed, such as the left mouse button
        if (Input.GetMouseButtonDown(0)) // 0 Indicates the left mouse button
        {
            // Debug.LogError("fire a bullet£ºÖ¸:" + Time.time);
            SocketTcpClientManager.Instance.SendMessage(MessageType.SyncShoot, AirshipDataCenter.Instance.GetNowWeapon());
        }
        RotateTurret();
    }
    float syncPlayerTransTime = 0;
    void RotateTurret()
    {
        Vector3 ms = Input.mousePosition;
        ms = Camera.main.ScreenToWorldPoint(ms);//Get relative mouse position
                                                //Object location
        Vector3 gunPos = this.transform.position;
        float fireangle;//Emission Angle
        //Calculates the Angle between the mouse position and the object position
        Vector2 targetDir = ms - gunPos;
        fireangle = Vector2.Angle(targetDir, Vector3.up);
        if (ms.x > gunPos.x)
        {
            fireangle = -fireangle;
        }
        this.transform.eulerAngles = new Vector3(0, 0, fireangle);
        syncPlayerTransTime += Time.fixedDeltaTime;
        if (syncPlayerTransTime >= 0.05)
        {
            SocketUdpClientManager.Instance.SendMessage(MessageType.SyncWeaponData, new WeaponDataRa(weaponName, transform.localRotation));
            syncPlayerTransTime = 0;
        }
    }

    void Shoot()
    {
        // Instantiate the bullet preform
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        GameUtils.ChangeObjValue2(bullet, bulletParent);
        // Set the direction of movement of the bullet, assuming the muzzle is facing forward
        BulletController bulletMovement = bullet.GetComponent<BulletController>();
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
}
