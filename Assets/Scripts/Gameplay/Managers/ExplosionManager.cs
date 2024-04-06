using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject explosionPrefab;

    public void TriggerExplosion(Vector2 location)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, location, Quaternion.identity);
        explosionInstance.transform.position = location;

        explosionInstance.GetComponent<ExplosionEffectController>().PlayExplosion();
    }
}
