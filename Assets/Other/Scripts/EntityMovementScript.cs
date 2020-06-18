using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
//A script that handles the movement of this rigidbody
public class EntityMovementScript : NetworkedBehaviour
{
    public bool apply;//Should we apply forces to the rigidbody ?
    public float speed;//The speed of this movable entity
    public Vector2 inputVelocity;//The world velocity that we want to move at (exclude the Y axis)
    public float acceleration = 7;//How fast the player velocity will go to the target velocity
    private Rigidbody rb;//This rigidbody
    // Start is called before the first frame update
    void Start()
    {
        apply = IsOwner;
        rb = GetComponent<Rigidbody>();
    }
    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {
        if(apply) rb.AddForce((new Vector3(inputVelocity.x, 0, inputVelocity.y) - new Vector3(rb.velocity.x, 0, rb.velocity.z)) * Time.fixedDeltaTime * acceleration, ForceMode.VelocityChange);
    }
}
