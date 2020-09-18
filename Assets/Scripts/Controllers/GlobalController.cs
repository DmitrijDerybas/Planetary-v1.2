using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class for all controllers except Global.
public abstract class Controller : MonoBehaviour
{
    protected bool isInitialized = false;

    [Header("Global Controller")]
    public GlobalController controller;
    public virtual void Init(GlobalController controller)
    {
        this.controller = controller;
    }
}

//Global controller/manager. Initializes child controllers. Provides access to all controllers. 
public class GlobalController : MonoBehaviour
{
    #region Singletone
    //Creates a global controller when the application starts.
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        instance = Instantiate(Resources.Load<GameObject>("_Global")).GetComponent<GlobalController>();
        instance.Init();
    }

    private static GlobalController instance;
    public static GlobalController Get() { return instance; }
    #endregion

    public bool isPaused = false;

    [Header("Controllers")]
    public SaveLoadController save; //Controller for saving and loading data.
    public AudioController sfx; //Controller for playing sound.
    public GUIController gui; //Controller for menus/buttons
    public LevelController level; //Controller for level and gameplay.
    public InputController input; //Controller for input.
    public MissileController missile; //Controller for creating missiles.
    public EffectsController effect; //Controller for creating special effects.

    [HideInInspector]
    public Player player; //Link for current player.

    //Initializes all controllers.
    public void Init()
    {
        save.Init(this);
        sfx.Init(this);
        gui.Init(this);
        level.Init(this);
        input.Init(this);
        missile.Init(this);
        effect.Init(this);

        DontDestroyOnLoad(gameObject);
    }

    //Global pause.
    public void SetPause(bool val)
    {
        isPaused = val;
        Time.timeScale = val ? 0 : 1;
    }
}
