using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores links to planets. Checks if the player has won the game.
public class Level : MonoBehaviour
{
    bool isInitialized = false;
    GlobalController controller;
    public GameObject sun;
    public List<Planet> planets = new List<Planet>();
    public GameObject worldCanvas;

    public void Init(GlobalController controller)
    {
        this.controller = controller;

        for (int i = 0; i < planets.Count; i++)
            planets[i].Init(controller);

        isInitialized = true;
    }

    //If there is no enemies left - shows the win menu.
    public void CheckForWin()
    {
        bool noEnemiesLeft = true;

        for (int i = 0; i < planets.Count; i++)
        {
            if(!planets[i].saveData.controlledByPlayer && planets[i].saveData.isAlive)
            {
                noEnemiesLeft = false;
                break;
            }
        }

        if (noEnemiesLeft)
            PlayerWinsLevel();
    }

    void PlayerWinsLevel()
    {
        controller.gui.ShowWinMenu();
        controller.level.ClearLevel();
    }
}
