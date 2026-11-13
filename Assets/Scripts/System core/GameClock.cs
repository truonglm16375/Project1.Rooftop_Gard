using UnityEngine;
using System;

public class GameClock : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private float getUpTime = 5f * 60f;

    public static event Action<int> OnHourPassed;
    public static event Action OnDayPassed;

    public float timePassed { get; private set; }
    public int dayCount { get; private set; }= 0;
    public bool isPaused = false;

    void Start()
    {
        timePassed = getUpTime;
    }

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }
    void Update()
    {
        if (!isPaused)
        {
            timePassed += timeScale * Time.deltaTime;
        }
        if(timePassed % 60f == 0) {
            int hoursPassed = Mathf.FloorToInt(timePassed / 60f);
            OnHourPassed?.Invoke(hoursPassed);
        }
        if(timePassed >= 60f * 24f)
        {
            timePassed = getUpTime;
            dayCount++;
        }
    }

    void HandleGameStateChanged(GameState newState)
    {
        isPaused = (newState == GameState.Paused);
    }

    public string getTime()
    {
        int totalMinutes = Mathf.FloorToInt(timePassed);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        return string.Format("{0:D2}:{1:D2}", hours, minutes);
    }
}