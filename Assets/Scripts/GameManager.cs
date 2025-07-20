using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public ReplayManager replayManager;
    public PlayerController player;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject inGamePanel;
    public GameObject deathPanel;

    [Header("Player Spawn")]
    public Transform playerStartPosition;

    [Header("Score")]
    public TMP_Text scoreText;
    private float score = 0f;
    private bool isCounting = false;

    private bool isRunActive = false;

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        if (isCounting)
        {
            score += Time.deltaTime * 10f;
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    public void ShowMainMenu()
    {
        isRunActive = false;
        player.isControllable = false;
        mainMenuPanel.SetActive(true);
        inGamePanel.SetActive(false);
        deathPanel.SetActive(false);
    }

    public void StartRun()
    {
        isRunActive = true;
        isCounting = true;
        score = 0f;

        replayManager.StartRecording();
        player.isDead = false;
        player.isControllable = true;
        ResetPlayer();

        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(true);
        deathPanel.SetActive(false);
    }

    public void EndRun()
    {
        if (!isRunActive) return;

        isRunActive = false;
        isCounting = false;

        player.isControllable = false;
        replayManager.StopRecordingAndSave();

        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(false);
        deathPanel.SetActive(true);
    }

    public void OnReplayButtonClicked()
    {
        deathPanel.SetActive(false);
        replayManager.StartReplay();
    }

    public void OnRestartButtonClicked()
    {
        StartRun();
    }

    public void OnMainMenuButtonClicked()
    {
        ShowMainMenu();
    }

    private void ResetPlayer()
    {
        player.transform.position = playerStartPosition.position + new Vector3(0f, 5f, 0f);
        player.transform.rotation = playerStartPosition.rotation;
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.isDead = false;
    }

    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
