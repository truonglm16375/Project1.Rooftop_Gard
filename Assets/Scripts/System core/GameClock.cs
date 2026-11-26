using UnityEngine;
using System;

public class GameClock : MonoBehaviour
{
    [Header("Cài đặt thời gian")]
    [Tooltip("Tốc độ trôi thời gian (1 = 1 giây thực là 1 phút trong game, 60 = 1 giây thực là 1 giờ)")]
    [SerializeField] private float timeScale = 1f;

    [Tooltip("Thời điểm thức dậy / bắt đầu ngày mới (tính bằng phút). Mặc định 5h sáng = 300 phút")]
    [SerializeField] private float getUpTime = 5f * 60f;

    // ===== Event =====
    // Sự kiện mỗi khi trôi qua một số giờ (tham số int: số giờ vừa trôi qua, thường là 1)
    public static event Action<int> OnHourPassed;
    public static event Action OnDayPassed;

    // ===== Thuộc tính thời gian =====
    public float timePassed { get; private set; }
    public int dayCount { get; private set; } = 0;
    public bool isPaused = false;
    private int lastHour = -1;

    // ---------- Admin Controls ----------
    [ContextMenu("Set Time (Minutes)")]
    public void SetTime(float minutes)
    {
        timePassed = Mathf.Max(0f, minutes);
        lastHour = Mathf.FloorToInt(timePassed / 60f);
        // Fire hour events up to the new hour if time moved forward
        OnHourPassed?.Invoke(0);
    }
    /// <summary>
    /// Advance the clock by a number of hours (useful for fast‑forward testing).
    /// </summary>
    [ContextMenu("Advance Hours")]
    public void AdvanceHours(int hours)
    {
        if (hours <= 0) return;
        // Simulate hour passing using existing logic
        for (int i = 0; i < hours; i++)
        {
            timePassed += 60f;
            int currentHour = Mathf.FloorToInt(timePassed / 60f);
            if (currentHour != lastHour && currentHour < 24)
            {
                int diff = currentHour - lastHour;
                if (diff < 0) diff += 24;
                lastHour = currentHour;
                OnHourPassed?.Invoke(diff);
            }
        }
    }
    /// <summary>
    /// Change the speed at which in‑game time progresses.
    /// </summary>
    [ContextMenu("Set Time Scale")]
    public void SetTimeScale(float scale)
    {
        timeScale = Mathf.Max(0f, scale);
    }

    public static GameClock Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        timePassed = getUpTime;
        lastHour = Mathf.FloorToInt(timePassed / 60f);
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (isPaused) return;

        timePassed += Time.deltaTime * timeScale;
        int currentHour = Mathf.FloorToInt(timePassed / 60f);

        // Kiểm tra khi trôi qua 1 giờ mới
        if (currentHour != lastHour && currentHour < 24)
        {
            int hoursPassed = currentHour - lastHour;
            if (hoursPassed < 0) hoursPassed += 24;
            lastHour = currentHour;
            OnHourPassed?.Invoke(hoursPassed);
        }

        // Kiểm tra chuyển sang ngày tiếp theo (24h = 1440 phút)
        if (timePassed >= 60f * 24f)
        {
            int overnightHours = (24 - lastHour) + Mathf.FloorToInt(getUpTime / 60f);
            timePassed = getUpTime;
            dayCount++;
            lastHour = Mathf.FloorToInt(timePassed / 60f);

            OnDayPassed?.Invoke();

            if (overnightHours > 0)
            {
                OnHourPassed?.Invoke(overnightHours);
            }
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        isPaused = (newState == GameState.Paused);
    }

    public int[] getTime()
    {
        int totalMinutes = Mathf.FloorToInt(timePassed);
        int hours = (totalMinutes / 60) % 24;
        int minutes = totalMinutes % 60;

        return new int[] {dayCount, hours, minutes};
    }
}