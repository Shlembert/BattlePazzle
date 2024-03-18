using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System;

public class DataManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        // Указываем путь к файлу данных
        filePath = Path.Combine(Application.persistentDataPath, "userData.json");
        Debug.Log(filePath);
        // Проверяем, существует ли файл, иначе создаем новый
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Файл с данными не найден. Создание нового файла.");

            // Создаем новый объект UserData с пустым значением альбома
            UserData newData = new UserData("");
            string jsonData = JsonUtility.ToJson(newData);

            // Записываем JSON данные в новый файл
            File.WriteAllText(filePath, jsonData);
        }
    }

    // Метод сохранения данных в файл JSON
    public void SaveUserData(string albumData)
    {
        UserData userData = new UserData(albumData);

        // Преобразуем данные в JSON формат
        string jsonData = JsonUtility.ToJson(userData);

        // Записываем JSON данные в файл
        File.WriteAllText(filePath, jsonData);
    }

    // Метод загрузки данных из файла JSON
    [System.Obsolete]
    public async UniTask<string> LoadUserDataAsync()
    {
        // Проверяем существует ли файл
        if (File.Exists(filePath))
        {
            try
            {
                // Асинхронно считываем JSON данные из файла
                string jsonData = await UniTask.Run(() => File.ReadAllText(filePath));

                // Преобразуем JSON данные в объект UserData
                UserData userData = JsonUtility.FromJson<UserData>(jsonData);

                // Возвращаем строковое значение из объекта UserData
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
