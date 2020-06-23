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
    public bool isGrounded;//Is the player on the ground
    [Range(-1f, 1f)]
    public float groundedThresholdAngle;
    // Start is called before the first frame update
    void Start()
    {
        apply = IsOwner;
        rb = GetComponent<Rigidbody>();
    }
    private void LateUpdate()
    {
        isGrounded = false;//Default
    }
    // FixedUpdate is called each physics timestep
    void FixedUpdate()
    {
        if(apply) rb.AddForce((new Vector3(inputVelocity.x, 0, inputVelocity.y) - new Vector3(rb.velocity.x, 0, rb.velocity.z)) * Time.fixedDeltaTime * acceleration, ForceMode.VelocityChange);
    }
    private void OnCollisionStay(Collision collision)
    {
        float minDotNormal = 0;
        foreach (var contacts in collision.contacts)
        {
            Debug.DrawRay(contacts.point, contacts.normal);
            if (Vector3.Dot(contacts.normal, Vector3.up) > minDotNormal)
            {
                minDotNormal = Vector3.Dot(contacts.normal, Vector3.up);//Check if there is ground below us
            }
        }
        isGrounded = minDotNormal > groundedThresholdAngle;
    }
}
