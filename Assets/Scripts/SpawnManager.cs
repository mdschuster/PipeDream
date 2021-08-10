using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{

    private Queue<GameObject> upcomingTiles;
    private Dictionary<string, Tile> tileDict;
    private Dictionary<string, Tile> underTileDict;
    private Vector3Int startPos;
    private Vector3Int endPos;

    [Header("Placeable Tiles")]
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private Tile[] tileMapTiles;
    [SerializeField] private string[] tileNames;
    [SerializeField] private Tile[] underTileMapTiles;
    [SerializeField] private Tile underTileEnd;

    [Header("Locations")]
    [SerializeField] private GameObject[] locations;

    [Header("Tile Map")]
    [SerializeField] private Tilemap placeableMap;
    [SerializeField] private Tilemap underMap;


    private int numSpawnLocs;

    // Start is called before the first frame update
    void Start()
    {
        upcomingTiles = new Queue<GameObject>();
        tileDict = new Dictionary<string, Tile>();
        underTileDict = new Dictionary<string, Tile>();
        setupTileDict();
        //initial spawn of the first n tiles
        numSpawnLocs = locations.Length;

        for (int i = 0; i < numSpawnLocs; i++)
        {
            int randVal = Random.Range(0, tiles.Length);
            GameObject go = Instantiate(tiles[randVal], locations[i].transform.position, Quaternion.identity);
            upcomingTiles.Enqueue(go);
        }

        //get the starting position
        BoundsInt bounds = underMap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int z = bounds.min.z; z < bounds.max.z; z++)
                {
                    TileBase tile = underMap.GetTile(new Vector3Int(x, y, z));
                    if (tile != null && tile.name == "Start 1")
                    {
                        startPos = new Vector3Int(x, y, z);
                    }
                }
            }
        }

        //get the end position
        bounds = placeableMap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int z = bounds.min.z; z < bounds.max.z; z++)
                {
                    TileBase tile = placeableMap.GetTile(new Vector3Int(x, y, z));
                    if (tile != null && tile.name == "End")
                    {
                        endPos = new Vector3Int(x, y, z);
                    }
                }
            }
        }

    }

    public void updateQueue()
    {
        Vector3Int mousePos = GameManager.instance().getMousePos();

        //don't do anything if we haven't clicked on the gameplay map
        if (placeableMap.GetTile(mousePos) == null)
        {
            return;
        }

        GameObject dequeued = upcomingTiles.Dequeue();
        string tiletype = dequeued.GetComponent<TileType>().getTileType();
        Tile tile;
        Tile underTile;
        tileDict.TryGetValue(tiletype, out tile);
        underTileDict.TryGetValue(tiletype, out underTile);
        if (tile != null)
        {
            placeTile(tile, mousePos);
            //placeUnderMapTile(underTile, mousePos);
        }
        else
        {
            Debug.LogError("Invalid tile type");
            return;
        }
        //destroy the preview prefab that was next
        Destroy(dequeued.gameObject);
        GameObject[] temp = upcomingTiles.ToArray();

        //shift positions of all other tiles down
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].transform.position = locations[i].transform.position;
        }
        int randVal = Random.Range(0, tiles.Length);
        GameObject go = Instantiate(tiles[randVal], locations[numSpawnLocs - 1].transform.position, Quaternion.identity);
        upcomingTiles.Enqueue(go);
    }



    public bool spawnFinalPath()
    {
        bool win = false;
        Vector3Int left = new Vector3Int(-1, 0, 0);
        Vector3Int right = new Vector3Int(1, 0, 0);
        Vector3Int up = new Vector3Int(0, 1, 0);
        Vector3Int down = new Vector3Int(0, -1, 0);
        Vector3Int moveDir = new Vector3Int(0, 0, 0);

        //start left
        moveDir = left;
        Vector3Int currentPos = startPos;
        bool finished = false;
        Tile tileToPlace = null;
        while (!finished)
        {
            TileBase testTile;
            currentPos += moveDir;
            testTile = placeableMap.GetTile(currentPos);
            if (testTile != null && testTile.name != "Empty")
            {
                if (testTile.name == "Start") //Tile is missnamed
                {
                    placeUnderMapTile(underTileEnd, currentPos);
                    finished = true;
                    win = true;
                    break;
                }
                switch (testTile.name)
                {
                    case ("Horizontal"):
                        if (moveDir == left) moveDir = left;
                        else if (moveDir == right) moveDir = right;
                        else finished = true;
                        tileToPlace = underTileMapTiles[2];
                        break;
                    case ("Vertical"):
                        if (moveDir == up) moveDir = up;
                        else if (moveDir == down) moveDir = down;
                        else finished = true;
                        tileToPlace = underTileMapTiles[5];
                        break;
                    case ("UpLeft"):
                        if (moveDir == right) moveDir = down;
                        else if (moveDir == up) moveDir = left;
                        else finished = true;
                        tileToPlace = underTileMapTiles[3];
                        break;
                    case ("UpRight"):
                        if (moveDir == left) moveDir = down;
                        else if (moveDir == up) moveDir = right;
                        else finished = true;
                        tileToPlace = underTileMapTiles[4];
                        break;
                    case ("DownLeft"):
                        if (moveDir == right) moveDir = up;
                        else if (moveDir == down) moveDir = left;
                        else finished = true;
                        tileToPlace = underTileMapTiles[0];
                        break;
                    case ("DownRight"):
                        if (moveDir == left) moveDir = up;
                        else if (moveDir == down) moveDir = right;
                        else finished = true;
                        tileToPlace = underTileMapTiles[1];
                        break;

                    default:
                        Debug.LogError("Unknown Tile Name");
                        break;
                }
                if (finished == false)
                {
                    placeUnderMapTile(tileToPlace, currentPos);
                }
            }
            else
            {
                finished = true;
            }
        }
        return win;
    }


    private void setupTileDict()
    {
        for (int i = 0; i < tileNames.Length; i++)
        {
            tileDict.Add(tileNames[i], tileMapTiles[i]);
            underTileDict.Add(tileNames[i], underTileMapTiles[i]);
        }
    }

    private void placeTile(Tile tile, Vector3Int pos)
    {
        placeableMap.SetTile(pos, tile);

    }

    public void placeUnderMapTile(Tile tile, Vector3Int pos)
    {
        underMap.SetTile(pos, tile);
    }

    public GameObject getNextTile()
    {
        return upcomingTiles.Peek();
    }

    public Tile getNextPlaceableTile()
    {
        Tile tile;
        tileDict.TryGetValue(upcomingTiles.Peek().GetComponent<TileType>().getTileType(), out tile);
        return tile;
    }

    public Vector3Int getStartPos()
    {
        return startPos;
    }

    public Vector3Int getEndPos()
    {
        return endPos;
    }

    public Tile[] getUnderMapTiles()
    {
        return underTileMapTiles;
    }
}
