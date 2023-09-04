using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    
    [Header("Object Assignments")]
    [SerializeField] private GameObject barFillObject;
    [SerializeField] private Color highHealth;
    [SerializeField] private Color medHealth;
    [SerializeField] private Color lowHealth;
    // OTHER VARIABLES
    private Vector3 fillOriginalScale;

    private void Start() {
        fillOriginalScale = barFillObject.transform.localScale;
    }

    /*
        This function takes a percentage and sets
        the filled portion of the HP bar to that
        value.
    */
    public void SetHPBar(float percent) {
        // Set the localScale of the HP bar depending on percentage.
        float calculatedX = fillOriginalScale.x * percent;
        barFillObject.transform.localScale = new Vector3(calculatedX,
                                                        fillOriginalScale.y,
                                                        fillOriginalScale.z);
        // Set the color of the HP bar depending on percentage.
        barFillObject.GetComponent<SpriteRenderer>().color = GetColor(percent);
    }

    /*
        This function takes a percentage of health
        and returns the valid color that represents
        the color of the health bar at that value.
    */
    public Color GetColor(float percent) {
        if (percent > 0.7f) {
            return highHealth;
        } else if (percent > 0.3f) {
            return medHealth;
        } else {
            return lowHealth;
        }
    }

}
