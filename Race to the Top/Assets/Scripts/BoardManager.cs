using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoardManager : MonoBehaviourPunCallbacks
{
    public static BoardManager Instance; // Singleton instance

    public Transform tilesParent;
    public List<Transform> tiles = new List<Transform>();
    public GameObject playerMarker;
    private int currentTileIndex = 0;

    public float moveDuration = 0.5f;
    public float pauseBetweenMoves = 0.1f;

    private void Awake()
    {
        // Singleton Pattern - Ensures only one instance of BoardManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize Tiles
        tiles.Clear();
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            tiles.Add(tilesParent.GetChild(i));
        }
    }

    public void MovePlayer(int steps)
    {
        int targetIndex = currentTileIndex + steps;
        targetIndex = Mathf.Min(targetIndex, tiles.Count - 1);
        StartCoroutine(MovePlayerCoroutine(targetIndex));
    }

    IEnumerator MovePlayerCoroutine(int targetIndex)
    {
        for (int i = currentTileIndex + 1; i <= targetIndex; i++)
        {
            Vector3 startPosition = playerMarker.transform.position;
            Vector3 endPosition = tiles[i].position;
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                playerMarker.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            playerMarker.transform.position = endPosition;
            yield return new WaitForSeconds(pauseBetweenMoves);
        }

        currentTileIndex = targetIndex;
    }
}