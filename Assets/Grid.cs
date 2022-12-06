using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Grid : MonoBehaviour
{
    public Cell prefab;

    int[] numbers = new int[81];
    List<Cell> cells = new List<Cell>();

    void Start()
    {
        for (int i = 0; i < numbers.Length; ++i)
        {
            Cell cell = Instantiate(prefab, transform);
            cell.Reset();
            cells.Add(cell);
        }
    }

    Cell GetSelectedCell()
    {
        Cell cell = null;
        var selection = EventSystem.current.currentSelectedGameObject;
        if (selection != null)
        {
            cell = selection.GetComponent<Cell>();
        }
        return cell;
    }

    // returns which number was pressed
    // returns 0 if no number was pressed
    // returns -1 if nothing was pressed
    int GetNumberPressed()
    {
        string pressed = Input.inputString;
        if (string.IsNullOrEmpty(pressed))
        {
            return -1;
        }

        if (int.TryParse(pressed, out int value))
        {
            if (value > 0 && value <= 9)
            {
                return value;
            }
        }

        return 0;
    }

    private void Update()
    {
        Cell cell = GetSelectedCell();
        if (cell != null)
        {
            int index = cells.IndexOf(cell);
            int number = GetNumberPressed();
            if (number >= 1 && IsValid(index, number))
            {
                SetCell(index, number);
            }
            else if (number == 0)
            {
                ResetCell(index);
            }
        }
    }

    void SetCell(int index, int number)
    {
        numbers[index] = number;
        cells[index].Set(number);
    }

    void ResetCell(int index)
    {
        numbers[index] = 0;
        cells[index].Reset();
    }

    bool IsValid(int index, int number)
    {
        int row = index / 9;
        int col = index % 9;

        if (RowContains(row, number))
        {
            return false;
        }

        if (ColumnContains(col, number))
        {
            return false;
        }

        if (SubGridContains(col / 3, row / 3, number))
        {
            return false;
        }


        return true;
    }

    private bool SubGridContains(int col, int row, int number)
    {
        for (int r = row * 3; r < row * 3 + 3; r++)
        {
            for (int c = col * 3; c < col * 3 + 3; c++)
            {
                if (numbers[r * 9 + c] == number)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool RowContains(int row, int number)
    {
        for (int i = 0; i < 9; ++i)
        {
            if (numbers[row * 9 + i] == number)
            {
                return true;
            }
        }
        return false;
    }

    private bool ColumnContains(int col, int number)
    {
        for (int i = 0; i < 9; ++i)
        {
            if (numbers[i * 9 + col] == number)
            {
                return true;
            }
        }
        return false;
    }

    bool solved = false;

    public void TryToSolve()
    {
        solved = false;
        StartCoroutine(Solve());
    }

    IEnumerator Solve()
    {
        int free = FindFreeCellIndex();
        if (free == -1)
        {
            solved = true;
            yield break;
        }

        for (int i = 1; i < 10; i++)
        {
            if (IsValid(free, i))
            {
                SetCell(free, i);
                yield return Solve();
                if (solved)
                {
                    break;
                }
                ResetCell(free);
            }
        }
    }

    private int FindFreeCellIndex()
    {
        for (int i = 0; i < numbers.Length; ++i)
        {
            if (numbers[i] == 0)
            {
                return i;
            }
        }
        return -1;
    }
}
