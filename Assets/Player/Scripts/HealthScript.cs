using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//Controls health of player
public class HealthScript : MonoBehaviour
{
    public int MaxHealth;//Maximum health of player
    public int Health;//Current health
    public Text HealthText;//Text showing current health
    public Slider HealthBar;//The health bar
    // Start is called before the first frame update
    void Start()
    {
        //Setup health
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Damage the player. Remove health out of player
    public void Damage(int damage) 
    {
        Health -= damage;//Apply damage to health
        HealthText.text = "Health : " + Health;//Update health text
        HealthBar.value = (float)Health / (float)MaxHealth;//Update health bar
        if (Health <= 0) 
        {            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);//Reloads current scene
        }
    }
}
