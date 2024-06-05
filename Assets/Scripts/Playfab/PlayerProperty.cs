using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : DBSyncSynchronizer
{
    public static PlayerProperty instance;

    [SyncWithDatabase]
    public int collectedWoodCount;

    [SyncWithDatabase]
    public int characterLanguageIndex;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public override void AddDBKeys() // Adding json key to the database with its own Script name to the action of classes with DBSyncSynchronizer superclass
    {
        base.AddDBKeys();
        PlayfabManager.instance.dbKeys[GetType().Name] = this;
    }

    [ContextMenu("SaveData")]
    public override void SaveData()
    {
        base.SaveData();
        PlayfabManager.instance.SaveData(GetType().Name);
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

}