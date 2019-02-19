using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelTimer : MonoBehaviour
{ 
    private Text _timer;
    private float levelStartTime;
    private float levelEndTime;

    void Start()
    {
        _timer = GetComponent<Text>();
        Events.LevelStarted += StartTimer;
    }

    private void OnDestroy()
    {
        Events.LevelStarted -= StartTimer;
    }

    private void StartTimer()
    {
        StartCoroutine(TimerTic());
    }

    private IEnumerator TimerTic()
    {
        levelEndTime = Timer.Instance.TimeFromStart + GameController.Instance.LevelsGoals[GameController.Instance.CurrentLevel - 1, 1];
        while (levelEndTime > Timer.Instance.TimeFromStart)
        { 
            _timer.text = (levelEndTime - Timer.Instance.TimeFromStart).ToString("00");
            yield return new WaitForFixedUpdate();
        }
        _timer.text = "0";
    }
}
