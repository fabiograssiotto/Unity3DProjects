using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject cross, nought, bar, crossBar;
    public enum Seed { EMPTY, CROSS, NOUGHT };
    public AudioClip gameOverClip;
    public Seed turn;


    // Game board data
    private Seed[] Cells;
    // To hold GameObjects instantiated in the cells.
    private GameObject[] references = new GameObject[9];
    private int numberOfPlays = 0;
    private int lastPlayedPosition = 0;

    // Use Singleton pattern
    private static GameManager instance = null;

    // Game Objects
    private Canvas playerTurnCanvas, gameOverCanvas;
    private Text playerTurn;
    private Toggle playerToggle;
    private Text winningPlayerText;

    // To enable/disable the Game
    private bool gameEnabled = true;

    // For the winner Bar
    private enum barType {
        ROW_1,
        ROW_2,
        ROW_3,
        COL_1,
        COL_2,
        COL_3,
        DIAG_1,
        DIAG_2
    }

    private barType winnerBar = barType.ROW_1;

    void Awake() {
        if (instance == null) {
            // Set the instance to the singleton object
            instance = this;
        } else {
            if ( instance != this) {
                // another object, destroy to keep a single instance
                DestroyObject(gameObject);
                instance = this;
                // Now keep this object from being destroyed between scenes
                DontDestroyOnLoad(gameObject);  
            }
        }

        InitGame();
    }

    private void InitGame() {

        // Initalize object references and Cells.
        Cells = new Seed[9];
        for (int i = 0; i < 9; i++) {
            Cells[i] = Seed.EMPTY;
        }

        // Enable the player turn canvas
        playerTurnCanvas = GameObject.Find("PlayerTurnCanvas").GetComponent<Canvas>();
        playerTurnCanvas.enabled = true;

        // Set up player Toggle, only enables after the player plays.
        playerToggle = GameObject.Find("PlayerTurnToggle").GetComponent<Toggle>();
        playerToggle.enabled = false;    
        playerTurn = GameObject.Find("PlayerTurnLabel").GetComponent<Text>();        
        playerTurn.text = "Vez da Alice";

        // Game Over Canvas
        gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        winningPlayerText = GameObject.Find("WinningPlayerText").GetComponent<Text>();
        winningPlayerText.text = "";

        // enable Game
        enableGame();
    }

    public static GameManager Instance {
        get {
            return instance;
        }
    }

    public void Play(GameObject obj, int position) {

        // do not run until the game is enabled.
        if (gameEnabled == false) return; else DoPlay(obj, position);
    }

    private void DoPlay(GameObject obj, int position) {

        Cells[position] = turn;

        if (turn == Seed.CROSS) {
            references[position] = Instantiate(cross, obj.transform.position, Quaternion.identity) as GameObject;
            turn = Seed.NOUGHT;
        }
        else if (turn == Seed.NOUGHT) {
            references[position] = Instantiate(nought, obj.transform.position, Quaternion.identity) as GameObject;
            turn = Seed.CROSS;
        }
        Destroy(obj.gameObject);
        numberOfPlays++;
        lastPlayedPosition = position;

        // Disable game and wait for toggle
        disableGame();
        playerToggle.enabled = true;
    }

    public void playerToggleChange() {

        if (playerToggle.isOn == true) {

            // Check winner conditions
            checkWinner();            
        }
    }

    private void checkWinner() {

        bool gameOver = false;

        // Check Victory/Draw condition
        Seed winnerSide = winner(lastPlayedPosition);
        if ((winnerSide == Seed.EMPTY) && numberOfPlays == 9) {
            // Draw condition
            winningPlayerText.text = "EMPATOU !!!";
            gameOver = true;
        }
        else if (winnerSide == Seed.CROSS) {
            // Crosses win
            displayBar();
            winningPlayerText.text = "A ALICE GANHOU !!!";
            gameOver = true;
        }
        else if (winnerSide == Seed.NOUGHT) {
            // Nought win
            displayBar();
            winningPlayerText.text = "O TATAI GANHOU !!!";
            gameOver = true;
        }

        if (gameOver) {
            // Run coroutine for waiting and restart the game.
            DisplayGameOverCanvas();
        }
        else {
            // Run coroutine for waiting and re-enabling the toggle and thegame.
            StartCoroutine(WaitAndNextPlay());
        }
    }

    void DisplayGameOverCanvas() {
        // Remove playerTurnCanvas first
        playerTurnCanvas.enabled = false;
        gameOverCanvas.enabled = true;
        // play audio clip 
        AudioSource.PlayClipAtPoint(gameOverClip, gameObject.transform.position, 1.0f);  
    }

    public void RestartGame() {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame() {
        Application.Quit();
    }

    IEnumerator WaitAndNextPlay() {
        yield return new WaitForSeconds(1.0f);

        // Toggle transition to checked, change player and enable game
        if (turn == Seed.CROSS) {
            playerTurn.text = "Vez da Alice";
        }
        else {
            playerTurn.text = "Vez do Tatai";
        }

        // Turn toggle off
        playerToggle.isOn = false;
        playerToggle.enabled = false;
        enableGame();
    }

    private void displayBar() {
        switch(winnerBar) {
            case barType.ROW_1:
                Instantiate(bar, references[1].transform.position, Quaternion.Euler(0, 0, -90));
                break;
            case barType.ROW_2:
                Instantiate(bar, references[4].transform.position, Quaternion.Euler(0, 0, -90));
                break;
            case barType.ROW_3:
                Instantiate(bar, references[7].transform.position, Quaternion.Euler(0, 0, -90));
                break;
            case barType.COL_1:
                Instantiate(bar, references[3].transform.position, Quaternion.Euler(0, 0, 0));
                break;
            case barType.COL_2:
                Instantiate(bar, references[4].transform.position, Quaternion.Euler(0, 0, 0));
                break;
            case barType.COL_3:
                Instantiate(bar, references[5].transform.position, Quaternion.Euler(0, 0, 0));
                break;
            case barType.DIAG_1:
                Instantiate(crossBar, references[4].transform.position, Quaternion.Euler(0, 0, 45));
                break;
            case barType.DIAG_2:
                Instantiate(crossBar, references[4].transform.position, Quaternion.Euler(0, 0, -45));
                break;
        }

    }

    private Seed winner(int position) {

        // Checks if we have a winner
        Seed winner = Seed.EMPTY;

        // Get current row and column.
        int row = getRow(position);
        int col = getCol(position);

        // Check if the row is complete
        if ((row == 0) && (Cells[0] == Cells[1] && Cells[1] == Cells[2])) {
            winner = Cells[0];
            winnerBar = barType.ROW_1;
        }
        else if ((row == 1) && (Cells[3] == Cells[4] && Cells[4] == Cells[5])) {
            winner = Cells[3];
            winnerBar = barType.ROW_2;
        }
        else if ((row == 2) && (Cells[6] == Cells[7] && Cells[7] == Cells[8])) {
            winner = Cells[6];
            winnerBar = barType.ROW_3;
        }
        
        // Now check columns
            if ((col == 0) && (Cells[0] == Cells[3] && Cells[3] == Cells[6])) {
                winner = Cells[0];
                winnerBar = barType.COL_1;
            }
            else if ((col == 1) && (Cells[1] == Cells[4] && Cells[4] == Cells[7])) {
                winner = Cells[1];
                winnerBar = barType.COL_2;
            }
            else if ((col == 2) && (Cells[2] == Cells[5] && Cells[5] == Cells[8])) {
                winner = Cells[2];
                winnerBar = barType.COL_3;
            }
        
            // Diagonals
            if ((position == 0 || position == 4 || position == 8) &&
                 (Cells[0] == Cells[4]) && (Cells[4] == Cells[8])) {
                winner = Cells[0];
                winnerBar = barType.DIAG_1;
            } else if ((position == 2 || position == 4 || position == 6) &&
                       (Cells[2] == Cells[4]) && (Cells[4] == Cells[6])) {
                winner = Cells[2];
                winnerBar = barType.DIAG_2;
            }
        
    return winner;
    }

    private void enableGame() {
        gameEnabled = true;
    }

    private void disableGame() {
        gameEnabled = false;
    }

    int getRow(int position) {

        int row = 0;

        switch (position) {

            case 0:
            case 1:
            case 2:
                row = 0;
                break;
            case 3:
            case 4:
            case 5:
                row = 1;
                break;
            case 6:
            case 7:
            case 8:
                row = 2;
                break;
        }
        return row;
    }

    int getCol(int position) {

        int col = 0;

        switch (position) {

            case 0:
            case 3:
            case 6:
                col = 0;
                break;
            case 1:
            case 4:
            case 7:
                col = 1;
                break;
            case 2:
            case 5:
            case 8:
                col = 2;
                break;
        }
        return col;
    }
}
