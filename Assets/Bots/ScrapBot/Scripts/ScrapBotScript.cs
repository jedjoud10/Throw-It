using MLAPI.NetworkedVar;
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

    //Throwing pellets
    private ThrowableThrowingScript thrower;//The thrower that is going to throw the pellets
    private NetworkedVarBool throwing = new NetworkedVarBool(false);//Are we allowed to throw pellets ?
    public float pelletThrowingDelay = 0.5f;//How much to wait (in seconds) between each pellet throw

    public Animator animator;//The animator of the scrapbot
    public float animatorSlowdownSpeed;//The speed of how fast we are going to slowdown the animator's playback speed
    private float animatorSpeed;//How fast the current animation of the animator is playing


    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();//Call parent class start method

        //Initialize scripts
        straightpathScript = GetComponent<BotStraightPathScript>();
        thrower = GetComponent<ThrowableThrowingScript>();

        if (IsServer) InvokeRepeating("ThrowPellet", 0, pelletThrowingDelay);
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();//Call parent class update method
        if (isDead.Value) 
        {
            animatorSpeed = Mathf.Lerp(animatorSpeed, 0, animatorSlowdownSpeed * Time.deltaTime);//Go smoothly to zero, thus making animation smoothly stop
            animator.speed = animatorSpeed;//Set new playback speed
        }
    }
    //When bot dies (Only executed on server)
    override public void OnBotDeath() 
    {
        base.OnBotDeath();
        throwing.Value = false;
    }
    //Throws a single pellet (Only executed on server)
    private void ThrowPellet() 
    {
        if (throwing.Value) thrower.ThrowOnServer(1f, "ScrapBot", OwnerClientId, "pellet");
    }
}
