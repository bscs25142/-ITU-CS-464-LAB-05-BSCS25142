using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text bonusText;

    [Header("References")]
    [SerializeField] private Spawner spawner;

    [Header("Bonus Coin Settings")]
    [SerializeField] private int bonusCoinTotal = 50;

    private int score = 0;
    private float elapsedTime = 0f;
    private int redCoinThreshold;
    private bool redCoinSpawned = false;

    private int bonusCollected = 0;
    private bool bonusRoundActive = false;

    private void Awake()
    {
        // Simple singleton so Coin.cs can call GameManager.Instance from anywhere
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Decide, once, which score value (1-10) will trigger the Red Coin
        redCoinThreshold = Random.Range(1, 11); // Random.Range max is exclusive for ints -> 1..10 inclusive

        UpdateScoreUI();
        UpdateTimeUI();

        if (bonusText != null)
            bonusText.gameObject.SetActive(false);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimeUI();
    }

    /// <summary>Called by normal (yellow) coins.</summary>
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();

        if (!redCoinSpawned && score >= redCoinThreshold)
        {
            redCoinSpawned = true;
            spawner.StopNormalCoinSpawning(); // board settles so the Red Coin stands out
            spawner.SpawnRedCoin();
        }
    }

    /// <summary>Called by Coin.cs when the Red Coin is collected.</summary>
    public void OnRedCoinCollected()
    {
        bonusCollected = 0;
        bonusRoundActive = true;

        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(true);
            bonusText.text = $"BonusCoins: {bonusCollected}/{bonusCoinTotal}";
        }

        spawner.StartBlueCoinSpawning(bonusCoinTotal);
    }

    /// <summary>Called by Coin.cs for each Blue Coin collected.</summary>
    public void OnBlueCoinCollected()
    {
        if (!bonusRoundActive) return;

        bonusCollected++;

        if (bonusText != null)
            bonusText.text = $"BonusCoins: {bonusCollected}/{bonusCoinTotal}";

        if (bonusCollected >= bonusCoinTotal)
        {
            bonusRoundActive = false;
            // All 50 Blue Coins collected - hook a win/celebration event here if needed
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score : {score}";
    }

    private void UpdateTimeUI()
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeText.text = $"Time : {minutes:00}:{seconds:00}";
    }
}