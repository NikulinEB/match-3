using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PointsGoal : MonoBehaviour
{
    private Text _matchesGoal;

    void Start()
    {
        _matchesGoal = GetComponent<Text>();
        Events.LevelStarted += SetLevelGoal;
    }

    private void OnDestroy()
    {
        Events.LevelStarted -= SetLevelGoal;
    }

    private void SetLevelGoal()
    {
        _matchesGoal.text = "GOAL: " + GameController.Instance.LevelsGoals[GameController.Instance.CurrentLevel - 1, 0].ToString();
    }
}
