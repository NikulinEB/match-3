using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GridObject[] _gridObjects;
    private Cell[,] _cells = new Cell[6,6];
    private float startPositionX = 100;
    private float startPositionY = -100;
    private float offsetX = 175;
    private float offsetY = -175;
    private bool _debugWaiting;

    void Start()
    {
        InitGrid();
        FillGrid();
        Events.Swipe += Swipe;
    }

    private void OnDestroy()
    {
        Events.Swipe -= Swipe;
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

    private void FillGrid()
    {
        do
        {
            SetRandomObjects();
        }
        while (FindMatches().Count != 0);
    }

    private void SetRandomObjects()
    {
        foreach (var cell in _cells)
        {
            cell.SetGridObject(GetRandomGridObject());
        }
    }

    private void Swipe(Cell cell, SwipeTurn swipeTurn)
    {
        if (!GameController.Instance.ControlEnabled)
            return;
        GameController.Instance.DisableControl();
        var gridObject = cell.GridObject;
        Cell cell2;
        switch(swipeTurn)
        {
            case SwipeTurn.Left:
                if (cell.jIndex - 1 < 0)
                {
                    GameController.Instance.EnableControl();
                    return;
                }
                cell2 = _cells[cell.iIndex, cell.jIndex - 1];
                break;
            case SwipeTurn.Right:
                if (cell.jIndex + 1 >= _cells.GetLength(1))
                {
                    GameController.Instance.EnableControl();
                    return;
                }
                cell2 = _cells[cell.iIndex, cell.jIndex + 1];
                break;
            case SwipeTurn.Up:
                if (cell.iIndex - 1 < 0)
                {
                    GameController.Instance.EnableControl();
                    return;
                }
                cell2 = _cells[cell.iIndex - 1, cell.jIndex];
                break;
            case SwipeTurn.Down:
                if (cell.iIndex + 1 >= _cells.GetLength(0))
                {
                    GameController.Instance.EnableControl();
                    return;
                }
                cell2 = _cells[cell.iIndex + 1, cell.jIndex];
                break;
            default:
                if (cell.iIndex + 1 > _cells.GetLength(0))
                {
                    GameController.Instance.EnableControl();
                    return;
                }
                cell2 = _cells[cell.iIndex + 1, cell.jIndex];
                break;
        }
        StartCoroutine(Swipe(cell, cell2));
    }

    private IEnumerator Swipe(Cell cell1, Cell cell2)
    {
        int movedCellsCounter = 0;
        float cellPositionX = startPositionX + cell1.jIndex * offsetX;
        float cellPositionY = startPositionY + cell1.iIndex * offsetY;
        float cell2PositionX = startPositionX + cell2.jIndex * offsetX;
        float cell2PositionY = startPositionY + cell2.iIndex * offsetY;
        cell2.SwipeToPosition(cellPositionX, cellPositionY, () => { movedCellsCounter++; });
        cell1.SwipeToPosition(cell2PositionX, cell2PositionY, () => { movedCellsCounter++; });
        yield return new WaitUntil(() => { return movedCellsCounter == 2; });
        SwapCells(cell1, cell2);
        // Если ход не дает совпадений, то отменяем его.
        if (FindMatches().Count == 0)
        {
            movedCellsCounter = 0;
            cell2.SwipeToPosition(cell2PositionX, cell2PositionY, () => { movedCellsCounter++; });
            cell1.SwipeToPosition(cellPositionX, cellPositionY, () => { movedCellsCounter++; });
            yield return new WaitUntil(() => { return movedCellsCounter == 2; });
            SwapCells(cell1, cell2);
        }
        // Если есть совпадение, то удаляем объекты.
        else
        {
            do
            {
                _debugWaiting = true;
                DeleteMatches(FindMatches(), () => { _debugWaiting = false; });
                yield return new WaitWhile(() => _debugWaiting);
                _debugWaiting = true;
                StartCoroutine(MoveCellsDown(() => { _debugWaiting = false; }));
                yield return new WaitWhile(() => _debugWaiting);
                _debugWaiting = true;
                StartCoroutine(FillEmptyCells(() => { _debugWaiting = false; }));
                yield return new WaitWhile(() => _debugWaiting);

            }
            while (FindMatches().Count != 0);
            // Если больше нет возможных совпадений, то обновляем сетку.
            //yield return new WaitWhile(() => _debugWaiting);
            var matches = new List<Cell>();
            if (!IsMatchExist(out matches))
            {
                FillGrid();
            }
        }
        GameController.Instance.EnableControl();
        //WriteGridButton();
    }

    private void SwapCells(Cell cell1, Cell cell2)
    {
        _cells[cell1.iIndex, cell1.jIndex] = cell2;
        _cells[cell2.iIndex, cell2.jIndex] = cell1;
        //cell1.SetGridObject(cell2.GridObject);
        int tempI = cell1.iIndex;
        int tempJ = cell1.jIndex;
        cell1.SetIndexes(cell2.iIndex, cell2.jIndex);
        //cell2.SetGridObject(tempCell.GridObject);
        cell2.SetIndexes(tempI, tempJ);
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

    private void DeleteMatches(List<Cell> matches, Action callback)
    {
        StartCoroutine(ReducingCells(matches, callback));
    }

    private IEnumerator ReducingCells(List<Cell> matches, Action callback)
    {
        int cellsCounter = 0;
        foreach (var cell in matches)
        {
            cell.ReduceCell(() => { cellsCounter++; });
        }
        yield return new WaitUntil(() => { return cellsCounter == matches.Count; });
        foreach (var cell in matches)
        {
            cell.ResetCell();
            cell.SetPosition(1000);
        }
        GameController.Instance.AddPoints(matches.Count);
        Events.Matched_Call();
        callback?.Invoke();
    }

    private bool IsMatchExist(out List<Cell> matches)
    {
        matches = new List<Cell>();
        Cell matchingCell;
        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                // Проверка горизонтального совпадения с двумя объектами рядом.
                if (IsMatchPattern(i, j, new int[,] { { 0, 1 } }, new int[,] { { -1, -1 }, { 1, -1 }, { 0, -2 }, { 1, 2 }, { -1, 2 }, { 0, 3 } }, out matchingCell))
                {
                    matches.AddRange(new List<Cell>{ _cells[i,j], _cells[i,j + 1], matchingCell });
                    return true;
                }
                // Проверка горизонального совпадения с двумя объектами через один пропуск.
                if (IsMatchPattern(i, j, new int[,] { { 0, 2 } }, new int[,] { { -1, 1 }, { 1, 1 } }, out matchingCell))
                {
                    matches.AddRange(new List<Cell> { _cells[i, j], _cells[i, j + 2], matchingCell });
                    return true;
                }
                // Проверка вертикального совпадения с двумя объектами рядом.
                if (IsMatchPattern(i, j, new int[,] { { 1, 0 } }, new int[,] { { -2, 0 }, { -1, -1 }, { -1, 1 }, { 3, 0 }, { 2, -1 }, { 2, 1 } }, out matchingCell))
                {
                    matches.AddRange(new List<Cell> { _cells[i, j], _cells[i + 1, j], matchingCell });
                    return true;
                }
                // Проверка вертикального совпадения с двумя объектами через один пропуск.
                if (IsMatchPattern(i, j, new int[,] { { 2, 0 } }, new int[,] { { 1, -1 }, { 1, 1 } }, out matchingCell))
                {
                    matches.AddRange(new List<Cell> { _cells[i, j], _cells[i + 2, j], matchingCell });
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsMatchPattern(int iIndex, int jIndex, int[,] matchAll, int[,] matchOne, out Cell matchingCell)
    {
        matchingCell = null;
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
        for (int i = 0; i < matchOne.GetLength(0); i++)
        {
            if (IsMatchColor(iIndex + matchOne[i, 0], jIndex + matchOne[i, 1], color))
            {
                matchingCell = _cells[iIndex + matchOne[i, 0], jIndex + matchOne[i, 1]];
                return true;
            }
        }
        return false;
    }

    private bool IsMatchColor(int iIndex, int jIndex, ObjectColor color)
    {
        if (iIndex < 0 || iIndex >= _cells.GetLength(0) || jIndex < 0 || jIndex >= _cells.GetLength(1))
        {
            return false;
        }
        else
        {
            return _cells[iIndex, jIndex].GridObject.Color == color;
        }
    }

    private IEnumerator MoveCellsDown(Action callback)
    {
        int cellsCount = 0;
        int movedCellsCount = 0;
        for (int j = _cells.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = _cells.GetLength(0) - 1; i >= 0; i--)
            {
                int iNotNull = i;
                while (_cells[iNotNull, j].GridObject == null && iNotNull > 0)
                {
                    iNotNull--;
                }
                if (iNotNull != i)
                {
                    if (_cells[iNotNull, j].GridObject != null)
                    {
                        cellsCount++;
                        SwapCells(_cells[i, j], _cells[iNotNull, j]);
                        //_cells[i, j].SetGridObject(_cells[iNotNull, j].GridObject);
                        _cells[i, j].SwipeToPosition(startPositionX + offsetX * j, startPositionY + offsetY * i, () => { movedCellsCount++; });
                        _cells[iNotNull, j].ResetCell();
                    }
                }
            }
        }
        yield return new WaitUntil(() => { return movedCellsCount == cellsCount; });
        callback?.Invoke();
    }

    private IEnumerator FillEmptyCells(Action callback)
    {
        int cellsCount = 0;
        int movedCellsCount = 0;
        foreach (var cell in _cells)
        {
            if (cell.GridObject == null)
            {
                cellsCount++;
                cell.SetPosition(startPositionY - offsetY * (_cells.GetLength(1) - cell.iIndex));
                cell.SetGridObject(GetRandomGridObject());
                cell.SwipeToPosition( startPositionX + offsetX * cell.jIndex,  startPositionY + offsetY * cell.iIndex, () => { movedCellsCount++; });
            }
        }
        yield return new WaitUntil(() => { return movedCellsCount == cellsCount; });
        callback?.Invoke();
    }

    private GridObject GetRandomGridObject()
    {
        return _gridObjects[UnityEngine.Random.Range(0, (int)_gridObjects.Length)];
    }

    public void GetMatches(Text text)
    {
        string result = "";
        var matches = FindMatches();
        result += "Matches: " + matches.Count + "\n";
        foreach(var match in matches)
        {
            result += $"{match.iIndex},{match.jIndex}; ";
        }
        text.text = result;
        Debug.Log(result);
    }

    public void WriteGridButton()
    {
        string resultGrid = "";
        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                resultGrid += string.Format("{0},{1} {2}     \t",i, j, _cells[i, j].GridObject.Color);
            }
            resultGrid += "\n";
        }
        Debug.Log(resultGrid);
    }

    public void ShowMatchesButton()
    {
        List<Cell> matches = new List<Cell>();
        IsMatchExist(out matches);
        foreach(var match in matches)
        {
            match.ShowMatch();
        }
    }
}
