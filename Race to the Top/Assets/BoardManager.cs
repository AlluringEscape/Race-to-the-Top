using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Tile Setup")]
    // Reference to the parent object containing all tiles.
    public Transform tilesParent;

    // This list will be automatically populated with the parent's children.
    public List<Transform> tiles = new List<Transform>();

    [Header("Player Setup")]
    // Reference to the player marker that moves along the board.
    public GameObject playerMarker;

    // Track the player's current tile index.
    private int currentTileIndex = 0;

    // Automatically populate the tiles list from the tilesParent.
    void Awake()
    {
        if (tilesParent != null)
        {
            tiles.Clear();
            for (int i = 0; i < tilesParent.childCount; i++)
            {
                tiles.Add(tilesParent.GetChild(i));
            }
        }
        else
        {
            Debug.LogError("tilesParent is not assigned in BoardManager!");
        }
    }

    public void MovePlayer(int steps)
    {
        int targetIndex = currentTileIndex + steps;
        // Clamp the target index so it doesn't exceed the last tile.
        targetIndex = Mathf.Min(targetIndex, tiles.Count - 1);

        Vector3 targetPosition = tiles[targetIndex].position;
        Debug.Log("Moving player to tile index " + targetIndex + " at position: " + targetPosition);
        playerMarker.transform.position = targetPosition;
        currentTileIndex = targetIndex;
        Debug.Log("Player moved to tile " + currentTileIndex);
    }
}
