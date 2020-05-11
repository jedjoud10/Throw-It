using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script only used for bot "ScrapBot"

//Auto add scripts if they are not already on the gameobject
//TODO : Networking support
[RequireComponent(typeof(BotMovementScript))]
public class ScrapBotScript : BotScript
{
    private BotStraightPathScript straightpathScript;//Goes straight to the specified destination
    private PelletThrowingScript pelletThrowingScript;//The pellet throwing script that throws pellet
    public Animator animator;//The animator of the scrapbot
    public float animatorSlowdownSpeed;//The speed of how fast we are going to slowdown the animator's playback speed
    private float animatorSpeed;//How fast the current animation of the animator is playing


    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();//Call parent class start method

        //Initialize scripts
        straightpathScript = GetComponent<BotStraightPathScript>();
        pelletThrowingScript = GetComponent<PelletThrowingScript>();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();//Call parent class update method
        if (isDead) 
        {
            animatorSpeed = Mathf.Lerp(animatorSpeed, 0, animatorSlowdownSpeed * Time.deltaTime);//Go smoothly to zero, thus making animation smoothly stop
            animator.speed = animatorSpeed;//Set new playback speed
        }
    }
    //When bot dies
    override public void Death() 
    {
        base.Death();

        pelletThrowingScript.isThrowing = false;//Stop shooting pellets
    }
}
