using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SheepSpawner sheepSpawner;

    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text timerText;
    public GameObject pauseText;
    public GameObject gameOverText;

    public float matchDuration = 60f;
    public int comboThreshold = 3;
    public int comboMultiplier = 2;

    private int score;
    private int consecutiveHits;
    private float timeRemaining;
    private bool isPaused;
    private bool isGameOver;

    public bool IsPaused => isPaused;
    public bool IsGameOver => isGameOver;

    private void Awake(){
        Instance = this;
    }

    private void Start(){
        Time.timeScale = 1f;
        timeRemaining = matchDuration;

        if (pauseText != null){
            pauseText.SetActive(false);
        }

        if (gameOverText != null){
            gameOverText.SetActive(false);
        }

        RefreshUI();
    }

    private void Update(){
        if (isGameOver){
            if (Input.GetKeyDown(KeyCode.R)){
                RestartGame();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.P)){
            TogglePause();
        }

        if (isPaused){
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f){
            timeRemaining = 0f;
            EndGame();
        }

        RefreshUI();
    }

    public void OnSheepSaved(int basePoints){
        if (isGameOver){
            return;
        }

        consecutiveHits++;

        int awardedPoints = basePoints;

        if (consecutiveHits >= comboThreshold){
            awardedPoints *= comboMultiplier;
        }

        score += awardedPoints;
        RefreshUI();
    }

    public void OnSheepDropped(){
        if (isGameOver){
            return;
        }

        consecutiveHits = 0;
        RefreshUI();
    }

    private void TogglePause(){
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseText != null){
            pauseText.SetActive(isPaused);
        }
    }

    private void EndGame(){
        isGameOver = true;
        Time.timeScale = 0f;

        if (sheepSpawner != null){
            sheepSpawner.canSpawn = false;
            sheepSpawner.DestroyAllSheep();
        }

        if (gameOverText != null){
            gameOverText.SetActive(true);
        }

        RefreshUI();
    }

    private void RefreshUI(){
        if (scoreText != null){
            scoreText.text = "Score: " + score;
        }

        if (comboText != null)
        {
            if (consecutiveHits >= comboThreshold){
                comboText.text = "Combo: x" + comboMultiplier;
            }
            else{
                comboText.text = "Combo: " + consecutiveHits;
            }
        }

        if (timerText != null){
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
        }
    }

    private void RestartGame(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}