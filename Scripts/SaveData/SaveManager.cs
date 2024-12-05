using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    public static void Save<T>(string key, T saveData)
    {
        string jsonDataString = JsonUtility.ToJson(saveData, true);
        string encryptedJsonData = jsonDataString.Encrypt();

        PlayerPrefs.SetString(key, encryptedJsonData);
    }

    public static T Load<T>(string key) where T : new()
    {
        if (PlayerPrefs.HasKey(key))
        {
            string loadedString = PlayerPrefs.GetString(key);
            string decryptedJsonData = loadedString.Decrypt();

            return JsonUtility.FromJson<T>(decryptedJsonData);
        }
        else
            return new T();
    }
}
