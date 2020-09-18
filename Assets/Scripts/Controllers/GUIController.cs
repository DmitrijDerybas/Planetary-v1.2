using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Shows the menu. Moves the graphic cursor. Handles buttons/
public class GUIController : Controller
{
    public Camera camera_main;

    public Transform cursor;
    public GameObject menu;

    public Animator animation_hitEff; //Animation of hitting the player.

    [Header("Text")]
    public Text txt_winLose;

    [Header("Buttons")]
    public GameObject btn_save;
    public GameObject btn_load;

    [Range(1, 4)]
    public int enemiesCountMin = 1; //Minimum enemies to generate.
    [Range(1, 4)]
    public int enemiesCountMax = 4; //Maximum enemies to generate.

    public override void Init(GlobalController controller)
    {
        base.Init(controller);

        if (camera_main == null)
            camera_main = Camera.main;

        //Makes button "Load" interactable and vise versa when there is some data saved.
        btn_load.GetComponent<Button>().interactable = controller.save.IsSaveExist();
        
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        //Places the graphic cursor to real cursor pos.
        cursor.position = controller.input.mousePos;
    }

    //The camera follows the player.
    public void MoveCameraSmoothToPos(Vector3 pos)
    {
        pos.z = camera_main.transform.position.z;
        camera_main.transform.position = Vector3.Lerp(camera_main.transform.position, pos, .1f);
    }

    //Zooms in and out when the player turns the mouse wheel.
    public void Zoom(int dir)
    {
        camera_main.fieldOfView = Mathf.Clamp(camera_main.fieldOfView + dir, 15, 80);
    }

    //When the player presses "Load" button.
    public void Btn_LoadGame()
    {
        //Loads a data from the save file.
        controller.save.Load();
        //Clears the level before generating it from the save file.
        controller.level.ClearLevel();
        //Generates the level from the save file.
        controller.level.GenerateLevelFromSaveFile();
    }

    //When the player presses "Save" button.
    public void Btn_SaveGame()
    {
        //Creates and writes a save data.
        controller.save.gameSave.CreateSaveData(controller.level.curLevel);
        controller.save.Save();
        HideMenu();
    }

    public void Btn_NewGame()
    {
        if (enemiesCountMin > enemiesCountMax)
            enemiesCountMin = enemiesCountMax;

        int r = UnityEngine.Random.Range(enemiesCountMin, enemiesCountMax+1);
        r++; //One planet for player;

        //Clears the level before start
        controller.level.ClearLevel();
        //Generates random number of enemies from r num.
        controller.level.GenerateLevel(r);
    }

    //Quits the app.
    public void Btn_Quit()
    {
        Application.Quit();
    }

    //Shows the lose menu when the player dies.
    public void ShowLoseMenu()
    {
        controller.SetPause(true);

        menu.SetActive(true);
        txt_winLose.gameObject.SetActive(true);
        txt_winLose.text = "YOU LOSE!";
        txt_winLose.color = Color.red;
        btn_save.SetActive(false);

        btn_load.GetComponent<Button>().interactable = controller.save.IsSaveExist();
    }

    //Shows the menu when the player presses the escape button.
    public void ShowPauseMenu()
    {
        controller.SetPause(true);

        menu.SetActive(true);
        txt_winLose.gameObject.SetActive(false);
        btn_save.SetActive(true);
        btn_load.GetComponent<Button>().interactable = controller.save.IsSaveExist();
    }

    //Shows or hides the menu when the player presses the escape button.
    public void TogglePauseMenu()
    {
        if(controller.isPaused)
            HideMenu();
        else
            ShowPauseMenu();
    }

    //Hides the menu.
    public void HideMenu()
    {
        controller.SetPause(false);

        menu.SetActive(false);
        txt_winLose.gameObject.SetActive(false);
    }

    //Plays effect when a missile hits a player.
    public void PlayHitEffect()
    {
        animation_hitEff.Play("HitEff");
    }

    //Shows menu when the player destroys all enemies.
    public void ShowWinMenu()
    {
        controller.SetPause(true);

        menu.SetActive(true);
        txt_winLose.gameObject.SetActive(true);
        txt_winLose.text = "YOU WIN!";
        txt_winLose.color = Color.green;
        btn_save.SetActive(false);

        btn_load.GetComponent<Button>().interactable = controller.save.IsSaveExist();
    }
}
