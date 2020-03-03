using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Throws pellet for the scrap bot
public class ScrapBotPelletThrowingScript : MonoBehaviour
{
    public float throwDelay;//Delay between each throw
    public bool isThrowing;//Are we throwing ?
    public GameObject pelletprefab;//Pellet prefab
    public Transform pelletOrigin;//The origin of the throw if the pellet
    private GameObject pellet;//The spawned new pellet
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ThrowPellet", 0, throwDelay);//Call throwpellet method every throwDelay second
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Throws pellet at a constant rate
    private void ThrowPellet() 
    {
        if (isThrowing) 
        {
            pellet = Instantiate(pelletprefab, pelletOrigin.position, pelletOrigin.rotation);//Throw pellet from pellet origin
            pellet.GetComponent<SnowballMovementScript>().InitSnowball(1.0f, null);//Init snowball
        }
    }
}
