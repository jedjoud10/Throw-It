using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script to handle rotation of track of bot
public class TrackScript : MonoBehaviour
{
    public Transform offset;//Offset of start position of ray
    public float distanceOffset;//Offset of distance hit
    private Vector3 point;//The hit point
    private const float Smoothness = 5f;//Smoothnes from last frame hitpoint to current
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(offset.position, Vector3.down * 1000, out hit)) //Raycast with offset to ground
        {
            point.x = hit.point.x; point.z = hit.point.z;//Default point without smoothing
            point.y = Mathf.Lerp(point.y, hit.point.y, Smoothness * Time.deltaTime);//Smooth y-axis point, so smoothing affects track rotation too
            Debug.DrawLine(offset.position, point + Vector3.up * distanceOffset, Color.black);//Debug
            transform.LookAt(point + Vector3.up * distanceOffset);//Make track rotate to look in the correct direction
        }
    }
}
