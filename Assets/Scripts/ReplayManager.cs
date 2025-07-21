using UnityEngine;
using System.Collections.Generic;
using System.IO;

// This script manages the recording and playback of player movement and state
public class ReplayManager : MonoBehaviour
{
    public PlayerController player;           // Reference to the player controller
    public string fileName = "replay.json";   // Name of the file to save/load replay data
    public GameManager gameManager;           // Reference to the GameManager for UI interaction

    private List<ReplayFrame> frames = new List<ReplayFrame>();  // Recorded replay frames
    private int replayIndex = 0;              // Current index in replay playback
    private float replayTimer = 0f;           // Timer to control playback timing

    private bool isRecording = false;         // Flag to indicate if recording is active
    private bool isReplaying = false;         // Flag to indicate if replay is active

    void Update()
    {
        if (isRecording)
        {
            RecordFrame(); // Record player state every frame
        }
        else if (isReplaying)
        {
            PlayReplay(); // Play replay if active
        }
    }

    // Records the current state of the player into the frames list
    void RecordFrame()
    {
        frames.Add(new ReplayFrame(
            player.transform.position,
            player.transform.rotation,
            player.isDead,
            Time.time
        ));
    }

    // Plays back recorded frames to simulate the player's past actions
    void PlayReplay()
    {
        if (replayIndex >= frames.Count - 1)
        {
            EndReplay(); // End replay if we've reached the last frame
            return;
        }

        replayTimer += Time.deltaTime;

        // Advance the replayIndex while the recorded time is less than current time
        while (replayIndex < frames.Count - 1 && replayTimer >= frames[replayIndex + 1].time)
        {
            replayIndex++;
        }

        var frame = frames[replayIndex];

        // Apply recorded position and rotation
        player.transform.position = frame.position;
        player.transform.rotation = frame.rotation;

        // If the recorded frame indicates death, end the replay
        if (frame.isDead)
        {
            EndReplay();
            return;
        }
    }

    // Starts recording a new gameplay session
    public void StartRecording()
    {
        frames.Clear();       // Clear previous recordings
        isRecording = true;
        isReplaying = false;
        replayTimer = 0f;

        player.FreezePlayer(false);  // Unfreeze player for gameplay
        player.isDead = false;
    }

    // Stops recording and saves the captured data to disk
    public void StopRecordingAndSave()
    {
        isRecording = false;

        // Capture one final frame
        frames.Add(new ReplayFrame(
            player.transform.position,
            player.transform.rotation,
            player.isDead,
            Time.time
        ));

        SaveToFile();  // Serialize and save to JSON file
    }

    // Begins playback of a previously saved replay
    public void StartReplay()
    {
        frames = LoadFromFile(); // Load frames from disk
        if (frames.Count == 0) return;

        replayIndex = 0;
        replayTimer = 0f;
        isReplaying = true;
        isRecording = false;

        player.FreezePlayer(true); // Freeze player input during replay
    }

    // Ends the replay and shows the death screen
    void EndReplay()
    {
        gameManager.deathPanel.SetActive(true);
        isReplaying = false;
    }

    // Saves the replay frames to a JSON file
    void SaveToFile()
    {
        string json = JsonUtility.ToJson(new ReplayDataWrapper(frames));
        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
    }

    // Loads replay frames from a JSON file
    List<ReplayFrame> LoadFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) return new List<ReplayFrame>();

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<ReplayDataWrapper>(json).frames;
    }

    // Wrapper class needed for serializing/deserializing a list of ReplayFrames
    [System.Serializable]
    public class ReplayDataWrapper
    {
        public List<ReplayFrame> frames;
        public ReplayDataWrapper(List<ReplayFrame> frames) => this.frames = frames;
    }
}
