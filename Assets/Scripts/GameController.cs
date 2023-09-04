using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    [Header("Object Assignments")]
    public GameObject charSquarePrefab;
    public GameObject charSquarePreviewPrefab;
    public GameObject squareBoardObject;
    public GameObject selectedLettersObject;
    public GameObject enterContainerObject;
    public GameObject emphasisContainerObject;
    public Animator backgroundAnimator;
    public FighterController allyFighterController;
    public FighterController enemyFighterController;
    [Header("Board Info")]
    public float boardScale;
    public Vector3 boardOffset;
    [Header("Selected Letters Info")]
    public float lettersScale;
    public Vector3 lettersOffset;
    [Header("Other Assignments")]
    public TextAsset validWordsFile;
    // OTHER VARIABLES
    private const int BoardRows = 4;
    private const int BoardCols = 4;
    private float charSquareLength;
    private char[,] board = new char[BoardRows, BoardCols];
    private HashSet<string> validWords = new HashSet<string>();
    private List<GameObject> boardLetters = new List<GameObject>();
    private List<GameObject> selectedLetters = new List<GameObject>();
    private List<string> enteredWords = new List<string>(); // TODO: Use this!
    private string selectedWord = "";
    
    private void Start() {
        AssembleValidWords(); // Initialize `validWords` HashSet
        InitializeBoardLetters(); // Create random letters
        PrintBoard(); // Prints out the board to the console
        DrawBoard(); // Create all instantiations of board squares
        // Initialize the first enemy sprite in the Globals list.
        Enemy enemy = Globals.allEnemies[0];
        Globals.allEnemies.RemoveAt(0);
        enemyFighterController.InitializeStats(enemy);
    }

    /*
        The update function checks for key presses for
        the `Enter` button. This only works when the
        current word that is spelled is valid. This
        function relies on the `GameLoop` coroutine
        to properly time out function calls.
    */
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (IsValidWord(selectedWord)) {
                StartCoroutine(GameLoop());
            }
        }
    }

    /*
        This function runs the main game loop, animating
        the ally and enemy before toggling the turn back
        to the player move.
    */
    private IEnumerator GameLoop() {
        
        bool isLoopOver = false;

        // PLAYER LOOP

        string enteredWord = selectedWord;
        enteredWords.Add(enteredWord); // Store used word (for fun!)
        selectedWord = ""; // Reset selectedWord for next round
        Globals.isYourTurn = false; // Switch control
        RemoveLetter(0, true); // Remove all letters
        OverrideLetterAvailabilities(false); // Make all letters unavailable

        // Play the text emphasis animation if the word was good
        AnimateIfGood(enteredWord);

        // Calculate the damage dealt and store it in the allyFighterController
        allyFighterController.CalculateDamage(enteredWord);
        yield return allyFighterController.AnimateAttackCoroutine();
        if (Globals.gameIsOver || Globals.enemyDefeated) {
            isLoopOver = true;
        }

        // ENEMY LOOP

        // Only run if player if the player and enemy are not dead.
        if (!isLoopOver) {
            enemyFighterController.CalculateDamage(new string(' ', Random.Range(3, 5)));
            yield return enemyFighterController.AnimateAttackCoroutine();
            if (Globals.gameIsOver || Globals.enemyDefeated) {
                isLoopOver = true;
            }
        }

        // If only the enemy is dead and the player is not, load next enemy!
        if (Globals.enemyDefeated && !Globals.gameIsOver) {
            backgroundAnimator.enabled = true;
            backgroundAnimator.Play("MoveLeft");
            yield return new WaitForSeconds(2f);
            enemyFighterController.LoadNextEnemy();
            yield return new WaitForSeconds(0.001f);
            while (isPlaying(enemyFighterController.objectAnimator, "EnterScene")) {
                yield return null;
            }
            backgroundAnimator.enabled = false;
            Globals.enemyDefeated = false;
            isLoopOver = false;
        }

        // END LOOP

        if (isLoopOver) { yield break; }
        Globals.isYourTurn = true;
        OverrideLetterAvailabilities(true); // Make all letters available
    }

    /*
        This function goes through the file provided in
        `validWordsFile` and adds each word to the set
        `validWords`.
    */
    private void AssembleValidWords() {
        string[] words = validWordsFile.text.Split('\n');
        foreach (string word in words) {
            validWords.Add(word);
        }
    }

    /*
        This function randomizes the 4x4 board and
        sets the `board` variable to that board. Uses
        the boggle dice to determine which letters are
        in the board.
        Dice faces:
        AAEEGN, ABBJOO, ACHOPS, AFFKPS,
        AOOTTW, CIMOTU, DEILRX, DELRVY,
        DISTTY, EEGHNW, EEINSU, EHRTVW,
        EIOSST, ELRTTY, HIMNQU, HLNNRZ
    */
    private void InitializeBoardLetters() {
        List<string> diceList = new List<string> {
            "AAEEGN", "ABBJOO", "ACHOPS", "AFFKPS",
            "AOOTTW", "CIMOTU", "DEILRX", "DELRVY",
            "DISTTY", "EEGHNW", "EEINSU", "EHRTVW",
            "EIOSST", "ELRTTY", "HIMNQU", "HLNNRZ"
        };
        int currWord = 0;
        for (int i = 0; i < BoardRows; i++) {
            for (int j = 0; j < BoardCols; j++) {
                board[i, j] = diceList[currWord++][Random.Range(0, diceList[0].Length)];
            }
        }
    }

    /*
        Creates an instantiation of the `newSquare`
        object, depending on whether or not if it is
        designed to be a preview of the letter or in
        the board.
    */
    private GameObject CreateCharSquare(bool isPreview) {
        if (isPreview) {
            GameObject newSquare = Instantiate(charSquarePreviewPrefab);
            newSquare.GetComponent<BoardPreview>().gameController = GetComponent<GameController>();
            return newSquare;
        } else {
            GameObject newSquare = Instantiate(charSquarePrefab);
            newSquare.GetComponent<BoardSquare>().gameController = GetComponent<GameController>();
            return newSquare;
        }
    }

    /*
        This function draws the board by using the
        `charSquarePrefab` GameObject. The only reason
        why the function is big is because there's so 
        much crap that deals with offsets.
    */
    private void DrawBoard() {
        float letterGap = 0.125f;
        charSquareLength = charSquarePrefab.GetComponent<BoardSquare>().GetSideLength();
        float relativeX = charSquareLength * -1.5f - letterGap * 1.5f;
        float relativeY = charSquareLength * 1.5f + letterGap * 1.5f;
        squareBoardObject.SetActive(true);
        int lettersInstantiated = 0;
        for (int i = 0; i < BoardRows; i++) {
            for (int j = 0; j < BoardCols; j++) {
                GameObject newSquare = CreateCharSquare(false);
                newSquare.transform.SetParent(squareBoardObject.transform);
                char squareLetter = board[i, j];
                newSquare.GetComponent<BoardSquare>().SetLetter(squareLetter, lettersInstantiated++);
                newSquare.GetComponent<BoardSquare>().ChangePos(relativeX, relativeY);
                if (j == BoardRows - 1) {
                    relativeX -= charSquareLength * (BoardRows - 1) + letterGap * (BoardRows - 1);
                    relativeY -= charSquareLength + letterGap;
                } else {
                    relativeX += charSquareLength + letterGap;
                }
                boardLetters.Add(newSquare);
            }
        }
        squareBoardObject.transform.localScale *= boardScale;
        squareBoardObject.transform.position += boardOffset;
    }

    /*
        This function prints out the `board` in one
        Debug.Log() statement.
    */
    private void PrintBoard() {
        string s = "[ ";
        for (int i = 0; i < BoardRows; i++) {
            for (int j = 0; j < BoardCols; j++) {
                s += board[i, j];
            }
            if (i != BoardRows - 1) {
                s += ", ";
            }
        }
        Debug.Log(s + " ]");
    }

    /* 
        This function creates a new `charSquarePreviewPrefab`
        gameObject and adds it to the `selectedLetters` 
        List. This new gameObject is not interactable.
    */
    public void AddLetter(char c, int bIndex) {
        // Add character to our string.
        selectedWord += c;
        // Create our square and set its parent and character.
        GameObject newSquare = CreateCharSquare(true);
        newSquare.transform.SetParent(selectedLettersObject.transform);
        newSquare.GetComponent<BoardPreview>().SetLetter(c, selectedLetters.Count, bIndex);
        // Add it to the `selectedLetters` List.
        selectedLetters.Add(newSquare);
        DrawSelectedLetters();
    }

    /* 
        This function removes the letter at the index `idx`.
        It also removes all letters that are following that
        letter and reallows the user to select those letters
        in the letter grid. Removed letters are randomized.
    */
    public void RemoveLetter(int idx, bool randomizeChar = false) {
        // Remove the characters from our string.
        selectedWord = selectedWord.Substring(0, idx);
        // Remove all of the GameObjects.
        HashSet<int> indexesRemoved = new HashSet<int>();
        for (int i = selectedLetters.Count - 1; i >= 0; i--) {
            GameObject letter = selectedLetters[i];
            BoardPreview letterBP = letter.GetComponent<BoardPreview>();
            if (letterBP.wordIndex >= idx) {
                selectedLetters.RemoveAt(i);
                indexesRemoved.Add(letterBP.boardIndex);
                Destroy(letter);
            }
        }
        for (int i = 0; i < boardLetters.Count; i++) {
            GameObject letter = boardLetters[i];
            BoardSquare letterBS = letter.GetComponent<BoardSquare>();
            if (indexesRemoved.Contains(letter.GetComponent<BoardSquare>().boardIndex)) {
                if (randomizeChar) {
                    // Randomize the character
                    letterBS.RandomizeCharacter();
                }
                letterBS.inWord = false;
            }
        }
        DrawSelectedLetters();
    }

    /*
        This function loops through all letters in the board
        and updates the looks of each depending on whether
        or not they are currently in the selected letter list.
    */
    private void UpdateLetterAvailabilities() {
        for (int i = 0; i < boardLetters.Count; i++) {
            boardLetters[i].GetComponent<BoardSquare>().UpdateLetterAvailability();
        }
    }

    /*
        This function loops through all letters in the board
        and updates the looks of each based on what color should
        be set for them. 
        true = available | false = unavailable
    */
    private void OverrideLetterAvailabilities(bool isAvailable) {
        for (int i = 0; i < boardLetters.Count; i++) {
            boardLetters[i].GetComponent<BoardSquare>().OverrideLetterAvailability(isAvailable);
        }
    }

    /*
        This function repositions all of the letters in the 
        `selectedLetters` List, putting them in the correct
        positions. Also changes the color of any tiles in
        the board. It handles valid words as well at the end
        of the function.
    */
    private void DrawSelectedLetters() {
        UpdateLetterAvailabilities();
        int letterCount = selectedLetters.Count;
        float letterGap = 0.075f;
        for (int i = 0; i < letterCount; i++) {
            GameObject letter = selectedLetters[i];
            float xOffset = ((letterCount - 1) * charSquareLength) / -2f - letterGap / 2 * (letterCount - 1);
            xOffset += i * charSquareLength + i * letterGap;
            letter.transform.position = new Vector3(lettersOffset.x + xOffset,
                                                    lettersOffset.y,
                                                    0);
        }
        UpdateLettersIfValid();
    }

    /*
        This function updates each GameObject in the
        `selectedLetters` list if it is a valid word,
        changing the colors to the available color.
        Also shows the text to show that the current
        word is valid to submit.
    */
    private void UpdateLettersIfValid() {
        bool isValid = IsValidWord(selectedWord);
        for (int i = 0; i < selectedLetters.Count; i++) {
            selectedLetters[i].GetComponent<BoardPreview>().UpdateLetterAvailability(isValid);
        }
        enterContainerObject.SetActive(isValid);
        if (isValid) {
            enterContainerObject.GetComponent<Animator>().enabled = true;
            enterContainerObject.GetComponent<Animator>().Play("TextBlink");
        } else {
            enterContainerObject.GetComponent<Animator>().enabled = false;
        }
    }

    /*
        This function takes in a string and determines
        whether or not it is a valid word in the English
        dictionary.
    */  
    private bool IsValidWord(string s) {
        // If it's less than three characters, return false!
        if (s.Length < 3) {
            return false;
        }
        return validWords.Contains(s.ToLower());
    }
    
    /*
        This function animates and sets the text for the
        `TextEmphasis` GameObject depending on whether or
        not the string was good enough to trigger it.
    */
    private void AnimateIfGood(string s) {
        int wordLength = s.Length;
        if (wordLength < 5) { return; }
        if (wordLength == 5) {
            emphasisContainerObject.GetComponent<EmphasisController>().SetText("GREAT!");
        }
        if (wordLength == 6) {
            emphasisContainerObject.GetComponent<EmphasisController>().SetText("WOW!");
        }
        if (wordLength == 7) {
            emphasisContainerObject.GetComponent<EmphasisController>().SetText("INCREDIBLE!");
        }
        if (wordLength > 7) {
            emphasisContainerObject.GetComponent<EmphasisController>().SetText("WHOMPED!");
        }
        emphasisContainerObject.GetComponent<Animator>().Play("EmphasizeText");
    }

    /*
        This function takes in an Animator and a string
        representing the name of an animation, and returns
        a boolean if it is currently playing or not.
    */
    public bool isPlaying(Animator anim, string stateName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

}
