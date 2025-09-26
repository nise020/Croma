using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CSVReader
{
    private String[,] _arr_grid;

    public CSVReader()
    {
    }

    public CSVReader(String[,] grid)
    {
        _arr_grid = grid;
    }

    public String[,] grid
    {
        get { return _arr_grid; }
    }

    public CSVReader parse(TextAsset text_asset, bool debug)
    {
        parse(text_asset.text, debug);

        return this;
    }

    public CSVReader parse(string text, bool debug, int nEncode = 0)
    {
        _arr_grid = _split_csv_grid(text, nEncode);

        if (debug)
            debug_output_grid();

        return this;
    }

    public void debug_output_grid()
    {
        String textOutput = "";
        for (int y = 0; y < _arr_grid.GetUpperBound(1); y++)
        {
            for (int x = 0; x < _arr_grid.GetUpperBound(0); x++)
            {

                textOutput += _arr_grid[x, y];
                textOutput += "|";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }

    private int cur_col = 0;
    public bool reset_row(int row, int StartCol)
    {
        cur_col = StartCol;
        string s = _arr_grid[StartCol, row];
        if (s == null)
            return false;

        if (_arr_grid[StartCol, row] == "")
            return false;

        return true;
    }

    public bool get(int row, ref bool val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = false;
            return false;
        }

        val = Convert.ToBoolean(s);
        return true;
    }

    public bool get(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return false;
        }

        val = (int)Convert.ToInt32(s);
        return true;
    }
    public bool get(int row, ref short val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return false;
        }

        val = (short)Convert.ToInt16(s);
        return true;
    }

    public bool get(int row, ref byte val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return false;
        }

        val = (byte)Convert.ToInt32(s);
        return true;
    }

    public bool getlong(int row, ref long val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return false;
        }

        val = (long)Convert.ToInt64(s);
        return true;
    }

    public void get(int row, ref int[] val, int nCnt)
    {
        for (int i = 0; i < nCnt; ++i)
        {
            string s = _arr_grid[cur_col, row];
            ++cur_col;

            if ((s == null) || (s == ""))
            {
                val[i] = 0;
                continue;
            }

            val[i] = (int)Convert.ToInt32(s);
        }
    }

    public bool get(int row, ref float val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0f;
            return false;
        }

        val = (float)Convert.ToSingle(s);
        return true;
    }

    public void get(int row, ref float[] val, int nCnt)
    {
        for (int i = 0; i < nCnt; ++i)
        {
            string s = _arr_grid[cur_col, row];
            ++cur_col;

            if ((s == null) || (s == ""))
            {
                val[i] = 0f;
                continue;
            }

            val[i] = (float)Convert.ToSingle(s);
        }
    }

    public bool get(int row, ref string val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = "";
            return false;
        }

        val = s;
        return true;
    }

    public void get(int row, ref string[] val, int nCnt)
    {
        for (int i = 0; i < nCnt; ++i)
        {
            string s = _arr_grid[cur_col, row];
            ++cur_col;

            if ((s == null) || (s == ""))
            {
                val[i] = "";
                continue;
            }

            val[i] = s;
        }
    }

    public void get_Anihash(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = Animator.StringToHash(s);
    }

    public void get_hash(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = s.GetHashCode();
    }

    public void get_bit(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        val = 0;
        if ((s == null) || (s == ""))
        {
            return;
        }


        int v = (int)Convert.ToInt32(s);
        int nBitIndex = 0;
        while (v != 0)
        {
            if (v % 10 == 1)
                val |= 1 << nBitIndex;

            v = v / 10;
            ++nBitIndex;
        }
    }

    public void get_bit_ZeroToAll(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        val = (int)0xFFFF;
        if ((s == null) || (s == ""))
        {
            return;
        }

        int v = (int)Convert.ToInt32(s);
        if (v == 0)
            return;

        val = 0;
        int nBitIndex = 0;
        while (v != 0)
        {
            if (v % 10 == 1)
                val |= 1 << nBitIndex;

            v = v / 10;
            ++nBitIndex;
        }
    }

    public void next()
    {
        ++cur_col;
    }

    public CSVReader find(int field_index, String value)
    {
        List<int> list_index = new();
        for (int i = 0; i < _arr_grid.GetUpperBound(1); ++i)
        {
            if (value != _arr_grid[field_index, i])
                continue;

            list_index.Add(i);
        }

        if (0 == list_index.Count)
            return null;

        String[,] arr_new_grid = new String[_arr_grid.GetUpperBound(0) + 1, list_index.Count + 1];
        for (int i = 0; i < _arr_grid.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < list_index.Count; ++j)
            {
                arr_new_grid[i, j] = _arr_grid[i, list_index[j]];
            }
        }
        return new CSVReader(arr_new_grid);
    }

    public String find_value(int field_index, String value, System.Object field)
    {
        return find(field_index, value).grid[Convert.ToInt32(field), 0];
    }

    public int column
    {
        get { return _arr_grid.GetUpperBound(0); }
    }

    public int row
    {
        get { return _arr_grid.GetUpperBound(1); }
    }

    private String[,] _split_csv_grid(String csvText, int nEncode)
    {
        if (2 == nEncode)
            csvText = csvText.Replace("\t", ",");

        bool bFindNewLine = false;
        int nFindStartIndex = 0;
        int nFindEndIndex = 0;
        List<string> strList = new List<string>();
        for (int i = 0; i < csvText.Length; ++i)
        {
            if (csvText[i] == '"')
            {
                if (bFindNewLine == false)
                {
                    nFindStartIndex = i;
                    strList.Add(csvText.Substring(nFindEndIndex, nFindStartIndex - nFindEndIndex));
                    bFindNewLine = true;
                }
                else if (bFindNewLine == true)
                {
                    nFindEndIndex = i + 1;
                    string parcing = csvText.Substring(nFindStartIndex, nFindEndIndex - nFindStartIndex);
                    parcing = parcing.Replace("\"", "");
                    parcing = parcing.Replace("\n", "\\z");
                    strList.Add(parcing);
                    bFindNewLine = false;
                }
            }
        }


        if (strList.Count > 0)
        {
            strList.Add(csvText.Substring(nFindEndIndex, csvText.Length - 1 - nFindEndIndex));

            csvText = "";
            for (int i = 0; i < strList.Count; ++i)
            {
                csvText += strList[i];
            }
        }

        String[] lines = csvText.Split("\n"[0]);



        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            String[] row = _split_csv_line(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        String[,] outputGrid = new String[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            lines[y] = lines[y].Replace("/,", "asdf!@#$");
            String[] row = _split_csv_line(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                row[x] = row[x].Replace("asdf!@#$", ",");

                outputGrid[x, y] = row[x];

                outputGrid[x, y] = outputGrid[x, y].Replace(@"\n", "\n");
                outputGrid[x, y] = outputGrid[x, y].Replace(@"\z", "\n");
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }
    private String[] _split_csv_line(String line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
           @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
           System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }

    //public string[] GetRow(int rowIndex)
    //{
    //    // 예: 전체 데이터를 2차원 배열 또는 List<List<string>> 등으로 가지고 있을 경우
    //    return CSVReader[rowIndex];
    //}
}