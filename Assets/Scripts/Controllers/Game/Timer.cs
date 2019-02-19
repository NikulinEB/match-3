using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    #region Singleton

    private static Timer _instance;

    public static Timer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Timer>();
            }
            if (_instance == null)
            {
                GameObject obj = new GameObject("Timer");
                _instance = obj.AddComponent<Timer>();
                Debug.Log("Could not locate an Timer object. Timer was generated Automatically.");
            }
            return _instance;
        }
    }

    static Timer() { }

    #endregion

    public float TimeFromStart { get; set; }

    private Dictionary<int, Coroutine> timers = new Dictionary<int, Coroutine>();

    private int _dictionaryNumber = 0;

    void Start()
    {
        
    }

    void Update()
    {
        TimeFromStart += Time.deltaTime;
    }

    public int DoAfter(float seconds, Action action)
    {
        int number = _dictionaryNumber++;
        timers.Add(number, StartCoroutine(WaitingForAction(seconds, action)));
        return number;
    }

    private IEnumerator WaitingForAction(float seconds, Action action)
    {
        float timerEnd = TimeFromStart + seconds;
        yield return new WaitUntil(() => { return TimeFromStart >= timerEnd; });
        action?.Invoke();
    }

    public int DoEvery(float seconds, Action action, int times = 0)
    {
        int number = _dictionaryNumber++;
        timers.Add(number, StartCoroutine(RegularAction(seconds, action, times)));
        return number;
    }

    private IEnumerator RegularAction(float seconds, Action action, int times)
    {
        float timer;
        while(times > 0)
        {
            timer = TimeFromStart + seconds;
            yield return new WaitUntil(() => { return TimeFromStart >= timer; });
            action?.Invoke();
            times--;
        }
    }

    public void StopTimer(int timerNumber)
    {
        StopCoroutine(timers[timerNumber]);
    }
}
