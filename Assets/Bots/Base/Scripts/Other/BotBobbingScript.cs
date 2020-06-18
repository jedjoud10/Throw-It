using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adds bobbing to the bot in up and down motion
//TODO : Networking support
public class BotBobbingScript : MonoBehaviour
{
    public float frequency;//Frequency for the bot motion
    public float amplitude;//How much up and down motion is there ?
    public bool applybobbing = true;//Should we apply bobbing ?
    private const float smoothness = 0.8f;//Smoothness to apply when we stop bobbing
    private Vector3 bobbing = Vector3.zero;//The bobbing movement that is applied
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (amplitude > 0.002)
        {
            bobbing.y = (Mathf.Sin(frequency * Time.time) * amplitude) + amplitude;
            transform.localPosition = bobbing;//Apply bobbing
        }
        else transform.localPosition = Vector3.zero;//Set pos to zero if amplitude is too low. Saving performence by not calculating sin function

        if (!applybobbing)
        {
            amplitude = Mathf.Lerp(amplitude, 0, smoothness * Time.deltaTime);//Smooth out amplitude to zero
        }
    }
}
