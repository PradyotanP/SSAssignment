using UnityEngine;

// Serializable class to store a snapshot of the player's state at a specific moment
[System.Serializable]
public class ReplayFrame
{
    public Vector3 position;      // Player's position at this frame
    public Quaternion rotation;  // Player's rotation at this frame
    public bool isDead;          // Whether the player was dead in this frame
    public float time;           // Time since the beginning of recording when this frame was captured

    // Constructor to initialize all values for the frame
    public ReplayFrame(Vector3 pos, Quaternion rot, bool isDead, float time)
    {
        this.position = pos;
        this.rotation = rot;
        this.isDead = isDead;
        this.time = time;
    }
}
