using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    None,

    MissileExplode
}

//Handles the effects pool.
public class EffectsController : Controller
{
    public List<Effect> effects = new List<Effect>();

    public override void Init(GlobalController controller)
    {
        base.Init(controller);

        isInitialized = true;
    }

    //Creates effect by type in specified position and rotation.
    public void CreateEffectInPos(EffectType type, Vector3 pos, Quaternion rot, bool forced = false)
    {
        if(forced)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].type == type)
                {
                    effects[i].gameObject.SetActive(true);
                    effects[i].transform.position = pos;
                    effects[i].transform.rotation = rot;
                    return;
                }
            }

            InstantiateNewEffect(type, pos, rot);
        }
        //If NOT "forced" checks if the effect object in innactive.
        else
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i] != null && effects[i].gameObject != null && !effects[i].gameObject.activeSelf && effects[i].type == type)
                {
                    effects[i].gameObject.SetActive(true);
                    effects[i].transform.position = pos;
                    effects[i].transform.rotation = rot;
                    return;
                }
            }

            InstantiateNewEffect(type, pos, rot);
        }
    }

    //Creates new effect and adds it to the pool if there was no free effects.
    void InstantiateNewEffect(EffectType type, Vector3 pos, Quaternion rot)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if(effects[i] != null && effects[i].gameObject != null && effects[i].type == type)
            {
                Effect e = Instantiate(effects[i]);
                effects.Add(e);
                e.transform.parent = transform;
                e.transform.position = pos;
                e.transform.rotation = rot;
                e.gameObject.SetActive(true);
                return;
            }
        }
    }

    //Disables all effects and puts it to the parent.
    public void ResetAllEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].transform.SetParent(transform);
            effects[i].gameObject.SetActive(false);
        }
    }
}
