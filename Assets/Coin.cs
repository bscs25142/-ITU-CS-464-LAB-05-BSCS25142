using UnityEngine;

public enum CoinType { Normal, Red, Blue }

// One script for all three coin prefabs - set the Coin Type in the Inspector
// on each prefab (Normal = yellow coin, Red = Red Coin, Blue = Blue bonus coin).
[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    [SerializeField] private CoinType coinType = CoinType.Normal;
    [SerializeField] private int scoreValue = 1; // only used by Normal coins

    [Header("Spin")]
    [SerializeField] private Vector3 spinAxis = Vector3.up;
    [SerializeField] private float spinSpeed = 90f; // degrees per second

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (coinType)
        {
            case CoinType.Normal:
                GameManager.Instance.AddScore(scoreValue);
                break;
            case CoinType.Red:
                GameManager.Instance.OnRedCoinCollected();
                break;
            case CoinType.Blue:
                GameManager.Instance.OnBlueCoinCollected();
                break;
        }

        Destroy(gameObject);
    }
}