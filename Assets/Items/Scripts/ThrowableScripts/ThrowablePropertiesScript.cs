using MLAPI;
using UnityEngine;
//Holds information for the throwable (ex : size, damage, speed) and might randomize them
public class ThrowablePropertiesScript : NetworkedBehaviour
{
    [HideInInspector]
    public ThrowableType throwableType;
    [HideInInspector]
    public float speed;//Speed force applied at start
    [HideInInspector]
    public float size;//Size of snowball
    [HideInInspector]
    public int damage;//Damage applied to someone/something when it collides with the snowball
    [HideInInspector]
    public float rigidbodyForce;//Force applied to every physics object when we hit it
    [HideInInspector()]
    public Vector3 angularVelocity;//The angular velocity of the snowball at throw 
    [HideInInspector]
    public float damageVelocityWeight;//How much the velocity changes the damage

    [Header("Randomness")]
    //Speed force applied at start
    private Vector2 speedRandomness;//How much randomness to apply to speed
    //Size of snowball
    private Vector2 sizeRandomness;//How much randomness to apply to speed
    //Damage applied to someone/something when it collides with the snowball
    private Vector2 damageRandomness;//How much randomness to apply to speed

    private float lifetime;//time the snowball is allowed to exist
    private float angularVelocityRange;//How much randomness to apply to angular velocity
    private Vector2 rigidbodyForceRange;//How much randomness to apply to rigidbody hit force
    [HideInInspector]
    public string owner;//The owner for this snowball
    //Load the throwable values from the item id
    public void LoadItemData(int throwableID) 
    {
        Throwable throwable = (Throwable)ItemsManager.ID2Item(throwableID);
        //Set all values from scriptable object
        throwableType = throwable.type;
        speedRandomness = throwable.speedRandomness;
        sizeRandomness = throwable.sizeRandomness;
        damageRandomness = throwable.damageRandomness;
        damageVelocityWeight = throwable.damageVelocityWeight;
        lifetime = throwable.lifetime;
        angularVelocityRange = throwable.angularVelocityRange;
        rigidbodyForceRange = throwable.rigidbodyForceRange;
    }
    //Randomizes the values
    public void RandomizeValues() 
    {
        //Randomize
        speed = Random.Range(speedRandomness.x, speedRandomness.y);
        size = Random.Range(sizeRandomness.x, sizeRandomness.y);
        angularVelocity = Random.insideUnitSphere * angularVelocityRange;//Random vector for angular velocity
        rigidbodyForce = Random.Range(rigidbodyForceRange.x, rigidbodyForceRange.y);
        //Round to int since damage is int
        damage = Mathf.RoundToInt(Random.Range(damageRandomness.x, damageRandomness.y));
    }
    //Set snowball values
    public void SetValues(float _speed, float _size, Vector3 _angularVelocity, float _rigidbodyForce, int _damage) 
    {
        //Set new variables using the struct
        speed = _speed;
        size = _size;
        angularVelocity = _angularVelocity;
        rigidbodyForce = _rigidbodyForce;
        damage = _damage;
    }
    //Init throwable
    //Called from other scripts to init some properities and change them in some way. Also called other stuff other from properities
    public void InitThrowable(string _owner)
    {
        owner = _owner;
        transform.localScale = new Vector3(size, size, size);//Set world scale with size variable
        Destroy(gameObject, lifetime);//Destroy snowball if lifetime is excedeed
    }
}
