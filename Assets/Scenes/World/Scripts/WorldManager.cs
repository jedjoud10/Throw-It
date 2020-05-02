using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Handles communications between multiple scripts and classes
public class WorldManager : MonoBehaviour
{
    public bool CalculatePathesAtStart = true;//Should we calculate bot pathfinding at the start of the game ?
    // Start is called before the first frame update
    void Start()
    {
        if (CalculatePathesAtStart)
        {
            FindObjectOfType<AStarPathfinder>().MakeTerrainGrid();//Init base terrain
            StartCoroutine("WorldUpdateCoroutine");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Called internally when map has changed
    private IEnumerator WorldUpdateCoroutine() 
    {
        #region Bots path calculations/recalculations
        //Recalculates every bots's path and updates pathfinding grid
        BotPathfinderScript[] pathfinders = FindObjectsOfType<BotPathfinderScript>();
        if (pathfinders != null || pathfinders.Length != 0)//Recalculate pathes since we have valid pathfinding bots
        {
            FindObjectOfType<AStarPathfinder>().MakeGrid();//Recalculate grid
            yield return new WaitForSecondsRealtime(1.0f);
            for (int i = 0; i < pathfinders.Length; i++)
            {
                yield return new WaitForSecondsRealtime(1.0f);
                pathfinders[i].FindPath();
            }
        }
        #endregion
    }
    //Called externally by scripts to start coroutine to start map update
    public void WorldUpdate() 
    {
        StartCoroutine("WorldUpdateCoroutine");
    }
    //Switch to the world map
    public void StartWorldMap() { ChangeScene("TestMap"); }
    //Switches to a specific map
    public void ChangeScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
