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

    private bool _sound;

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

    public bool ControlEnabled { get; private set; } = true;

    private void Awake()
    {
        GetPlayerPrefs();
        Events.ToggleControl += ToggleControl;
        //Events.LevelLoaded += EnableControl;
        //Events.ContinueLevel += EnableControl;
        //Events.Death += DisableControl;
    }

    private void OnDestroy()
    {
        Events.ToggleControl -= ToggleControl;
        //Events.LevelLoaded -= EnableControl;
        //Events.ContinueLevel -= EnableControl;
        //Events.Death -= DisableControl;
    }

    private void GetPlayerPrefs()
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
        else
        {
            Sound = true;
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
}
