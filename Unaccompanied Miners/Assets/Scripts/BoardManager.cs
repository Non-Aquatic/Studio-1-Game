using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject tileContainer; 
    public GameObject traversableTilePrefab;
    public GameObject nonTraversableTilePrefab;
    public GameObject miningTilePrefab;

    public int[,] gridLayout = {
        { 1, 2, 1, 1, 1, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 1, 1, 2, 1, 1, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 2, 1, 1, 1 }
    };

    private void Start()
    {
        GenerateBoard();
    }
    private void GenerateBoard()
    {
        int rows = gridLayout.GetLength(0);
        int columns = gridLayout.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject tilePrefab;
                if (gridLayout[y, x] == 1)
                {
                    tilePrefab = traversableTilePrefab;
                }
                else if (gridLayout[y, x] == 2)
                {
                    tilePrefab = miningTilePrefab; 
                }
                else
                {
                    tilePrefab = nonTraversableTilePrefab;
                }
                GameObject tileInstance = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tileInstance.transform.SetParent(tileContainer.transform);
            }
        }
    }

    public bool IsTileTraversable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;
        return gridLayout[position.y, position.x] == 1 || gridLayout[position.y, position.x] == 2;
    }
    public bool IsMiningNode(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridLayout.GetLength(1) || position.y < 0 || position.y >= gridLayout.GetLength(0))
            return false;

        return gridLayout[position.y, position.x] == 2; 
    }
}
