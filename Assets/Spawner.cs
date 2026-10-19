using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Coin Prefabs")]
    [SerializeField] private GameObject normalCoinPrefab;
    [SerializeField] private GameObject redCoinPrefab;
    [SerializeField] private GameObject blueCoinPrefab;

    [Header("References")]
    [Tooltip("Assign Blockout/Coins here. Coins must be parented to the Blockout so they tilt with the floor.")]
    [SerializeField] private Transform coinsParent;
    [Tooltip("Assign Blockout/Plane here. Every coin type spawns inside this floor's bounds.")]
    [SerializeField] private Transform floor;

    [Header("Floor Spawn Bounds (local to floor)")]
    [SerializeField] private Vector2 floorHalfExtents = new Vector2(4.5f, 4.5f);
    [SerializeField] private float spawnHeight = 0.5f;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1.5f;

    private Coroutine normalCoinRoutine;
    private Coroutine blueCoinRoutine;

    private void Start()
    {
        StartNormalCoinSpawning();
    }

    // --- Normal coins: spawn continuously until the Red Coin threshold is reached ---

    public void StartNormalCoinSpawning()
    {
        if (normalCoinRoutine != null) return;
        normalCoinRoutine = StartCoroutine(SpawnNormalCoinsRoutine());
    }

    public void StopNormalCoinSpawning()
    {
        if (normalCoinRoutine == null) return;
        StopCoroutine(normalCoinRoutine);
        normalCoinRoutine = null;
    }

    private IEnumerator SpawnNormalCoinsRoutine()
    {
        while (true)
        {
            SpawnOnFloor(normalCoinPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // --- Red coin: spawns exactly once, called by GameManager when score hits the threshold ---

    public void SpawnRedCoin()
    {
        SpawnOnFloor(redCoinPrefab);
    }

    // --- Blue coins: spawn one at a time, every spawnInterval seconds, until `count` is reached ---

    public void StartBlueCoinSpawning(int count)
    {
        if (blueCoinRoutine != null)
            StopCoroutine(blueCoinRoutine);

        blueCoinRoutine = StartCoroutine(SpawnBlueCoinsRoutine(count));
    }

    private IEnumerator SpawnBlueCoinsRoutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnOnFloor(blueCoinPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }
        blueCoinRoutine = null;
    }

    // --- Shared spawn logic: every coin type lands at a random point inside the floor's bounds,
    // computed in the floor's LOCAL space so it's always correct regardless of current tilt. ---

    private void SpawnOnFloor(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Spawner: a coin prefab slot is not assigned.");
            return;
        }
        if (floor == null)
        {
            Debug.LogWarning("Spawner: Floor reference is not assigned.");
            return;
        }

        float x = Random.Range(-floorHalfExtents.x, floorHalfExtents.x);
        float z = Random.Range(-floorHalfExtents.y, floorHalfExtents.y);
        Vector3 localPos = new Vector3(x, spawnHeight, z);
        Vector3 worldPos = floor.TransformPoint(localPos);

        Instantiate(prefab, worldPos, Quaternion.identity, coinsParent);
    }

#if UNITY_EDITOR
    // Visual-only: shows the shared floor spawn bounds in the Scene view so you can
    // tune floorHalfExtents / spawnHeight against your actual box size.
    private void OnDrawGizmosSelected()
    {
        if (floor == null) return;

        Gizmos.color = Color.yellow;
        Vector3 p1 = floor.TransformPoint(new Vector3(-floorHalfExtents.x, spawnHeight, -floorHalfExtents.y));
        Vector3 p2 = floor.TransformPoint(new Vector3(floorHalfExtents.x, spawnHeight, -floorHalfExtents.y));
        Vector3 p3 = floor.TransformPoint(new Vector3(floorHalfExtents.x, spawnHeight, floorHalfExtents.y));
        Vector3 p4 = floor.TransformPoint(new Vector3(-floorHalfExtents.x, spawnHeight, floorHalfExtents.y));

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
#endif
}