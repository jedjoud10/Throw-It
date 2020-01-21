using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adds bobbing to the bot in up and down motion
public class BotBobbingScript : MonoBehaviour
{
    public float frequency;//Frequency for the bot motion
    public float amplitude;//How much up and down motion is there ?
    public bool applybobbing = true;//Should we apply bobbing ?
    private const float smoothness = 0.9f;//Smoothness to apply when we stop bobbing
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (applybobbing)
        {
            transform.localPosition = new Vector3(0, (Mathf.Sin(frequency * Time.time) * amplitude) + amplitude, 0);//Apply bobbing
        }
        else transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, smoothness*Time.deltaTime);
    }
}
