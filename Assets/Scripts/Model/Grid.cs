using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private GridObject[] _gridObjects;
    private Cell[,] _cells = new Cell[6,6];

    void Start()
    {
        InitGrid();
        SetRandomObjects();
        Events.Swipe += SwipeObjects;
    }

    private void OnDestroy()
    {
        Events.Swipe -= SwipeObjects;
    }

    private void InitGrid()
    {
        var cells = GetComponentsInChildren<Cell>();
        Array.Sort(cells);
        int counter = 0;
        for(int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                _cells[i, j] = cells[counter];
                _cells[i, j].iIndex = i;
                _cells[i, j].jIndex = j;
                counter++;
            }
        }
    }

    private void SetRandomObjects()
    {
        foreach (var cell in _cells)
        {
            cell.SetGridObject(_gridObjects[UnityEngine.Random.Range(0, (int)_gridObjects.Length)]);
        }

    }

    private void SwipeObjects(Cell cell, SwipeTurn swipeTurn)
    {
        var gridObject = cell.GridObject;
        //TO DO: проверка на крайние объекты
        switch(swipeTurn)
        {
            case SwipeTurn.Left:
                _cells[cell.iIndex, cell.jIndex].SetGridObject(_cells[cell.iIndex, cell.jIndex - 1].GridObject);
                _cells[cell.iIndex, cell.jIndex - 1].SetGridObject(gridObject);
                break;
            case SwipeTurn.Right:
                _cells[cell.iIndex, cell.jIndex].SetGridObject(_cells[cell.iIndex, cell.jIndex + 1].GridObject);
                _cells[cell.iIndex, cell.jIndex + 1].SetGridObject(gridObject);
                break;
            case SwipeTurn.Up:
                _cells[cell.iIndex, cell.jIndex].SetGridObject(_cells[cell.iIndex - 1, cell.jIndex].GridObject);
                _cells[cell.iIndex - 1, cell.jIndex].SetGridObject(gridObject);
                break;
            case SwipeTurn.Down:
                _cells[cell.iIndex, cell.jIndex].SetGridObject(_cells[cell.iIndex + 1, cell.jIndex].GridObject);
                _cells[cell.iIndex + 1, cell.jIndex].SetGridObject(gridObject);
                break;
        }
    }

    private List<Cell> CheckMatches()
    {
        List<Cell> matches = new List<Cell>();
        int matchCounter = 0;
        ObjectColor color = 0;
        // Check horizontal matches.
        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            color = _cells[i, 0].GridObject.Color;
            matchCounter = 1;
            for (int j = 1; j < _cells.GetLength(1); j++)
            {
                if (_cells[i, j].GridObject.Color == color)
                {
                    matchCounter++;
                }
                else
                {
                    if (matchCounter >= 3)
                    {
                        for (int m = j - 1; m > j - 1 - matchCounter; m--)
                        {
                            matches.Add(_cells[i, m]);
                        }
                    }
                    color = _cells[i, j].GridObject.Color;
                    matchCounter = 1;
                }
            }
            if (matchCounter >= 3)
            {
                for (int m = _cells.GetLength(1) - 1; m > _cells.GetLength(1) - 1 - matchCounter; m--)
                {
                    matches.Add(_cells[i, m]);
                }
            }
        }
        // Check vertical matches.
        for (int j = 0; j < _cells.GetLength(1); j++)
        {
            color = _cells[0, j].GridObject.Color;
            matchCounter = 1;
            for (int i = 1; i < _cells.GetLength(0); i++)
            {
                if (_cells[i, j].GridObject.Color == color)
                {
                    matchCounter++;
                }
                else
                {
                    if (matchCounter >= 3)
                    {
                        for (int m = i - 1; m > i - 1 - matchCounter; m--)
                        {
                            if (!matches.Exists(cell => cell.iIndex == _cells[m, j].iIndex && cell.jIndex == _cells[m, j].jIndex))
                                matches.Add(_cells[m, j]);
                        }
                    }
                    color = _cells[i, j].GridObject.Color;
                    matchCounter = 1;
                }
            }
            if (matchCounter >= 3)
            {
                for (int m = _cells.GetLength(0) - 1; m > _cells.GetLength(0) - 1 - matchCounter; m--)
                {
                    if (!matches.Exists(cell => cell.iIndex == _cells[m, j].iIndex && cell.jIndex == _cells[m, j].jIndex))
                        matches.Add(_cells[m, j]);
                }
            }
        }
        return matches;
    }

    private void DeleteMatches(List<Cell> matches)
    {
        foreach(var cell in matches)
        {
            cell.ResetCell();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void MoveCellsDown()
    {

    }

    public void DebugButton()
    {
        var matches = CheckMatches();

        foreach (var cell in matches)
        {
            Debug.Log("Cell: " + cell.iIndex + ", " + cell.jIndex);
        }
        DeleteMatches(matches);
    }
}
