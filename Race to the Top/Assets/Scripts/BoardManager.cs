using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Tile Setup")]
    public Transform tilesParent;
    public List<Transform> tiles = new List<Transform>();

    [Header("Player Setup")]
    public GameObject playerMarker;
    private int currentTileIndex = 0;

    [Header("Movement Settings")]
    public float moveDuration = 0.5f; // Time to move between two tiles
    public float pauseBetweenMoves = 0.1f; // Optional pause between each tile move

    void Awake()
    {
        if (tilesParent != null)
        {
            tiles.Clear();
            // Populate tiles list using the order of children in the parent
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

    // Call this function to move the player a given number of steps
    public void MovePlayer(int steps)
    {
        int targetIndex = currentTileIndex + steps;
        targetIndex = Mathf.Min(targetIndex, tiles.Count - 1);
        StartCoroutine(MovePlayerCoroutine(targetIndex));
    }

    // Coroutine to smoothly move the player token from the current tile to the target tile, one tile at a time.
    IEnumerator MovePlayerCoroutine(int targetIndex)
    {
        // Move tile by tile from currentTileIndex+1 to targetIndex
        for (int i = currentTileIndex + 1; i <= targetIndex; i++)
        {
            Vector3 startPosition = playerMarker.transform.position;
            Vector3 endPosition = tiles[i].position;
            float elapsedTime = 0f;

            // Lerp between the current tile and the next tile over moveDuration seconds
            while (elapsedTime < moveDuration)
            {
                playerMarker.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Ensure the position is exactly the end position
            playerMarker.transform.position = endPosition;

            // Optional: pause briefly between tile moves
            yield return new WaitForSeconds(pauseBetweenMoves);
        }

        // Update the current tile index after moving
        currentTileIndex = targetIndex;
        Debug.Log("Player moved to tile " + currentTileIndex);
    }
}
