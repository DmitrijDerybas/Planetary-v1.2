using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores current level data, planet assets. Generates enemies an player. Generates enemies and player from save data. 
public class LevelController : Controller
{
    public Level curLevel;
    public Planet[] planetTypes;

    public override void Init(GlobalController controller)
    {
        base.Init(controller);

        if (curLevel == null)
            curLevel = GameObject.FindObjectOfType<Level>();

        isInitialized = true;
    }

    //Clears level from planets and effects.
    public void ClearLevel()
    {
        for (int i = 0; i < curLevel.planets.Count; i++)
            curLevel.planets[i].DestroyMe();

        curLevel.planets.Clear();
        controller.effect.ResetAllEffects();
    }

    //Finds specific planet by type from planets assets.
    Planet FindPlanetByType(PlanetType type)
    {
        for (int i = 0; i < planetTypes.Length; i++)
        {
            if (planetTypes[i].saveData.planetType == type)
                return planetTypes[i];
        }

        return null;
    }
    
    //Generates level from save file. Creates players and enemies.
    public void GenerateLevelFromSaveFile()
    {
        Transform planetContainer = curLevel.sun.transform.parent;
        GameSave gs = controller.save.gameSave;

        for (int i = 0; i < gs.pdata.Length; i++)
        {
            if (!gs.pdata[i].isAlive)
                continue;

            //Instantiates a planet from a save file and places it at the current level.
            GameObject planet = Instantiate(FindPlanetByType(gs.pdata[i].planetType).gameObject, planetContainer);
            planet.transform.position = new Vector3(gs.pdata[i].X, gs.pdata[i].Y, 0);
            planet.GetComponent<Planet>().saveData = gs.pdata[i];
            curLevel.planets.Add(planet.GetComponent<Planet>());

            //Creates and inits a player from a save file.
            if (gs.pdata[i].controlledByPlayer)
            {
                Player p = planet.AddComponent<Player>();
                p.Init(controller);
                p.SetMissleByType(gs.pdata[i].missleType);

                controller.player = p;
            }
            //Creates and inits an enemy from a save file.
            else
            {
                Enemy e = planet.AddComponent<Enemy>();
                e.Init(controller);
                e.SetMissleByType(gs.pdata[i].missleType);
            }
        }
        
        //Inits current level and sun. Hides GUI.
        curLevel.Init(controller);
        curLevel.sun.GetComponent<Planet>().Init(controller);
        controller.gui.HideMenu();
    }

    //Generating level with enemies count equal pCount.
    public void GenerateLevel(int pCount = 2)
    {
        Vector3 sunPos = curLevel.sun.transform.position;
        Transform planetContainer = curLevel.sun.transform.parent;

        //Planets creation.
        for (int i = 0; i < pCount; i++)
        {
            //Instantiates a random planet from a planets types array and places it at the current level.
            GameObject planet = Instantiate(planetTypes[Random.Range(0, planetTypes.Length)].gameObject, planetContainer);
            Vector2 planetPos = sunPos;
            int dir = Random.Range(0, 100) > 50 ? 1 : -1;
            planetPos.x = (35 + (i*20)) * dir;
            planet.transform.position = planetPos;
            curLevel.planets.Add(planet.GetComponent<Planet>());
        }

        //Initializes current level.
        curLevel.Init(controller);

        //Player initialization.
        int playerPlanetId = Random.Range(0, pCount);
        controller.player = curLevel.planets[playerPlanetId].gameObject.AddComponent<Player>();
        controller.player.Init(controller);

        //Enemies initialization.
        for (int i = 0; i < curLevel.planets.Count; i++)
        {
            if(i != playerPlanetId)
            {
                Enemy e = curLevel.planets[i].gameObject.AddComponent<Enemy>();
                e.Init(controller);
            }
        }

        //Intializes a sun and hides a menu.
        curLevel.sun.GetComponent<Planet>().Init(controller);
        controller.gui.HideMenu();
    }
}
