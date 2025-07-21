using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public ReplayManager replayManager;  // Reference to the ReplayManager
    public PlayerController player;      // Reference to the player

    [Header("UI Panels")]
    public GameObject mainMenuPanel;     // Main Menu UI panel
    public GameObject inGamePanel;       // In-Game UI panel
    public GameObject deathPanel;        // Death UI panel

    [Header("Player Spawn")]
    public Transform playerStartPosition;  // The starting position for the player

    [Header("Score")]
    public TMP_Text scoreText;     // UI text displaying the score
    private float score = 0f;      // Player's score
    private bool isCounting = false; // Flag to determine if score should be increasing

    private bool isRunActive = false; // Indicates if a run is currently active

    void Start()
    {
        ShowMainMenu(); // Show main menu on game start
    }

    void Update()
    {
        // Update score during active run
        if (isCounting)
        {
            score += Time.deltaTime * 10f; // Increase score over time
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    // Displays the main menu UI
    public void ShowMainMenu()
    {
        isRunActive = false;
        player.isControllable = false;

        mainMenuPanel.SetActive(true);
        inGamePanel.SetActive(false);
        deathPanel.SetActive(false);
    }

    // Called when starting a new run
    public void StartRun()
    {
        isRunActive = true;
        isCounting = true;
        score = 0f;

        replayManager.StartRecording(); // Begin recording replay
        player.isDead = false;
        player.isControllable = true;
        ResetPlayer();

        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(true);
        deathPanel.SetActive(false);
    }

    // Called when the run ends (e.g., on player death)
    public void EndRun()
    {
        if (!isRunActive) return;

        isRunActive = false;
        isCounting = false;

        player.isControllable = false;
        replayManager.StopRecordingAndSave(); // Save replay data

        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(false);
        deathPanel.SetActive(true);
    }

    // Triggered by the replay button on the death screen
    public void OnReplayButtonClicked()
    {
        deathPanel.SetActive(false);
        replayManager.StartReplay(); // Begin replaying saved run
    }

    // Triggered by the restart button on the death screen
    public void OnRestartButtonClicked()
    {
        StartRun(); // Restart a new run
    }

    // Triggered by the main menu button
    public void OnMainMenuButtonClicked()
    {
        ShowMainMenu();
    }

    // Resets player to the start position and resets physics
    private void ResetPlayer()
    {
        player.transform.position = playerStartPosition.position + new Vector3(0f, 5f, 0f); // Slight vertical offset
        player.transform.rotation = playerStartPosition.rotation;
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Reset velocity
        player.isDead = false;
    }

    // Exits the game or stops play mode in the editor
    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quits the application if built
#endif
    }
}
