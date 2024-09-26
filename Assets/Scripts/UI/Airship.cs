using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    private float h;
    private float v;
    private float newX;
    private float newY;
    private Vector3 rotation;
    private float syncPlayerTransTime = 0;
    private float currentAngle; // The rotation Angle of the current battery
    public float turnSpeed = 90f; // The Angle of rotation per second

    void Start()
    {
        currentAngle = transform.eulerAngles.y;
    }


    void Update()
    {
        if (!BattleManager.Instance.IsMoveSpaceship())
            return;
        ChangeTransform();
        // Check if the A key is pressed
        if (Input.GetKey(KeyCode.A))
        {
            RotateTurret(turnSpeed * Time.deltaTime);
        }
        // Check if the D key is pressed
        else if (Input.GetKey(KeyCode.D))
        {
            RotateTurret(-turnSpeed * Time.deltaTime);
        }
    }

    void ChangeTransform()
    {
        // Gets the value of the vertical axis, in the range -1 to 1, for forward/backward
        float moveAmount = Input.GetAxis("Vertical") * AirshipDataCenter.Instance.moveSpeed * Time.deltaTime;
        // Debug.LogError("moveAmount:" + moveAmount);
        // Use transform.right to get the current forward direction of the ship and multiply by the amount of movement to get the movement vector
        Vector3 moveDirection = transform.right * moveAmount;

        // Calculate the position after the move
        Vector3 newPosition = transform.position + moveDirection;

        // Use Mathf.Clamp to limit the range of movement
        newPosition.x = Mathf.Clamp(newPosition.x, PlayerManager.Instance.minX, PlayerManager.Instance.maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, PlayerManager.Instance.minY, PlayerManager.Instance.maxY);
        // Set the new position, leaving the Z-axis position unchanged
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        syncPlayerTransTime += Time.fixedDeltaTime;
        if (syncPlayerTransTime >= 0.05)
        {
            SocketUdpClientManager.Instance.SendMessage(MessageType.SyncAirshipData, new PlayerDataTf(transform.localPosition, transform.localRotation));
            syncPlayerTransTime = 0;
        }
    }

    void RotateTurret(float angle)
    {
        v = Input.GetAxis("Vertical") * AirshipDataCenter.Instance.moveSpeed * Time.deltaTime;
        // Update current Angle
        currentAngle += angle;
        // Application rotation
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
