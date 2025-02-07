using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    public Text diceResultText;  // UI text to show the dice result
    public int diceMin = 1;
    public int diceMax = 6;

    public void RollDice()
    {
        int roll = Random.Range(diceMin, diceMax + 1);
        diceResultText.text = "Rolled: " + roll;
        Debug.Log("Player rolled: " + roll);
        MovePlayer(roll);
    }

    void MovePlayer(int spaces)
    {
        // TODO: Move the player forward by "spaces" tiles.
        Debug.Log("Move player forward by " + spaces + " spaces.");
    }
}
