using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlanetType
{
    Planet1,
    Planet2,
    Planet3,
    Planet4
}

//Data to be saved when player hits "save" button.
[System.Serializable]
public struct PlanetData
{
    public PlanetType planetType;

    [HideInInspector]
    public MissileType missleType;

    public float health;
    public bool isAlive;
    public float moveSpeed;

    public float X;
    public float Y;

    [HideInInspector]
    public bool controlledByPlayer;
}

public class Planet : MonoBehaviour
{
    //When the planet recieves a damage.
    public delegate void OnHitDelegate();
    public event OnHitDelegate OnHitEvent; 

    //When a planet destroys.
    public delegate void OnHitDeath();
    public event OnHitDelegate OnDeathEvent;

    [HideInInspector]
    public GlobalController controller;
   
    [Header("Save data")]
    public PlanetData saveData;

    [Space]
    public bool isStar;
    public Rigidbody2D rbody;
    public float max_health;
    public float hudSize = 0;
    public float speedMod = 0;
    protected bool isInitialized = false;
    protected Transform sun;

    public SpriteRenderer missileSpr;
    public Transform missilePos;
    public Transform pivot;

    //HUD for reload and life.
    GameObject hud;
    Image hud_lifeImg;
    Image hud_reloadImg;

    public void Init(GlobalController controller)
    {
        this.controller = controller;
        if(isStar)
        {
            isInitialized = true;
            return;
        }

        sun = controller.level.curLevel.sun.transform;

        if (hud == null)
            hud = Instantiate(Resources.Load<GameObject>("Prefabs/HUD/LifeBar"));
        else hud.SetActive(true);

        //Places and resizes the HUD.
        hud.transform.parent = controller.level.curLevel.worldCanvas.transform;
        hud.GetComponent<RectTransform>().sizeDelta = new Vector2(hudSize, hudSize);
        hud_reloadImg = hud.GetComponent<Image>();
        hud_reloadImg.GetComponent<RectTransform>().sizeDelta = new Vector2(hudSize + 1, hudSize + 1);
        hud_reloadImg.enabled = false;
        hud_lifeImg = hud.transform.GetChild(1).GetComponent<Image>();
        hud_lifeImg.GetComponent<RectTransform>().sizeDelta = new Vector2(hudSize, hudSize);
        hud.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(hudSize, hudSize);
        UpdateHud();

        isInitialized = true;
    }

    //Chanes the missiles sprite.
    public void SetMissileSprite(Sprite spr)
    {
        missileSpr.sprite = spr;
    }

    //Destroys the planet and the HUD.
    public void DestroyMe()
    {
        saveData.isAlive = false;
        Destroy(hud);
        Destroy(gameObject);
    }

    //Moves the planet around the Sun.
    public void MovePlanet(int dir = 1)
    {
        if (!isInitialized || isStar || !saveData.isAlive)
            return;

        transform.RotateAround(sun.position, Vector3.forward, saveData.moveSpeed * (dir + speedMod));
        transform.rotation = Quaternion.identity;
        hud.transform.position = transform.position;

        //Places planet position to the data.
        saveData.X = transform.position.x;
        saveData.Y = transform.position.y;
    }

    //When the planet receives the damage.
    public void Hit(float damage)
    {
        if (isStar)
            return;

        controller.sfx.PlaySound("hit");
        saveData.health -= damage;
        UpdateHud();
        OnHitEvent?.Invoke();

        //If the planet runs out of life - destroys itself.
        if (saveData.health <= 0)
        {
            saveData.health = 0;
            Death();
        }
    }

    //Sets the reload image and image fill amound to determine the reload progress.
    public void SetReloadImage(bool active, float fillAmount)
    {
        hud_reloadImg.enabled = active;
        hud_reloadImg.fillAmount = fillAmount;
    }

    //Updates the planet life value progress.
    void UpdateHud()
    {
        if (isStar)
            return;

        hud_lifeImg.fillAmount = (saveData.health / max_health);
    }

    //Destroys the planet when runs out of life.
    void Death()
    {
        if (isStar)
            return;

        saveData.isAlive = false;
        hud.SetActive(false);
        gameObject.SetActive(false);

        //Invokes planet death event.
        OnDeathEvent?.Invoke();

        //Checks if the player wins the level.
        controller.level.curLevel.CheckForWin();
    }
}
