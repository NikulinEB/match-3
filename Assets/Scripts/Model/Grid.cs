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

    private List<Cell> FindMatches()
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

    private bool IsMatchExist()
    {
        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                // Проверка горизонтального совпадения с двумя объектами рядом.
                if (IsMatchPattern(i, j, new int[,] { { 0, 1 } }, new int[,] { { -1, -1 }, { 1, -1 }, { 0, -2 }, { 1, 2 }, { -1, 2 }, { 0, 3 } }))
                    return true;

                // Проверка горизонального совпадения с двумя объектами через один пропуск.
                if (IsMatchPattern(i, j, new int[,] { { 0, 2} }, new int[,] { { -1, 1}, { 1, 1} }))
                    return true;

                // Проверка вертикального совпадения с двумя объектами рядом.
                if (IsMatchPattern(i, j, new int[,] { { 1, 0} }, new int[,] { { -2, 0}, { -1, -1}, { -1, 1}, { 3, 0}, { 2, -1}, { 2, 1} }))
                    return true;

                // Проверка вертикального совпадения с двумя объектами через один пропуск.
                if (IsMatchPattern(i, j, new int[,] { { 2, 0} }, new int[,] { { 1, -1}, { 1, 1} }))
                    return true;
            }
        }
        return false;
    }

    private bool IsMatchPattern(int iIndex, int jIndex, int[,] matchAll, int[,] matchOne)
    {
        ObjectColor color = _cells[iIndex, jIndex].GridObject.Color;
        // Проверка обязательного совпадения.
        for (int i = 0; i < matchAll.GetLength(0); i++)
        {
            if (!IsMatchColor(iIndex + matchAll[i, 0], jIndex + matchAll[i, 1], color))
            {
                return false;
            }
        }

        // До первого совпадения.
        for (int i = 0; i < matchOne.Length; i++)
        {
            if (IsMatchColor(iIndex + matchOne[i, 0], jIndex + matchOne[i, 1], color))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsMatchColor(int iIndex, int jIndex, ObjectColor color)
    {
        if (iIndex < 0 || iIndex >= _cells.GetLength(0) || jIndex < 0 || jIndex > _cells.GetLength(1))
        {
            return false;
        }
        else
        {
            return _cells[iIndex, jIndex].GridObject.Color == color;
        }
    }

    private void MoveCellsDown()
    {

    }

    public void DebugButton()
    {
        var matches = FindMatches();

        foreach (var cell in matches)
        {
            Debug.Log("Cell: " + cell.iIndex + ", " + cell.jIndex);
        }
        DeleteMatches(matches);
    }
}
