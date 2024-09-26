using UnityEngine;
using UnityEngine.UIElements;

public class BulletController : MonoBehaviour
{
    public int damage = 10;
    public float speed = 10f; // Velocity of a bullet
    public float lifeTime = 5f; // The time of the bullet, after which it will be destroyed
    private Vector3 direction; // The direction the bullet was moving

    // Called when an enemy is hit by a bullet
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Star" || collider.gameObject.tag == "EnemyPlane" || collider.gameObject.tag == "Guided")
        {
            if (collider.gameObject.tag == "Guided")
            {
                BattleManager.Instance.Addunds(90);
            }
            if (AirshipDataCenter.Instance.weaponEnchantDamage > 0)
            {
                if (collider.gameObject.tag == "Star")
                {
                    StarController star = collider.GetComponent<StarController>();
                    star.OpenTimer();
                }
                else if (collider.gameObject.tag == "EnemyPlane")
                {
                    EnemyPlaneController enemyPlane = collider.GetComponent<EnemyPlaneController>();
                    enemyPlane.OpenTimer();
                }
            }
            
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
