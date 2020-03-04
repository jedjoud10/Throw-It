using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script only used for bot "ScrapBot"

//Auto add scripts if they are not already on the gameobject
[RequireComponent(typeof(BotMovementScript))]
public class ScrapBotScript : BotScript
{    
    private BotStraightPathScript straightpathScript;//Goes straight to the destination specified
    private PelletThrowingScript pelletThrowingScript;//The pellet throwing script that throws pellet
    public Animator scrapBotAnimator;//The animator of the scrapBot that handles animations

    public float slowDownSpeed;//The speed of howhow fast we slow-down the bot current animation when we are dead
    private float scrapBotAnimationSpeed;//Current speed of scrapBot's animator
    // Start is called before the first frame update
    public override void Start()
    {
        //Init components
        pelletThrowingScript = GetComponent<PelletThrowingScript>();
        straightpathScript = GetComponent<BotStraightPathScript>();

        //Init speed so it matches up later in the Update function when we slow it down
        scrapBotAnimationSpeed = scrapBotAnimator.speed;

        base.Start();
    }
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!alive)//Death
        {
            //Slow down current scrapBot animation
            scrapBotAnimationSpeed = Mathf.Lerp(scrapBotAnimationSpeed, 0.0f, Time.deltaTime * slowDownSpeed);//Slowly go to 0.0 with smoothing
            scrapBotAnimator.speed = scrapBotAnimationSpeed;//Set new speed
        }
    }
    //Called when bot dies
    public override void Death()
    {
        base.Death();
        pelletThrowingScript.isThrowing = false;
    }
}
