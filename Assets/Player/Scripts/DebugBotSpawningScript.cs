using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Spawning of placeholder with "m" and scrapbot with "n"
public class DebugBotSpawningScript : MonoBehaviour
{
    public Transform cam;//Camera
    public Vector3 offset;
    public float distance;
    private Vector3 point;//End point
    private RaycastHit hit;
    public GameObject placeholderbot;
    public GameObject scrapbot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cam.position, cam.forward * distance, out hit)) 
        {
            point = hit.point;
        }
        else 
        {
            point = cam.position + cam.forward * distance;
        }
        if (Input.GetKeyDown(KeyCode.M)) Instantiate(placeholderbot, point + offset, Quaternion.identity);
        if (Input.GetKeyDown(KeyCode.N)) Instantiate(scrapbot, point + offset, Quaternion.identity);
    }
    //Gizmoo
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point + offset, 0.5f);
    }
}
