using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GameSave
{
    public PlanetData[] pdata;

    public void CreateSaveData(Level curLevel)
    {
        pdata = new PlanetData[curLevel.planets.Count];
        for (int i = 0; i < curLevel.planets.Count; i++)
            pdata[i] = curLevel.planets[i].saveData;
    }
}

public abstract class SavePlatform : MonoBehaviour
{
    protected RuntimePlatform platform;
    protected bool isInitialized = false;
    protected SaveLoadController controller;
    public abstract void Init(SaveLoadController controller);
    public abstract void Save(GameSave save);
    public abstract bool IsSaveExist();
    public abstract GameSave Load();

    public virtual RuntimePlatform GetPlatform() { return this.platform; }
}

//Handles platform selecting (for multiplatform). Saves and loads the data. 
public class SaveLoadController : Controller
{
    public SavePlatform currentSavePlatform;

    public delegate void OnGameLoadDelegate(GameSave save);
    public event OnGameLoadDelegate OnGameLoadEvent;

    public GameSave gameSave;
    public bool isGameLoaded = false;

    public SaveLoadIndicator indicator;

    public override void Init(GlobalController controller)
    {
        base.Init(controller);

        //Get all attached platforms, sets current platform and removes all other platforms
        SavePlatform[] platforms = GetComponentsInChildren<SavePlatform>();
        for (int i = 0; i < platforms.Length; i++)
        {
            if (Application.platform.HasFlag(platforms[i].GetPlatform()))
            {
                currentSavePlatform = platforms[i];
                RemoveOtherPlatforms(platforms, Application.platform);
                platforms = null;
                break;
            }
        }

        currentSavePlatform.Init(this);
    }

    //Checks if there is a save data.
    public bool IsSaveExist()
    {
        return currentSavePlatform.IsSaveExist();
    }

    //Removes unnecessary platforms.
    void RemoveOtherPlatforms(SavePlatform[] platforms, RuntimePlatform p)
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if (p != platforms[i].GetPlatform())
            {
                Destroy(platforms[i]);
            }
        }
    }

    //Loads the data.
    public void Load()
    {
        gameSave = currentSavePlatform.Load();
        indicator.LoadIndicator();
        isGameLoaded = true;
    }

    //Saves the data.
    public void Save()
    {
        indicator.SaveIndicator();
        currentSavePlatform.Save(gameSave);
    }
}
