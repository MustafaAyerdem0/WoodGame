using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;
using System;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;
    public static Action dbSyncAction;
    public static Action onDataReceivedAction;
    public Dictionary<string, DBSyncSynchronizer> dbKeys = new Dictionary<string, DBSyncSynchronizer>();
    public static string displayName;

    public PlayerProperty playerProperty;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        else
        {
            instance = this;
        }

        playerProperty = FindObjectOfType<PlayerProperty>();
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = CanvasManager.instance.R_mailText.text,
            DisplayName = CanvasManager.instance.R_playerNameText.text,
            Password = CanvasManager.instance.R_passwordText.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = CanvasManager.instance.L_mailText.text,
            Password = CanvasManager.instance.L_passwordText.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);

        CanvasManager.instance.ConnectionStatusPanel.SetActive(true);
    }

    private void GetPlayerProfile()
    {
        var request = new GetPlayerProfileRequest();

        PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnGetPlayerProfileFailure);
    }

    private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
    {
        displayName = result.PlayerProfile.DisplayName;
        LaunchManager.instance.SetDisplayName(displayName);
        CanvasManager.instance.OpenProfilePanel();
    }

    private void OnGetPlayerProfileFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get player profile: " + error.GenerateErrorReport());
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        CanvasManager.instance.statusText.text = "Login Success!";
        GetPlayerProfile();
        dbSyncAction?.Invoke();
        GetAllData();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register Successful");
        CanvasManager.instance.statusText.text = "Register Successful!";
        // Kayıt başarılı olduğunda yapılacak işlemler
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Failure: " + error.ErrorMessage);
        CanvasManager.instance.statusText.text = "Failure: " + error.GenerateErrorReport();
        // Hata durumunda yapılacak işlemler
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

        onDataReceivedAction?.Invoke();
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log(result);
    }


}
