using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelNumber : MonoBehaviour
{
    private Text _levelNumber;

    void Start()
    {
        _levelNumber = GetComponent<Text>();
        Events.LevelStarted += SetLevelNumber;
    }

    private void SetLevelNumber()
    {
        _levelNumber.text = $"LEVEL: {GameController.Instance.CurrentLevel}";
    }
}
