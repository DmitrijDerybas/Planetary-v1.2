using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Shows save/load indicator.
public class SaveLoadIndicator : MonoBehaviour
{
    public Text txt_saveLoad;
    public CanvasGroup canvasGroup;

    public void SaveIndicator(float time = 100)
    {
        txt_saveLoad.text = "Saving...";
        canvasGroup.alpha = 1;
        TimeToDisable(time);
    }

    public void LoadIndicator(float time = 100)
    {
        txt_saveLoad.text = "Loading...";
        canvasGroup.alpha = 1;
        TimeToDisable(time);
    }

    void TimeToDisable(float time)
    {
        maxTimer = time;
        timer = 0;
        indicator = true;
    }

    bool indicator = false;
    float timer;
    float maxTimer = 300;
    private void Update()
    {
        if(indicator)
        {
            timer++;
            if(timer > maxTimer)
            {
                indicator = false;
                canvasGroup.alpha = 0;
            }
        }
    }
}
