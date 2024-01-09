using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements an interface for all scripts that save and load data
/// </summary>
public interface IDataPersistence
{
    void LoadData(GameData data);

    void SaveData(GameData data);
}