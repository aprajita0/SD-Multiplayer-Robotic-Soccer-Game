using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance;

    public int blueTeamScore = 0;
    public int redTeamScore = 0;

    public float gameDuration = 240f;
    private float timer;
    private bool gameEnded = false;

    public TMP_Text timerText;
    public TMP_Text blueTeamScoreText;
    public TMP_Text metalTeamScoreText;

    public GameObject ball;
    public Transform ballStartPosition;

    public GameObject[] players;
    public Transform[] playerStartPositions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        timer = gameDuration;
        UpdateAllUI();
    }

    private void Update()
    {
        if (!gameEnded)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                UpdateTimerUI();
                gameEnded = true;
                SceneManager.LoadScene("GameOverScene");
            }
            else
            {
                UpdateTimerUI();
            }
        }
    }

    public void AddScore(string team)
    {
        if (team == "BlueTeamGoal")
        {
            redTeamScore++; // Metal team scored
        }
        else if (team == "RedTeamGoal")
        {
            blueTeamScore++;
        }

        UpdateScoreUI();
        Debug.Log($"Score Updated: Blue={blueTeamScore}, Metal={redTeamScore}");
    }

    public void ResetAfterGoal()
    {
        if (ball != null && ballStartPosition != null)
        {
            ball.transform.position = ballStartPosition.position;
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
        }

        for (int i = 0; i < players.Length && i < playerStartPositions.Length; i++)
        {
            players[i].transform.position = playerStartPositions[i].position;
            players[i].transform.rotation = playerStartPositions[i].rotation;

            Rigidbody rb = players[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        UpdateScoreUI();
    }

    public void UpdateAllUI()
    {
        UpdateTimerUI();
        UpdateScoreUI();
    }

    private void UpdateTimerUI()
    {
        float displayTime = Mathf.Max(timer, 0f);
        int minutes = Mathf.FloorToInt(displayTime / 60f);
        int seconds = Mathf.FloorToInt(displayTime % 60f);
        if (timerText != null)
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void UpdateScoreUI()
    {
        if (blueTeamScoreText != null)
    {
        blueTeamScoreText.text = $"Blue Team: {blueTeamScore}";
    }
    else
    {
        Debug.LogWarning("BlueTeamScoreText is null!");
    }

    if (metalTeamScoreText != null)
    {
        metalTeamScoreText.text = $"Metal Team: {redTeamScore}";
    }
    else
    {
        Debug.LogWarning("MetalTeamScoreText is null!");
    }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    timerText = GameObject.Find("Timer")?.GetComponent<TMP_Text>();
    blueTeamScoreText = GameObject.Find("BlueTeamScore")?.GetComponent<TMP_Text>();
    metalTeamScoreText = GameObject.Find("MetalTeamScore")?.GetComponent<TMP_Text>();
    TMP_Text winnerText = GameObject.Find("Winner")?.GetComponent<TMP_Text>();

    if (winnerText != null)
    {
        if (blueTeamScore > redTeamScore)
        {
            winnerText.text = "WINNER: Blue Team!";
        }
        else if (redTeamScore > blueTeamScore)
        {
            winnerText.text = "WINNER: Metal Team!";
        }
        else
        {
            winnerText.text = "WINNER: It's a tie!";
        }
    }

    UpdateAllUI();
}

}
