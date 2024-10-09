using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteScrollingTileset : MonoBehaviour
{
    public Tilemap tilemap;            // The tilemap to work with
    public Vector2 scrollSpeed;        // The speed of scrolling, e.g., (0, -1) for downward scrolling

    private Vector3 startPos;          // Starting position of the tilemap
    private Camera mainCamera;         // Reference to the main camera

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        mainCamera = Camera.main;      // Get the main camera

        // Store the initial position of the tilemap
        startPos = tilemap.transform.position;
    }

    void Update()
    {
        // Move the tilemap by the scroll speed
        tilemap.transform.position += new Vector3(scrollSpeed.x, scrollSpeed.y, 0) * Time.deltaTime;

        // Check if the bottom row is off the screen
        if (IsBottomRowOffScreen())
        {
            RecycleTilesVertical();
        }
    }

    bool IsBottomRowOffScreen()
    {
        // Get the world position of the bottom row of the tilemap
        BoundsInt tilemapBounds = tilemap.cellBounds;
        Vector3Int bottomRowCell = new Vector3Int(tilemapBounds.xMin, tilemapBounds.yMin+4, 0);
        Vector3 bottomRowWorldPos = tilemap.CellToWorld(bottomRowCell);

        // Get the camera's bottom boundary (in world coordinates)
        float cameraBottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.transform.position.z)).y;

        // Return true if the bottom row is below the camera's bottom boundary
        return bottomRowWorldPos.y < cameraBottom;
    }


    void RecycleTilesVertical()
    {
        int bottomRowY = tilemap.cellBounds.yMin;
        int topRowY = tilemap.cellBounds.yMax;

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            Vector3Int oldTilePos = new Vector3Int(x, bottomRowY, 0);
            TileBase tile = tilemap.GetTile(oldTilePos);

            Vector3Int newTilePosition = new Vector3Int(x, topRowY, 0);
            tilemap.SetTile(newTilePosition, tile);

            tilemap.SetTile(oldTilePos, null);
        }

        tilemap.CompressBounds();
    }
}
