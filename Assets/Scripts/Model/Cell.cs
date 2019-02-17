using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Cell : MonoBehaviour, IComparable<Cell> //, IComparer<Cell>
{
    [SerializeField]
    private float swipeDuration = 1;
    //[HideInInspector]
    public int iIndex;
    //[HideInInspector]
    public int jIndex;
    public GridObject GridObject { get; private set; }
    private Image _image;
    private RectTransform _rectTransform;

    void Awake()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetGridObject(GridObject gridObject)
    {
        GridObject = gridObject;
        _image.sprite = GridObject.Sprite;
    }

    public void ResetCell()
    {
        GridObject = null;
        _image.sprite = null;
        _rectTransform.localScale = new Vector3(1,1,1);
    }

    public void ReduceCell(Action callback)
    {
        StartCoroutine(Reduce(callback));
    }

    private IEnumerator Reduce(Action callback)
    {
        while (_rectTransform.localScale.x > 0.1f)
        {
            _rectTransform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            yield return null;
        }
        callback?.Invoke();
    }

    public void SetPosition(float positionY)
    {
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, positionY);
    }

    public void SwipeToPosition(float goalX, float goalY, Action callback)
    {
        StartCoroutine(Swipe(goalX, goalY, callback));
    }

    private IEnumerator Swipe(float goalX, float goalY, Action callback)
    {
        Vector3 startPosition = _rectTransform.anchoredPosition;
        Vector3 goalPosition = new Vector3(goalX, goalY, 0);
        float swipeTimer = 0;
        while (_rectTransform.anchoredPosition.x != goalPosition.x || _rectTransform.anchoredPosition.y != goalPosition.y)
        {
            swipeTimer += Time.unscaledDeltaTime;
            _rectTransform.anchoredPosition = Vector3.Lerp(startPosition, goalPosition, swipeTimer / swipeDuration);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log($"Cell {iIndex},{jIndex} moving ended.");
        callback?.Invoke();
    }

    public void SetIndexes(int i, int j)
    {
        iIndex = i;
        jIndex = j;
    }

    public void ShowMatch()
    {
        StartCoroutine(Increase());
    }

    private IEnumerator Increase()
    {
        while (_rectTransform.localScale.x < 1.2f)
        {
            _rectTransform.localScale += new Vector3(0.03f, 0.03f, 0.03f);
            yield return null;
        }
        while (_rectTransform.localScale.x > 1f)
        {
            _rectTransform.localScale -= new Vector3(0.03f, 0.03f, 0.03f);
            yield return null;
        }
        _rectTransform.localScale = new Vector3(1, 1, 1);
    }


    public int Compare(Cell x, Cell y)
    {
        RectTransform xRect = x.GetComponent<RectTransform>();
        RectTransform yRect = y.GetComponent<RectTransform>();
        // Если выше, то ближе к началу массива
        if (xRect.anchoredPosition.y > yRect.anchoredPosition.y)
        {
            return -1;
        }
        else if (xRect.anchoredPosition.y < yRect.anchoredPosition.y)
        {
            return 1;
        }
        // Если правее, то ближе к концу массива
        else if (xRect.anchoredPosition.x > yRect.anchoredPosition.x)
        {
            return 1;
        }
        else if (xRect.anchoredPosition.x < yRect.anchoredPosition.x)
        {
            return -1;
        }
        else
        {
            return 0;
        }

    }

    public int CompareTo(Cell other)
    {
        RectTransform xRect = transform.GetComponent<RectTransform>();
        RectTransform yRect = other.GetComponent<RectTransform>();
        // Если выше, то ближе к началу массива
        if (xRect.anchoredPosition.y > yRect.anchoredPosition.y)
        {
            return -1;
        }
        else if (xRect.anchoredPosition.y < yRect.anchoredPosition.y)
        {
            return 1;
        }
        // Если правее, то ближе к концу массива
        else if (xRect.anchoredPosition.x > yRect.anchoredPosition.x)
        {
            return 1;
        }
        else if (xRect.anchoredPosition.x < yRect.anchoredPosition.x)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
