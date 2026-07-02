using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class AlembicLoopPlayer : MonoBehaviour
{
    public AlembicStreamPlayer streamPlayer;
    public float speed = 1f;
    public bool loop = true;
    public float startTime = 0f;
    public Vector2[] timeStamps = new Vector2[2];

    void Start()
    {
        if (streamPlayer == null)
        {
            streamPlayer = GetComponent<AlembicStreamPlayer>();
        }

        if (streamPlayer != null)
        {
            streamPlayer.CurrentTime = startTime;
        }
    }

    void Update()
    {
        if (streamPlayer == null)
            return;

        if (streamPlayer.Duration <= 0f)
            return;

        var nextTime = streamPlayer.CurrentTime + Time.deltaTime * speed;

        if (loop)
        {
            streamPlayer.CurrentTime = Mathf.Repeat(nextTime, streamPlayer.Duration);
        }
        else
        {
            streamPlayer.CurrentTime = Mathf.Min(nextTime, streamPlayer.Duration);
        }
    }

    public void SetDesireTimeStamp(bool active)
    {
        streamPlayer.StartTime = timeStamps[active ? 1 : 0].x;
        streamPlayer.EndTime = timeStamps[active ? 1 : 0].y;
    }
}
