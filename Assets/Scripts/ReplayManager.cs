using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ReplayManager : MonoBehaviour
{
    public PlayerController player;
    public string fileName = "replay.json";
    public GameManager gameManager;

    private List<ReplayFrame> frames = new List<ReplayFrame>();
    private int replayIndex = 0;
    private float replayTimer = 0f;

    private bool isRecording = false;
    private bool isReplaying = false;

    void Update()
    {
        if (isRecording)
        {
            RecordFrame();
        }
        else if (isReplaying)
        {
            PlayReplay();
        }
    }

    void RecordFrame()
    {
        frames.Add(new ReplayFrame(
            player.transform.position,
            player.transform.rotation,
            player.isDead,
            Time.time
        ));
    }

    void PlayReplay()
    {
        if (replayIndex >= frames.Count - 1)
        {
            EndReplay();
            return;
        }

        replayTimer += Time.deltaTime;

        while (replayIndex < frames.Count - 1 && replayTimer >= frames[replayIndex + 1].time)
        {
            replayIndex++;
        }

        var frame = frames[replayIndex];

        player.transform.position = frame.position;
        player.transform.rotation = frame.rotation;

        if (frame.isDead)
        {
            EndReplay();
            return;
        }
    }

    public void StartRecording()
    {
        frames.Clear();
        isRecording = true;
        isReplaying = false;
        replayTimer = 0f;

        player.FreezePlayer(false);
        player.isDead = false;
    }

    public void StopRecordingAndSave()
    {
        isRecording = false;

        frames.Add(new ReplayFrame(
            player.transform.position,
            player.transform.rotation,
            player.isDead,
            Time.time
        ));

        SaveToFile();
    }

    public void StartReplay()
    {
        frames = LoadFromFile();
        if (frames.Count == 0) return;

        replayIndex = 0;
        replayTimer = 0f;
        isReplaying = true;
        isRecording = false;

        player.FreezePlayer(true);
    }

    void EndReplay()
    {
        gameManager.deathPanel.SetActive(true);
        isReplaying = false;
    }

    void SaveToFile()
    {
        string json = JsonUtility.ToJson(new ReplayDataWrapper(frames));
        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
    }

    List<ReplayFrame> LoadFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) return new List<ReplayFrame>();
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<ReplayDataWrapper>(json).frames;
    }

    [System.Serializable]
    public class ReplayDataWrapper
    {
        public List<ReplayFrame> frames;
        public ReplayDataWrapper(List<ReplayFrame> frames) => this.frames = frames;
    }
}
