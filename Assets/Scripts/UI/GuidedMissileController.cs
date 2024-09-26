using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissileController : MonoBehaviour
{
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public int guidedMissileId;
    private float speed = 1f;
    GuidedMissileData guidedMissileData;

    SpriteRenderer spriteRenderer;
    string spriteName = null;
    public void Init(Transform airship, int i)
    {
        player = airship;
        guidedMissileData = new GuidedMissileData();
        guidedMissileData.GuidedMissileId = i;
        int index = i % 8;

        spriteRenderer = transform.Find("Guided").GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteName = "Art/Guided/" + index;
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
        else
        {
            Debug.LogError("SpriteRenderer component not found on the GameObject.");
        }
    }

    float fireangle;
    float distanceToPlayer;
    Vector3 gunPos;
    Vector2 direction;
    Vector2 targetDir;
    void Update()
    {
        // Calculate the distance between the enemy and the player
        distanceToPlayer = Vector2.Distance(player.position, transform.position);
        // Debug.LogError("distanceToPlayer:" + distanceToPlayer + "  " + BattleManager.Instance.chaseThreshold);
        // If the distance exceeds the limit, do not track
        if (distanceToPlayer > BattleManager.Instance.chaseThreshold)
        {
            return;
        }

        // Calculate the direction towards the player (2D)
        direction = player.position - transform.position;

        // Move enemies and ignore the Z-axis
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Object location
        gunPos = this.transform.position;
        // Calculates the Angle between the mouse position and the object position
        targetDir = player.position - gunPos;
        fireangle = Vector2.Angle(targetDir, Vector3.up);
        if (player.position.x > gunPos.x)
        {
            fireangle = -fireangle;
        }
        transform.eulerAngles = new Vector3(0, 0, fireangle);

        // Make sure the enemy's Z coordinate is always 0
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
    }

    bool isDie = false;
    private void OnTriggerEnter(Collider collider)
    {
        if (!isDie)
        {
            // Check to see if it was a bullet that hit the enemy
            if (collider.gameObject.CompareTag("Bullet"))
            {
                isDie = true;
                Destroy(gameObject); // Destroy enemy game objects
            }
            else if (collider.gameObject.CompareTag("Airship"))
            {
                BattleManager.Instance.ReduceShipIntegrity(2);
                isDie = true;
                Destroy(gameObject); // Destroy enemy game objects
            }
        }
    }
}
