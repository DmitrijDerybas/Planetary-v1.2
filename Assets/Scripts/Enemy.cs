using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the enemy AI.
public class Enemy : MonoBehaviour
{
    GlobalController controller;
    Planet planet;
    bool isInitialized = false;

    Player target;
    float attackDist = 80f;
    float aimSpeed = 2f;

    public Missile missileTypeObj;
    float reloadingTimer = 0;
    bool isReloading = false;

    public void Init(GlobalController controller)
    {
        this.controller = controller;
        target = controller.player;
        planet = GetComponent<Planet>();
        missileTypeObj = controller.missile.GetRandomMissile();
        planet.saveData.missleType = missileTypeObj.type;
        planet.SetMissileSprite(missileTypeObj.GetComponent<SpriteRenderer>().sprite);

        isInitialized = true;
    }

    //Sets the enemy missile by specified type.
    public void SetMissleByType(MissileType type)
    {
        missileTypeObj = controller.missile.GetMissleByType(type);
        planet.SetMissileSprite(missileTypeObj.GetComponent<SpriteRenderer>().sprite);
    }

    void Update()
    {
        if (!isInitialized || controller.isPaused)
            return;

        //Moves the planet around the Sun.
        planet.MovePlanet();
        //Checks if the player is nearby.
        CheckTarget();

        //Handles reloading.
        if (isReloading)
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

    //Checks if the player is nearby.
    void CheckTarget()
    {
        if (target == null)
            target = controller.player;

        //If player is nearby - rotates pivot and shoots.
        if (target != null && target.gameObject.activeSelf && Vector2.Distance(transform.position, target.transform.position) < attackDist)
        {
            Vector3 targetDir = target.transform.position - planet.pivot.position;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            planet.pivot.localRotation = Quaternion.SlerpUnclamped(planet.pivot.localRotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * aimSpeed);

            Shoot();
        }
        //If the player is far away - rotates around self.
        else planet.pivot.Rotate(new Vector3(0, 0, planet.saveData.moveSpeed * 5), Space.Self);
    }

    //Shotts the missile.
    void Shoot()
    {
        if (isReloading)
            return;

        isReloading = true;
        controller.missile.CreateMissileInPos(planet.missilePos.position, planet.missilePos.rotation, missileTypeObj.type, planet);
    }
}
