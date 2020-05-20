using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script that will tell the WorldManager that this game object has spawned with a tag
public class ObjectSpawnDetectionScript : MonoBehaviour
{
    public string stringTag;//The tag that will be passed to the WorldManager
    // Start is called before the first frame update
    void Start()
    {
        WorldManagerScript wm = FindObjectOfType<WorldManagerScript>();
        wm.OnObjectSpawn(gameObject, stringTag);
    }
}
