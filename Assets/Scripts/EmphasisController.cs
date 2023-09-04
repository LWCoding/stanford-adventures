using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmphasisController : MonoBehaviour
{
    
    [Header("Object Assignments")]
    public TextMeshPro textObject;

    public void SetText(string s) {
        textObject.text = s;
    }

}
