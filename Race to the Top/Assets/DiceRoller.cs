using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    public Text diceResultText;  // UI Text to show the dice result.
    public int diceMin = 1;
    public int diceMax = 6;

    // Reference to the BoardManager.
    public BoardManager boardManager;

    public void RollDice()
    {
        int roll = Random.Range(diceMin, diceMax + 1);

        if(diceResultText != null)
            diceResultText.text = "Rolled: " + roll;
        Debug.Log("Player rolled: " + roll);

        // Use BoardManager to move the player.
        if(boardManager != null)
            boardManager.MovePlayer(roll);
        else
            Debug.LogError("BoardManager reference not set in DiceRoller!");
    }
}
