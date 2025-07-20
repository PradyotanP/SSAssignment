using UnityEngine;

[System.Serializable]
public class ReplayFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public bool isDead;
    public float time;

    public ReplayFrame(Vector3 pos, Quaternion rot, bool isDead, float time)
    {
        this.position = pos;
        this.rotation = rot;
        this.isDead = isDead;
        this.time = time;
    }
}

