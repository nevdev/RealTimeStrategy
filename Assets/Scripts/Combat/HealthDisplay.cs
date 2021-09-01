using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    // Whenever the Health changes this must be change to update the health bar
    // for this we are using events by subscribig from here to he Health Object

    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent; // the toggle object over the actual bar
    [SerializeField] private Image healthBarImage = null; // name says it all

    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    //Reminder:  OnMouseEnter uses the old Input system
    private void OnMouseEnter() // a Unity method which is called when this event happens, like on trigger enter :)
    {
        healthBarParent.SetActive(true);
    }
    //Reminder:  OnMouseEnter uses the old Input system
    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    // we're using two ints, one for the currentHealth and one for th max health - 
    // ..so whenever the current change we divide the max with the current health to get a decimal value from 0.o, 0.1, 0.2 to 1.0
    // we need from 0.0 to 1.0 because we're filling an image sprite accessing it from ther inspector and the fill colour set at horizontal from left to right
    // changing this will be shown as a health bar
    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount =  currentHealth / maxHealth;// typecast result int to float to return value with decimal        
    }
}
