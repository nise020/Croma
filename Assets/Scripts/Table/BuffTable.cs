using System.Collections.Generic;
using System;
using static Enums;
using UnityEngine;

public class BuffTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int Type;
        public int Value;
        public int Time;
        public string effectPath;
        public string spriteName;
        public int soundID;
    }

    public Dictionary<int, BuffData> BuffTableData = new Dictionary<int, BuffData>();

    public BuffData Get(int _Id)
    {
        if (BuffTableData.ContainsKey(_Id))
            return BuffTableData[_Id];
        return null;
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, BuffData>>(_Name, ref BuffTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, BuffTableData);
    }

    public void Init_Csv(string _Name, int StartRow, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null)
        {
            return;
        }

        BuffTableData.Clear();
        int loadedCount = 0;
        int errorCount = 0;

        for (int row = StartRow; row < reader.row; ++row)
        {
            Info info = new Info();
            if (Read(reader, info, row, _StartCol) == false)
                break;

            // 안전한 타입 변환
            if (!TryConvertToBuffType(info.Type, out BUFFTYPE buffType))
            {
                errorCount++;
                continue;
            }

            var buffData = new BuffData
            {
                id = info.Id,
                type = buffType,
                value = info.Value,
                time = info.Time,
                effectPath = info.effectPath,
                spriteName = info.spriteName,
                soundID = info.soundID,
            };

            // 중복 ID 체크
            if (BuffTableData.ContainsKey(info.Id))
            {
                Debug.LogWarning($"{info.Id} buff, Overwrite.");
            }

            buffData.DebuffCheck(buffData.id);
            BuffTableData[info.Id] = buffData;
            loadedCount++;
        }
        
        Debug.Log($"BuffTable: Load Success : {loadedCount}, Failed : {errorCount}");
    }

    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        try
        {
            if (_Reader.reset_row(_Row, _Col) == false)
                return false;

            _Reader.get(_Row, ref _info.Id);
            _Reader.get(_Row, ref _info.Type);
            _Reader.get(_Row, ref _info.Value);
            _Reader.get(_Row, ref _info.Time);
            _Reader.get(_Row, ref _info.effectPath);
            _Reader.get(_Row, ref _info.spriteName);
            _Reader.get(_Row, ref _info.soundID);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"BuffTable: CSV 읽기 오류 - Row: {_Row}, Error: {e.Message}");
            return false;
        }
    }

    // 안전한 타입 변환 메서드
    private bool TryConvertToBuffType(int typeValue, out BUFFTYPE buffType)
    {
        buffType = BUFFTYPE.None;

        // Enum에 해당 값이 정의되어 있는지 확인
        if (System.Enum.IsDefined(typeof(BUFFTYPE), typeValue))
        {
            buffType = (BUFFTYPE)typeValue;
            return true;
        }

        // 추가 로그로 어떤 값들이 유효한지 알려줌 (디버그용)
        if (typeValue < 0 || typeValue > 10) // BUFFTYPE 범위에 따라 조정
        {
            Debug.LogWarning($"BuffTable: BUFFTYPE 범위를 벗어난 값 - {typeValue}");
        }

        return false;
    }
}