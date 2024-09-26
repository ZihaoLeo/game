using UnityEngine;

public class GemController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Airship")
        {
            BattleManager.Instance.Addunds(80);
            // Destroy enemy game objects Destroy the bullet itself
            Destroy(gameObject);
        }
    }
}
