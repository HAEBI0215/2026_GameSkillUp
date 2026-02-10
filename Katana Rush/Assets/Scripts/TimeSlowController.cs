using UnityEngine;

public class TimeSlowController : MonoBehaviour
{
    [Range(0.01f, 1f)]
    public float slowScale = 0.1f;

    public bool slowMode = true;

    void Start()
    {
        ApplyTimeScale();
    }

    void OnEnable()
    {
        ApplyTimeScale();
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void ApplyTimeScale()
    {
        if (slowMode)
        {
            Time.timeScale = slowScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}
