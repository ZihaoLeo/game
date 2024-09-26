using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    [HideInInspector]
    public float speed = 2f; // Velocity of a bullet
    [HideInInspector]
    public float lifeTime = 10f; // The time of the bullet, after which it will be destroyed
    private Vector3 direction; // The direction the bullet was moving

    // Called when an enemy is hit by a bullet
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Airship")
        {
            BattleManager.Instance.ReduceShipIntegrity(1);
            // Destroy the bullet itself
            Destroy(gameObject);
        }
        else if(collider.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
    }

    // Initializes the firing direction and life cycle of the bullet
    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized; // Set the direction of motion of the bullet
        Destroy(gameObject, lifeTime); // Set the life cycle and destroy the bullet
    }

    void Update()
    {
        // Gets the current location
        Vector3 currentPosition = transform.position;
        // update location
        currentPosition += direction * speed * Time.deltaTime;
        // Apply new location
        transform.position = currentPosition;
        // Set the orientation according to the direction
        SetBulletRotation(direction);
    }

    void SetBulletRotation(Vector3 direction)
    {
        // Computed target rotation
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = targetRotation;
    }
}
