using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {
    public Text diceResultsText;  // UI Text to display results
    public int diceSides = 6; // Number of sides on the dice

    public void RollDice()
    {
        int roll = Random.Range(1, diceSides + 1);
        diceResultsText.text = "Rolled: " + roll;
        Debug.Log("Dice roll: " + roll);
    }
}
