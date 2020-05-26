using UnityEngine;
using System.Collections;
//A throwable item
[System.Serializable]
public class Throwable : Item
{
    public int throwableGameObjectID;
    public ThrowableType type;//The type of throwable
    public Vector2 speedRandomness;//How much randomness to apply to speed
    public Vector2 sizeRandomness;//How much randomness to apply to speed
    public Vector2 damageRandomness;//How much randomness to apply to speed

    public float damageVelocityWeight;//How much the velocity changes the damage
    public float lifetime;//time the snowball is allowed to exist
    public float angularVelocityRange;//How much randomness to apply to angular velocity
    public Vector2 rigidbodyForceRange;//How much randomness to apply to rigidbody hit force
}
[System.Serializable]
public enum ThrowableType
{
    general, snowball
}