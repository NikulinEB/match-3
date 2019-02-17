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
    }

    private void OnDestroy()
    {
        Events.Matched -= UpdateCounter;
    }

    private void UpdateCounter()
    {
        _counter.text = GameController.Instance.MatchPoints.ToString();
    }
}
