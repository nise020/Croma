using System.Collections.Generic;
using System;

public class BookTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public byte Type;
        public int RewardId;
    }

    public Dictionary<int, Info> BookTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (BookTableData.ContainsKey(_Id))
            return BookTableData[_Id];
        return null;
    }
    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref BookTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, BookTableData);
    }

    public void Init_Csv(string _Name, int StartRoe, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null) { return; }

        for (int row = StartRoe; row < reader.row; ++row)
        {
            Info info = new Info();

            if (Read(reader, info, row, _StartCol) == false)
                break;

            BookTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Type);
        _Reader.get(_Row, ref _info.RewardId);

        return true;
    }
}
