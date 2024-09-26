using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoSingleton<PlayerController>
{
    public Animator m_Animator = null;// animation
    public Text tipsText;

    private float m_MoveTime = 0;
    private float m_MoveSpeed = 0.0f;
    // Boundary constraint
    private float minX = 0;
    private float maxX = 1.1f;
    private float minY = 0;
    private float maxY = 0.6f;
    private float newX;
    private float newY;
    private Vector3 rotation;
    // Speed: Moving 5 units of length per second
    private float moveSpeed = 1f;
    private float h;
    private float v;
    private string nowTag;
    private string playerId;
    private float syncPlayerTransTime = 0;

    void Start()
    {
    }

    void Update()
    {
        if (nowTag != null && nowTag.Length > 0 && Input.GetKeyDown(KeyCode.E))
            ControlBattery();
        if (BattleManager.Instance.PlayerIsMove())
            ChangeTransform();
        else
            m_Animator.SetBool("isMove", false);
    }

    void ControlBattery()
    {
        switch (nowTag)
        {
            case "Weapon":
            case "Weapon1":
            case "Weapon2":
                if (BattleManager.Instance.weaponType == SpaceshipType.None)
                {
                    BattleManager.Instance.SetWeaponType(SpaceshipType.ControlBattery);
                    tipsContent = "Press E to give up control of the battery";
                }
                else
                {
                    BattleManager.Instance.ResetWeaponType();
                    tipsContent = "Press E to control the battery";
                }
                BattleManager.Instance.ChangeMainCamera(nowTag);
                break;
            case "Rudder":
                if (BattleManager.Instance.weaponType == SpaceshipType.None)
                {
                    BattleManager.Instance.SetWeaponType(SpaceshipType.FlySpaceship);
                    PlayerManager.Instance.SetBoundary(false);
                    tipsContent = "Press E to abandon control of the rudder";
                }
                else
                {
                    BattleManager.Instance.ResetWeaponType();
                    PlayerManager.Instance.SetBoundary();
                    tipsContent = "Press E to control the rudder";
                }
                BattleManager.Instance.ChangeBattleScene();
                break;
            case "RepairShip":
                if (BattleManager.Instance.weaponType == SpaceshipType.None)
                {
                    BattleManager.Instance.SetWeaponType(SpaceshipType.RepairShip);
                    BattleManager.Instance.OpenTimer();
                    tipsContent = "Press E to stop repairing the ship";
                }
                else
                {
                    BattleManager.Instance.RemoveTimer();
                    BattleManager.Instance.ResetWeaponType();
                    tipsContent = "Press E to repair the ship";
                }
                break;
            default:
                break;
        }
        tipsText.text = tipsContent;
    }

    public void SetTipsText(string tipsContent)
    {
        tipsText.text = tipsContent;
    }

    void ChangeTransform()
    {
        // Gets the values for the vertical and horizontal axes in the range -1 to 1
        h = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        v = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        bool isMove = ((h != 0) || (v != 0));

        // Calculate the position after the move
        newX = transform.localPosition.x + h;
        newY = transform.localPosition.y + v;

        // Use Mathf.Clamp to limit the range of movement
        newX = Mathf.Clamp(newX, minX, maxX);
        newY = Mathf.Clamp(newY, minY, maxY);
        // Debug.LogError("newX:" + newX + "  newY:" + newY);
        //if (h >= 0)
        //    rotation = new Vector3(0, 0, 0);
        //else
        //    rotation = new Vector3(0, 180, 0);
        // transform.rotation = Quaternion.Euler(rotation);
        // Set new location
        transform.localPosition = new Vector3(newX, newY, 0);

        m_MoveTime = isMove ? (m_MoveTime + Time.deltaTime) : 0;
        // Calculated direction of movement
        m_Animator.SetBool("isMove", isMove);
        syncPlayerTransTime += Time.fixedDeltaTime;
        if (syncPlayerTransTime >= 0.05)
        {
            SocketUdpClientManager.Instance.SendMessage(MessageType.SyncPlayerData, new PlayerDataTf(transform.localPosition, transform.localRotation));
            syncPlayerTransTime = 0;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Debug.LogError(collider.gameObject.name);
        if (IsTrigger())
        {
            SetAirshipDataByTag(collider.gameObject.tag);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (IsTrigger())
        {
            AirshipDataCenter.Instance.RemoveNowWeapon();
            nowTag = null;
            tipsText.gameObject.SetActive(false);
            // Debug.LogError("leave" + collider.gameObject.tag);
        }
    }
    string tipsContent;
    void SetAirshipDataByTag(string tag)
    {
        switch (tag)
        {
            case "Weapon":
            case "Weapon1":
            case "Weapon2":
                if (BattleManager.Instance.IsControlBattery())
                {
                    AirshipDataCenter.Instance.SetNowWeapon(tag);
                    tipsContent = "Press E to control the battery";
                    nowTag = tag;
                }
                break;
            case "Rudder":
                if (BattleManager.Instance.IsFlySpaceship())
                {
                    tipsContent = "Press E to control the rudder";
                    nowTag = tag;
                }
                break;
            case "RepairShip":
                if (BattleManager.Instance.IsRepairShip())
                {
                    tipsContent = "Press E to repair the ship";
                    nowTag = tag;
                }
                break;
            default:
                break;
        }
        tipsText.text = tipsContent;
        tipsText.gameObject.SetActive(nowTag != null);
        // Debug.LogError("enter into" + collider.gameObject.tag);
    }
    bool IsTrigger()
    {
        return playerId == PlayerDataCenter.Instance.GetPlayerId();
    }

    public void SetPlayerId(string id)
    {
        playerId = id;
    }
}
