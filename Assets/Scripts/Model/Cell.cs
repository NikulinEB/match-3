using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Cell : MonoBehaviour, IComparable<Cell> //, IComparer<Cell>
{
    [HideInInspector]
    public int iIndex;
    [HideInInspector]
    public int jIndex;
    public GridObject GridObject { get; private set; }
    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
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
