using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script to handle rotation of track of bot
public class TrackScript : MonoBehaviour
{
    public Vector3 offset;//Offset of start position of ray
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position + offset, Vector3.down * 1000, out hit)) //Raycast with offset to ground
        {
            Debug.DrawRay(transform.position + offset, Vector3.down * hit.distance, Color.black);//Debug
            transform.LookAt(hit.point);//Make track rotate to look in the correct direction
        }
    }
}
