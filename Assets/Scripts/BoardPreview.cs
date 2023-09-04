using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardPreview : MonoBehaviour
{
    
    [Header("Object Assignments")]
    [SerializeField] public GameObject squareObject;
    [SerializeField] public TextMeshPro squareText;
    // OTHER VARIABLES
    [HideInInspector] public GameController gameController;
    [HideInInspector] public int wordIndex;
    [HideInInspector] public int boardIndex;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color unavailableColor;
    private char character;

    public void SetLetter(char c, int wordIdx, int boardIdx) {
        squareText.text = c.ToString();
        this.character = c;
        this.wordIndex = wordIdx;
        this.boardIndex = boardIdx;
    }

    public void ChangePos(float x, float y) {
        transform.position += new Vector3(x, y, 0);
    }

    private void OnMouseDown() {
        gameController.RemoveLetter(wordIndex);
        Destroy(this.gameObject);
    }

    /*
        This function changes the color of the letter
        depending on whether or not the word that has
        been formed is valid or not.
    */
    public void UpdateLetterAvailability(bool isValid) {
        SpriteRenderer sr = squareObject.GetComponent<SpriteRenderer>();
        if (isValid) {
            sr.color = availableColor;
        } else {
            sr.color = unavailableColor;
        }
    }

}
