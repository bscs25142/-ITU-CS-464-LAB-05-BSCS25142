using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Coin Prefabs")]
    [SerializeField] private GameObject redCoinPrefab;
    [SerializeField] private GameObject blueCoinPrefab;

    [Header("References")]
    [Tooltip("Assign Blockout/Coins here. Coins must be parented to the Blockout so they tilt with the floor.")]
    [SerializeField] private Transform coinsParent;
    [Tooltip("Assign Blockout/Plane here. Used to keep the Red Coin spawn point inside the box in LOCAL space.")]
    [SerializeField] private Transform floor;

    [Header("Red Coin Spawn Bounds (local to floor)")]
    [SerializeField] private Vector2 floorHalfExtents = new Vector2(4.5f, 4.5f);
    [SerializeField] private float spawnHeight = 0.5f;

    [Header("Blue Coin Spawn")]
    [SerializeField] private float blueCoinRadius = 5.5f;

    /// <summary>Spawns exactly one Red Coin at a random point inside the box.</summary>
    public void SpawnRedCoin()
    {
        if (redCoinPrefab == null)
        {
            Debug.LogWarning("Spawner: Red Coin prefab not assigned.");
            return;
        }

        float x = Random.Range(-floorHalfExtents.x, floorHalfExtents.x);
        float z = Random.Range(-floorHalfExtents.y, floorHalfExtents.y);
        Vector3 localPos = new Vector3(x, spawnHeight, z);

        // Using the floor's local space means the point stays inside the box
        // even though the box itself rotates over time.
        Vector3 worldPos = floor != null ? floor.TransformPoint(localPos) : localPos;

        Instantiate(redCoinPrefab, worldPos, Quaternion.identity, coinsParent);
    }

    /// <summary>Spawns `count` Blue Coins in a flat circle of radius blueCoinRadius around `center`.</summary>
    public void SpawnBlueCoins(Vector3 center, int count)
    {
        if (blueCoinPrefab == null)
        {
            Debug.LogWarning("Spawner: Blue Coin prefab not assigned.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * blueCoinRadius;
            Vector3 worldPos = new Vector3(center.x + offset.x, center.y + spawnHeight, center.z + offset.y);

            Instantiate(blueCoinPrefab, worldPos, Quaternion.identity, coinsParent);
        }
    }
}