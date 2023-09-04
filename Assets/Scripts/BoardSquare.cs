using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardSquare : MonoBehaviour
{
    
    [Header("Object Assignments")]
    [SerializeField] public GameObject squareObject;
    [SerializeField] public TextMeshPro squareText;
    [SerializeField] private Animator squareAnim;
    // OTHER VARIABLES
    [HideInInspector] public GameController gameController;
    [HideInInspector] public int boardIndex;
    [HideInInspector] public bool inWord = false;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color unavailableColor;
    private char character;

    public void SetLetter(char c, int idx) {
        squareText.text = c.ToString();
        this.character = c;
        this.boardIndex = idx;
    }

    public float GetSideLength() {
        SpriteRenderer sr = squareObject.GetComponent<SpriteRenderer>();
        return sr.bounds.size.x;
    }

    public void ChangePos(float x, float y) {
        transform.position += new Vector3(x, y, 0);
    }

    /*
        This function changes the color of the letter
        depending on whether or not it is currently
        in the user's selected letters or not, denoted
        by the `inWord` boolean variable.
    */
    public void UpdateLetterAvailability() {
        SpriteRenderer sr = squareObject.GetComponent<SpriteRenderer>();
        if (!inWord) {
            sr.color = availableColor;
        } else {
            sr.color = unavailableColor;
        }
    }

    public void OverrideLetterAvailability(bool isAvailable) {
        SpriteRenderer sr = squareObject.GetComponent<SpriteRenderer>();
        if (isAvailable) {
            sr.color = availableColor;
        } else {
            sr.color = unavailableColor;
        }
    }

    /*
        This function randomizes the current character on
        the board.
    */
    public void RandomizeCharacter() {
        // All the characters from Boggle but stringed together
        string chars = "AAEEGNABBJOOACHOPSAFFKPSAOOTTWCIMOTUDEILRXDELRVYDISTTYEEGHNWEEINSUEHRTVWEIOSSTELRTTYHIMNQUHLNNRZ";
        this.character = chars[Random.Range(0, chars.Length)];
        squareText.text = this.character.ToString();
    }

    private void OnMouseEnter() {
        squareAnim.Play("HoverEnter");
    }

    private void OnMouseExit() {
        squareAnim.Play("HoverExit");
    }

    private void OnMouseDown() {
        if (!inWord && Globals.isYourTurn) {
            inWord = true;
            gameController.AddLetter(character, boardIndex);
        }
    }

}
