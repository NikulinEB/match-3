using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    #region Singleton

    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
            }
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameController");
                _instance = obj.AddComponent<GameController>();
                Debug.Log("Could not locate an GameController object. GameController was generated Automatically.");
            }
            return _instance;
        }
    }

    static GameController() { }

    #endregion

    public string LinkInGooglePlay;

    private int _pointsCount = 0;

    public int PointsCount
    {
        get { return _pointsCount; }
        private set
        {
            if (value < 0)
            {
                throw new System.Exception("Points count value can not be lower than 0.");
            }
            else
            {
                _pointsCount = value;
                PlayerPrefs.SetInt(PlayerPrefsName.PointsCount.ToString(), value);
            }
        }
    }

    public int MatchPoints { get; private set; }

    private bool _sound = true;

    public bool Sound
    {
        get { return _sound; }

        set
        {
            _sound = value;
            AudioListener.pause = !_sound;
            PlayerPrefs.SetString(PlayerPrefsName.Sound.ToString(), _sound.ToString());
        }
    }

    private int _currentLevel = 1;

    public int CurrentLevel {
        get
        {
            return _currentLevel;
        }
        set
        {
            _currentLevel = value;
            PlayerPrefs.SetInt(PlayerPrefsName.CurrentLevel.ToString(), value);
        }
    }

    public bool ControlEnabled { get; private set; } = true;

    [Tooltip("Count of matches that player have to score.")]
    [SerializeField]
    private int[] _levelsPoints;
    [Tooltip("Seconds to complete level. Array length shoud be equal to LevelsPoint.")]
    [SerializeField]
    private int[] _levelsSeconds;

    public int[,] LevelsGoals;

    private int _levelTimer;

    private void Awake()
    {
        LoadPlayerPrefs();
        SetLevelsGoals();
        Events.ToggleControl += ToggleControl;
        Events.LevelStarted += EnableControl;
        Events.Matched += CheckLevelEnd;
        //Events.ContinueLevel += EnableControl;
        //Events.Death += DisableControl;
    }

    private void OnDestroy()
    {
        Events.ToggleControl -= ToggleControl;
        Events.LevelStarted -= EnableControl;
        Events.Matched -= CheckLevelEnd;
        //Events.ContinueLevel -= EnableControl;
        //Events.Death -= DisableControl;
    }

    private void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsName.PointsCount.ToString()))
        {
            PointsCount = PlayerPrefs.GetInt(PlayerPrefsName.PointsCount.ToString());
        }
        else
        {
            PointsCount = 0;
        }

        if (PlayerPrefs.HasKey(PlayerPrefsName.Sound.ToString()))
        {
            if ("True" == PlayerPrefs.GetString(PlayerPrefsName.Sound.ToString()))
            {
                Sound = true;
            }
            else
            {
                Sound = false;
            }
        }
        if (PlayerPrefs.HasKey(PlayerPrefsName.CurrentLevel.ToString()))
        {
            CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsName.CurrentLevel.ToString());
        }
    }

    private void SetLevelsGoals()
    {
        LevelsGoals = new int[_levelsPoints.Length,2];
        for (int i = 0; i < LevelsGoals.GetLength(0); i++)
        {
            LevelsGoals[i, 0] = _levelsPoints[i];
            LevelsGoals[i, 1] = _levelsSeconds[i];
        }
    }

    private void ToggleControl(bool enabled)
    {
        ControlEnabled = enabled;
    }

    public void EnableControl()
    {
        Events.ToggleControl_Call(true);
    }

    public void DisableControl()
    {
        Events.ToggleControl_Call(false);
    }

    public void AddPoints(int points)
    {
        MatchPoints += points;
    }

    public void StartLevel()
    {
        MatchPoints = 0;
        _levelTimer = Timer.Instance.DoAfter(LevelsGoals[CurrentLevel - 1, 1], EndLevel);
        Events.LevelStarted_Call();
    }

    private void EndLevel()
    {
        if (MatchPoints >= LevelsGoals[CurrentLevel - 1, 0])
        {
            if (CurrentLevel < LevelsGoals.GetLength(0))
            {
                CurrentLevel++;
            }
            Events.ShowMenu_Call(MenuType.Win);
        }
        else
        {
            Events.ShowMenu_Call(MenuType.Defeat);
        }
    }

    private void CheckLevelEnd()
    {
        if (MatchPoints >= LevelsGoals[CurrentLevel - 1, 0])
        {
            Timer.Instance.StopTimer(_levelTimer);
            EndLevel();
        }
    }
}
