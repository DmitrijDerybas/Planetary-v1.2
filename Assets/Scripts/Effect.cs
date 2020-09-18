using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores effect type.
public class Effect : MonoBehaviour
{
    public bool isPlaying = false;
    public EffectType type;

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
