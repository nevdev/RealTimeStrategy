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

    private void OnMouseEnter() // a Unity method which is called when this event happens, like on trigger enter :)
    {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float) currentHealth / maxHealth;// typecast result int to flat 
    }
}
