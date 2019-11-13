using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Snowball throwing for player
public class SnowballThrowingScript : MonoBehaviour
{
    public GameObject Snowball;//The snowball that we are going to throw
    public Transform ThrowPoint;//The point where the snowball is throwed
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//Dedect left mouse button click
        {
            ThrowSnowball();
        }
    }
    public void ThrowSnowball()//Throw snowball method
    {
        Instantiate(Snowball, ThrowPoint.position, ThrowPoint.rotation);//Throw snowball
    }
}
