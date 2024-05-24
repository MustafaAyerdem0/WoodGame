using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;
using System;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;
    public static Action dbSyncAction;
    public Dictionary<string, DBSyncSynchronizer> dbKeys = new Dictionary<string, DBSyncSynchronizer>();


    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        dbSyncAction?.Invoke();
        GetAllData();

    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("LoginFailure: " + error.GenerateErrorReport());
    }

    public void SaveData(string key)
    {


        var data = new Dictionary<string, string>();
        var fields = dbKeys[key].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        var jsonData = new Dictionary<string, string>();
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<SyncWithDatabaseAttribute>() != null)
            {
                jsonData[field.Name] = field.GetValue(dbKeys[key])?.ToString();
            }
        }

        if (jsonData.Count > 0)
        {
            string jsonString = JsonConvert.SerializeObject(jsonData);
            data[key] = jsonString;

            var request = new UpdateUserDataRequest
            {
                Data = data
            };

            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }
        else
        {
            Debug.LogError("No fields with SyncWithDatabase attribute found to save.");
        }
    }

    [ContextMenu("GetAllData")]
    public void GetAllData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }

    void OnDataRecieved(GetUserDataResult result)
    {
        if (result != null)
        {
            foreach (var key in dbKeys)
            {
                if (result.Data.ContainsKey(key.Key))
                {
                    string jsonString = result.Data[key.Key].Value;
                    var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                    var fields = dbKeys[key.Key].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var field in fields)
                    {
                        if (field.GetCustomAttribute<SyncWithDatabaseAttribute>() != null && jsonData.ContainsKey(field.Name))
                        {
                            var value = jsonData[field.Name];
                            if (field.FieldType == typeof(string))
                            {
                                field.SetValue(dbKeys[key.Key], value);
                            }
                            else if (field.FieldType == typeof(int))
                            {
                                field.SetValue(dbKeys[key.Key], int.Parse(value));
                            }
                        }
                    }
                }
            }
        }
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log(result);
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
