using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PointsCounter : MonoBehaviour
{
    private Text _counter;

    void Start()
    {
        _counter = GetComponent<Text>();
        Events.Matched += UpdateCounter;
        Events.LevelStarted += ResetCounter;
    }

    private void OnDestroy()
    {
        Events.Matched -= UpdateCounter;
        Events.LevelStarted -= ResetCounter;
    }

    private void UpdateCounter()
    {
        _counter.text = "SCORE: " + GameController.Instance.MatchPoints.ToString();
    }

    private void ResetCounter()
    {
        _counter.text = "SCORE: 0";
    }
}
