using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : DBSyncSynchronizer
{
    [SyncWithDatabase]
    public int collectedWoodCount;

    public override void AddDBKeys()
    {
        base.AddDBKeys();
        PlayfabManager.instance.dbKeys[this.GetType().Name] = this;
    }

    [ContextMenu("SaveData")]
    public override void SaveData()
    {
        base.SaveData();
        PlayfabManager.instance.SaveData(this.GetType().Name);
    }

}