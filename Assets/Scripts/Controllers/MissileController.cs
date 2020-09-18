using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissileType
{
    Light,
    Heavy,
    FastReload
}

//Stores and creates new missiles.
public class MissileController : Controller
{
    public Transform missileContainer;
    public List<Missile> missilesPool = new List<Missile>();

    public override void Init(GlobalController controller)
    {
        base.Init(controller);
        isInitialized = true;
    }

    //Returns a random missile from missilesPool.
    public Missile GetRandomMissile()
    {
        return missilesPool[Random.Range(0, missilesPool.Count)];
    }

    //Finds a missile of a specific type.
    public Missile GetMissleByType(MissileType type)
    {
        for (int i = 0; i < missilesPool.Count; i++)
        {
            if (missilesPool[i].type == type)
                return missilesPool[i];
        }

        return null;
    }

    //Places any innactive missile of a certain type in position (pos), with rotation (rot) and stores planet owner (owner).
    public void CreateMissileInPos(Vector2 pos, Quaternion rot, MissileType type, Planet owner)
    {
        for (int i = 0; i < missilesPool.Count; i++)
        {
            if(!missilesPool[i].gameObject.activeSelf && missilesPool[i].type == type)
            {
                missilesPool[i].transform.position = pos;
                missilesPool[i].transform.rotation = rot;
                missilesPool[i].owner = owner;
                missilesPool[i].gameObject.SetActive(true);
                controller.sfx.PlaySound("shoot");
                return;
            }
        }

        //All missiles are busy. Creating new one and adding it to the missiles pool.
        Missile mToCreate = Instantiate(GetMissleByType(type).gameObject, missileContainer).GetComponent<Missile>();
        mToCreate.rbody.velocity = Vector2.zero;
        mToCreate.transform.position = pos;
        mToCreate.transform.rotation = rot;
        mToCreate.owner = owner;
        mToCreate.gameObject.SetActive(true);
        controller.sfx.PlaySound("shoot");
        missilesPool.Add(mToCreate);
    }

}
