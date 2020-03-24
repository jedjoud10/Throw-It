using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Spawning of current bot with "m" and switching to next bot with "n"
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
        if (Input.GetKeyDown(KeyCode.N)) 
        {
            botIndex += 1;//Change to select next bot
            currentBot = bots[botIndex % bots.Length];
        }
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            Instantiate(currentBot, point, Quaternion.identity);
        }

    }
    //Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point + offset, 0.5f);
    }
}
