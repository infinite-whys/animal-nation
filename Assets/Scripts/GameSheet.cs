using System.Collections.Generic;
using UnityEngine;

public class GameSheet
{
    public List<List<object>> data;
    public List<object> columns;

    public GameSheet(List<List<object>> _data)
    {
        //Debug.Log($"Gamesheet Constructor data - Count: {_data.Count}. Data: {_data.ToString()}");

        data = _data.GetRange(1, _data.Count - 1);

        columns = _data[0];
    }

    public object GetValue(int index, string columnName)
    {
        // index : row number
        // column name: -> deduct column number from column name

        // step 1: get row at index number
        if (index >= data.Count || index < 0)
        {
            Debug.LogWarning("Index out of range - Caught: " + index);
            return null;
        }
        List<object> row = data[index];
        // Debug.Log("index: "+index);

        // step 2: get value at column name
        int columnIndex = GetColumnIndexFromColumnName(columnName);
        if (columnIndex >= row.Count || columnIndex < 0)
        {
            Debug.LogWarning("Index out of range - Caught: " + columnIndex + " " + columnName + " row count: " + row.Count);
            return null;
        }
        object value = row[columnIndex];
        return value;

    }

    private int GetColumnIndexFromColumnName(string columnName)
    {
        return columns.IndexOf(columnName);
    }
}