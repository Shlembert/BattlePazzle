using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System;

public class DataManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        // ��������� ���� � ����� ������
        filePath = Path.Combine(Application.persistentDataPath, "userData.json");
        Debug.Log(filePath);
        // ���������, ���������� �� ����, ����� ������� �����
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("���� � ������� �� ������. �������� ������ �����.");

            // ������� ����� ������ UserData � ������ ��������� �������
            UserData newData = new UserData("");
            string jsonData = JsonUtility.ToJson(newData);

            // ���������� JSON ������ � ����� ����
            File.WriteAllText(filePath, jsonData);
        }
    }

    // ����� ���������� ������ � ���� JSON
    public void SaveUserData(string albumData)
    {
        UserData userData = new UserData(albumData);

        // ����������� ������ � JSON ������
        string jsonData = JsonUtility.ToJson(userData);

        // ���������� JSON ������ � ����
        File.WriteAllText(filePath, jsonData);
    }

    // ����� �������� ������ �� ����� JSON
    [System.Obsolete]
    public async UniTask<string> LoadUserDataAsync()
    {
        // ��������� ���������� �� ����
        if (File.Exists(filePath))
        {
            try
            {
                // ���������� ��������� JSON ������ �� �����
                string jsonData = await UniTask.Run(() => File.ReadAllText(filePath));

                // ����������� JSON ������ � ������ UserData
                UserData userData = JsonUtility.FromJson<UserData>(jsonData);

                // ���������� ��������� �������� �� ������� UserData
                return userData.album;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading user data: {e.Message}");
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}

[System.Serializable]
public class UserData
{
    public string album;

    public UserData(string album)
    {
        this.album = album;
    }
}
