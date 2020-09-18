using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Missile missileTypeObj;
    float reloadingTimer = 0;
    bool isReloading = false;

    GlobalController controller;
    Planet planet;
    bool isInitialized = false;

    public void Init(GlobalController controller)
    {
        this.controller = controller;
        this.controller.input.OnInputEvent += Input_OnInputEvent;
        planet = GetComponent<Planet>();
        //Sets the current planet to controlled by player value,
        planet.saveData.controlledByPlayer = true;
        //Handles players planet hit.
        planet.OnHitEvent += Planet_OnHitEvent;
        //Handles players planet death.
        planet.OnDeathEvent += Planet_OnDeathEvent;
        //Gets the random missile for the player's planet.
        missileTypeObj = controller.missile.GetRandomMissile();
        planet.saveData.missleType = missileTypeObj.type;
        planet.SetMissileSprite(missileTypeObj.GetComponent<SpriteRenderer>().sprite);
        planet.speedMod = .05f;

        isInitialized = true;
    }

    public void SetMissleByType(MissileType type)
    {
        missileTypeObj = controller.missile.GetMissleByType(type);
        planet.SetMissileSprite(missileTypeObj.GetComponent<SpriteRenderer>().sprite);
    }

    //If the player runs out of life - shows the lose menu.
    private void Planet_OnDeathEvent()
    {
        controller.SetPause(true);
        controller.gui.ShowLoseMenu();
    }

    //Plays the (red screen) hit effect.
    private void Planet_OnHitEvent()
    {
        controller.gui.PlayHitEffect();
    }

    private void OnDestroy()
    {
        planet.OnHitEvent -= Planet_OnHitEvent;
        planet.OnDeathEvent -= Planet_OnDeathEvent;
    }

    //Handles input from the player.
    private void Input_OnInputEvent(InputButtonType button, InputEventType inputEvent, int joyID = 0)
    {
        if (!planet.saveData.isAlive)
            return;

        if (inputEvent == InputEventType.Pressed)
        {
            switch (button)
            {
                case InputButtonType.Cancel:
                    controller.gui.TogglePauseMenu();
                    break;
            }
        }

        if (planet.saveData.isAlive && inputEvent == InputEventType.Continuous && !controller.isPaused)
        {
            switch(button)
            {
                case InputButtonType.MouseLeft:
                    Shoot();
                    break;
                    #region ManualControlls
                    //In case we need to controll our planet manually. Comment out planet.MovePlanet() in Update method.
                    /*
                    case InputButtonType.Left:
                        planet.MovePlanet(1);
                        break;
                    case InputButtonType.Right:
                        planet.MovePlanet(-1);
                        break;
                    */
                    #endregion
            }
        }
    }

    //Shoots if reloading is completed.
    void Shoot()
    {
        if (isReloading)
            return;

        isReloading = true;
        controller.missile.CreateMissileInPos(planet.missilePos.position, planet.missilePos.rotation, missileTypeObj.type, planet);
    }

    void Update()
    {
        if (!isInitialized || !planet.saveData.isAlive || controller.isPaused)
            return;

        //The camera is following the player
        controller.gui.MoveCameraSmoothToPos(transform.position); 

        //Moves the planet around the sun.
        planet.MovePlanet();
        Vector3 mousePos = controller.input.GetMouseWorldPos();
        Vector3 perpendicular = Vector3.Cross(mousePos - transform.position, Vector3.forward);
        planet.pivot.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);

        //Zooms in and out.
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            controller.gui.Zoom(-1);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            controller.gui.Zoom(1);

        //Handles reloading.
        if(isReloading)
        {
            reloadingTimer += Time.deltaTime;
            planet.SetReloadImage(true, reloadingTimer / missileTypeObj.reloadTime);
            if (reloadingTimer >= missileTypeObj.reloadTime)
            {
                reloadingTimer = 0;
                isReloading = false;
                planet.SetReloadImage(false, 0);
            }
        }
    }
}
