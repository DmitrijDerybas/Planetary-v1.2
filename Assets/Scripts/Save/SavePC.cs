using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;

//Handles saving and loading data for the PC platform.
public class SavePC : SavePlatform
{
    string FILE_PATH = "";
    public override void Init(SaveLoadController controller)
    {
        FILE_PATH = Application.persistentDataPath + "/save.dat";

        this.controller = controller;
        DefinePlatform();
        isInitialized = true;
    }

    public override RuntimePlatform GetPlatform()
    {
        DefinePlatform();
        return base.GetPlatform();
    }

    void DefinePlatform()
    {
#if UNITY_EDITOR
        this.platform = RuntimePlatform.WindowsEditor;
#elif UNITY_STANDALONE
        this.platform = RuntimePlatform.WindowsPlayer;
#endif
    }

    //Loads the data from the file if exists.
    public override GameSave Load()
    {
        GameSave load = new GameSave();
        if (!File.Exists(FILE_PATH))
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(FILE_PATH, FileMode.Open);
        load = (GameSave)bf.Deserialize(file);
        file.Close();

        return load;
    }

    //Saves the specified data to the file.
    public override void Save(GameSave save)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(FILE_PATH, FileMode.OpenOrCreate);
        bf.Serialize(file, save);
        file.Close();
    }

    //Checks if the save file is exists.
    public override bool IsSaveExist()
    {
        return File.Exists(FILE_PATH);
    }
}
