using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;

public class AzureFunction : MonoBehaviour
{
    private string cloudScriptFunction = "WoodHttpTrigger";

    public static AzureFunction instance;

    public static int launchCount;

    public static Action onGetLaunchCountSuccess;

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
    }

    private void OnEnable()
    {
        PlayfabManager.onLoginSuccessAction += UpdateLaunchCount;
    }

    private void OnDisable()
    {
        PlayfabManager.onLoginSuccessAction -= UpdateLaunchCount;
    }

    void UpdateLaunchCount()
    {
        ExecuteFunctionRequest request = new ExecuteFunctionRequest
        {
            FunctionName = cloudScriptFunction,
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true
        };
        PlayFabCloudScriptAPI.ExecuteFunction(request, OnCloudScriptSuccess, OnCloudScriptFailure);
    }

    private void OnCloudScriptSuccess(ExecuteFunctionResult result)
    {
        launchCount = int.Parse(result.FunctionResult.ToString());
        onGetLaunchCountSuccess?.Invoke();
    }

    private void OnCloudScriptFailure(PlayFabError error)
    {
        Debug.LogError("Cloud Script Error: " + error.ErrorMessage);
    }
}
