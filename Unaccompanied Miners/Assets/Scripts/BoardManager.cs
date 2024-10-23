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

    public int[,] gemCounts;

    private void Start()
    {
        GenerateBoard();
        GenerateGemCounts();
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
                GameObject tiles = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tiles.transform.SetParent(tileContainer.transform);
                tiles.name = "Tile"+x+","+y;
            }
        }
    }
    private void GenerateGemCounts()
    {
        gemCounts = new int[gridLayout.GetLength(0), gridLayout.GetLength(1)];
        for (int y = 0; y < gemCounts.GetLength(0); y++)
        {
            for (int x = 0; x < gemCounts.GetLength(1); x++)
            {
                if (gridLayout[y, x] == 2) 
                {
                    gemCounts[y, x] = 5; 
                }
                else
                {
                    gemCounts[y, x] = 0; 
                }
            }
        }
    }
    public void ReplaceTile(Vector2Int position)
    {
        GameObject replacedTile = GameObject.Find("Tile"+position.x+","+position.y);

        Destroy(replacedTile);

        GameObject newTileInstance = Instantiate(traversableTilePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        newTileInstance.transform.SetParent(tileContainer.transform);
        newTileInstance.name = "Tile" + position.x + "," + position.y;
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
