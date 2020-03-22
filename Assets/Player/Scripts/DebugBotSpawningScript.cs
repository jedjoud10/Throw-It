using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Spawning of debug bots
public class DebugBotSpawningScript : MonoBehaviour
{
    public Transform cam;//Camera
    public Vector3 offset;
    public float distance;
    private Vector3 point;//End point
    private RaycastHit hit;
    public GameObject[] bots;//All the debug bots that the player can spawn
    private GameObject currentBot;//The current bot that the player can spawn
    private int botIndex = 0;//The index to select a bot from the bots array
    private bool spawnBot;
    private bool changeBot;

    private DebugControls debugControls;//The inputs controls
    private void Awake()
    {
        //Init input controls for debugging
        debugControls = new DebugControls();
        debugControls.BotSpawning.SpawnBot.performed += ctx => spawnBot = true;
        debugControls.BotSpawning.ChangeBot.performed += ctx => changeBot = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentBot = bots[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cam.position, cam.forward, out hit, distance)) 
        {
            point = hit.point;
        }
        else 
        {
            point = cam.position + cam.forward * distance;
        }
        if (changeBot) 
        {
            botIndex += 1;//Change to select next bot
            currentBot = bots[botIndex % bots.Length];
        }
        if (spawnBot) 
        {
            Instantiate(currentBot, point, Quaternion.identity);
        }
        spawnBot = false;
        changeBot = false;
    }
    //Gizmoo
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point + offset, 0.5f);
    }

    #region Enable/Disable
    private void OnEnable()
    {
        debugControls.Enable();
    }
    private void OnDisable()
    {
        debugControls.Disable();
    }
    #endregion 
}
