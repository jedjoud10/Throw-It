using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adds bobbing to the bot in up and down motion
public class BotBobbingScript : MonoBehaviour
{
    public float frequency;//Frequency for the bot motion
    public float amplitude;//How much up and down motion is there ?
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(0, (Mathf.Sin(frequency * Time.time) * amplitude) + amplitude, 0);//Apply bobbing
    }
}
