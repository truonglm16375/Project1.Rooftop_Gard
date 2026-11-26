using UnityEngine;
using TMPro;

public class ClockController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timeText;

    private void OnEnable() {
        GameClock.OnHourPassed += UpdateTime;
    }

    private void Start()
    {
        UpdateTime(0);
    }

    private void UpdateTime(int hoursPassed)
    {
        int[] timeAndDate = GameClock.Instance.getTime();
        timeText.SetText("Ngày {0} - {1:00}:00", timeAndDate[0], timeAndDate[1]);
    }
}