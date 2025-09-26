using UnityEngine;
using System.IO;
using System;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;

// ���� ����
public class Security_Base : MonoBehaviour
{
    protected T Load_Json<T>(string _Name, ref T _obj)
    {
        string fileBaseName = _Name;
        string runtimePath = Path.Combine(Application.persistentDataPath, fileBaseName + ".sav");
        string editorResourcePath = fileBaseName;

        TextAsset file = null;
        if (!File.Exists(runtimePath))
        {
            Debug.LogWarning($"[{nameof(Load_Json)}] No file: {runtimePath}");
            file = Resources.Load<TextAsset>(editorResourcePath);
        }

        try
        {
            // 1. ��ȣȭ�� ����(Base64 �ؽ�Ʈ) �б�
            string base64 = null;
            if (File.Exists(runtimePath))
            {
                base64 = File.ReadAllText(runtimePath, Encoding.UTF8);
            }
            else if (file != null)
            {
                base64 = file.text;
            }
            else
            {
                throw new FileNotFoundException("������ ã�� �� �����ϴ�.");
            }

            byte[] encrypted = Convert.FromBase64String(base64);

            // 2. ��ȣȭ
            byte[] decrypted = CryptoUtils.DecryptAndVerify(encrypted);

            // 3. JSON �� ��ü ������ȭ
            string json = Encoding.UTF8.GetString(decrypted);
            _obj = JsonConvert.DeserializeObject<T>(json);

            Debug.Log($"[{nameof(Load_Json)}] Load success: {runtimePath}");
            if (!File.Exists(runtimePath)) Save_Json(_Name, _obj);
        }
        catch (Exception e)
        {
            Debug.LogError($"[{nameof(Load_Json)}] Load failed: {e.Message}");
        }

        return _obj;
    }

    protected void Save_Json(string _Name, object _obj)
    {
        string fileBaseName = _Name;

        // 1. JSON ����ȭ
        string json = JsonConvert.SerializeObject(_obj);

        // 2. ��ȣȭ �� Base64 ���ڵ�
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        byte[] encrypted = CryptoUtils.EncryptAndHash(jsonBytes);
        string base64 = Convert.ToBase64String(encrypted);

        // 3. persistentDataPath�� ���� (��Ÿ�ӿ�)
        string runtimePath = Path.Combine(Application.persistentDataPath, fileBaseName + ".sav");

        try
        {
            string dir = Path.GetDirectoryName(runtimePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(runtimePath, base64, Encoding.UTF8);
            Debug.Log($"[{nameof(Save_Json)}] Save Success (Runtime Path): {runtimePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{nameof(Save_Json)}] Save failed (runtime path): {e.Message}");
        }

#if UNITY_EDITOR
        try
        {
            string editorFolder = Path.Combine(Application.dataPath, "Data/Resources");
            if (!Directory.Exists(editorFolder)) Directory.CreateDirectory(editorFolder);

            string editorPath = Path.Combine(editorFolder, fileBaseName + ".txt");
            File.WriteAllText(editorPath, base64, Encoding.UTF8);

            AssetDatabase.ImportAsset("Assets/Data/Resources/" + fileBaseName + ".txt");
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(Save_Json)}] Save Success (Editor Path): {editorPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{nameof(Save_Json)}] Save failed (Editor Path): {e.Message}");
        }
#endif
    }

    protected CSVReader GetCSVReader(string _Name)
    {
        string text = ".csv";
        string docPath = "./Document/";

        FileStream file = new FileStream(docPath + _Name + text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader stream = new StreamReader(file, Encoding.UTF8);
        CSVReader reader = new CSVReader();
        reader.parse(stream.ReadToEnd(), false, 1);
        stream.Close();

        return reader;
    }
}
