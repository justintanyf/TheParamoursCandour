using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class MapLoader : MonoBehaviour
{
    public Cell doorLL;
    public Cell doorUL;
    public Cell doorLR;
    public Cell doorUR;
    public Cell wall;
    public Cell empty;
    public Cell floorNothing;
    public Cell floorTestItem;
    public Cell floorTestFae;
    public Dictionary<int, Cell> allCells;
    public GameObject bed, auri, chest, crafting, merchant, coin, feather;
    public GameObject item1, item2, item3;
    public GameObject enemy1, enemy2, enemy3, enemy4, boss;
    public GameObject logContents;
    public GameObject logTextTemplate;
    public Dictionary<int, GameObject> prefabs;
    public Tilemap groundMap;
    public Tilemap wallMap;
    public Tilemap objsMap;
    public Rigidbody2D player;
    public GameObject itemEncounterMenu;
    public GameObject faeEncounterMenu;
    public int currX = -1;
    public int currY = -1;
    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        allCells = new Dictionary<int, Cell>(){
            {-3, doorLL},
            {-2, wall},
            {-1, empty},
            {0, floorNothing},
            {1, floorNothing},
            {2, floorNothing},
            {3, floorNothing},
        };
        prefabs = new Dictionary<int, GameObject>(){
            // prefabs here - negative for items, positive for enemies
            {-100, bed},
            {-101, auri},
            {-102, chest},
            {-103, crafting},
            {-104, merchant},
            {-105, coin},
            {-106, feather},
            {-3, item3},
            {-2, item2},
            {-1, item1},
            {1, enemy1},
            {2, enemy2},
            {3, enemy3},
            {4, enemy4},
            {5, boss}
        };

        Room currRoom = Underthing.rooms[CharStats.GetCurrRoom()];
        Debug.Log("loading room " + CharStats.GetCurrRoom());
        Move playerScript = player.GetComponent<Move>();
        playerScript.currentMapLoader = this;
        playerScript.currRoom = currRoom;
        playerScript.itemEncounterMenu = itemEncounterMenu;
        playerScript.faeEncounterMenu = faeEncounterMenu;
        playerScript.objsMap = objsMap;

        if (CharStats.tempCounter == 4 || CharStats.tempCounter == 8 || CharStats.tempCounter == 12)
        {
            // boss room!!!
            for (int x = currRoom.cells.GetLength(0) - 1; x >= 0; x--)
            {
                for (int y = currRoom.cells.GetLength(1) - 1; y >= 0; y--)
                {
                    Vector3Int p = new Vector3Int(x,y,0);
                    int cellNum = currRoom.cells[x, y].mainCell;
                    currRoom.cells[x, y].hasObj = false;
                    currRoom.cells[x, y].prefab = 0;
                    TileBase tile1 = allCells[cellNum].groundTile;
                    TileBase testTile = Instantiate(tile1);
                    if (cellNum >= 0)
                    {
                        groundMap.SetTile(p, testTile);
                    }
                    else
                    {
                        wallMap.SetTile(p, testTile);
                    }
                }
            }

            // load doors
            for (int i = 0; i < 4; i++)
            {
                if (currRoom.doors[i,0] == -1 || currRoom.doors[i,1] == -1)
                {
                    continue;
                }

                int doorXLL = currRoom.doors[i,0];
                int doorYLL = currRoom.doors[i,1];
                Vector3Int pLL = new Vector3Int(doorXLL,doorYLL,0);
                Vector3Int pLR = new Vector3Int(0,0,0);
                Vector3Int pUL = new Vector3Int(0,0,0);
                Vector3Int pUR = new Vector3Int(0,0,0);
                TileBase tile1 = doorLL.groundTile;
                TileBase tileLL = Instantiate(tile1);
                TileBase tile2 = doorLR.groundTile;
                TileBase tileLR = Instantiate(tile2);
                TileBase tile3 = doorUL.groundTile;
                TileBase tileUL = Instantiate(tile3);
                TileBase tile4 = doorUR.groundTile;
                TileBase tileUR = Instantiate(tile4);

                switch (i)
                {
                    case 0: // west
                        pLR = new Vector3Int(doorXLL, doorYLL+1, 0);
                        pUL = new Vector3Int(doorXLL-1, doorYLL, 0);
                        pUR = new Vector3Int(doorXLL-1, doorYLL+1, 0);
                        break;
                    case 1: // north
                        pLR = new Vector3Int(doorXLL+1, doorYLL, 0);
                        pUL = new Vector3Int(doorXLL, doorYLL+1, 0);
                        pUR = new Vector3Int(doorXLL+1, doorYLL+1, 0);
                        break;
                    case 2: // east
                        pLR = new Vector3Int(doorXLL, doorYLL-1, 0);
                        pUL = new Vector3Int(doorXLL+1, doorYLL, 0);
                        pUR = new Vector3Int(doorXLL+1, doorYLL-1, 0);
                        break;
                    case 3: // south
                        pLR = new Vector3Int(doorXLL-1, doorYLL, 0);
                        pUL = new Vector3Int(doorXLL, doorYLL-1, 0);
                        pUR = new Vector3Int(doorXLL-1, doorYLL-1, 0);
                        break;
                    default:
                        Debug.Log("no");
                        break;
                }
                groundMap.SetTile(pLL, tileLL);
                groundMap.SetTile(pLR, tileLR);
                wallMap.SetTile(pUL, tileUL);
                wallMap.SetTile(pUR, tileUR);
                wallMap.SetTile(pLL, null);
                wallMap.SetTile(pLR, null);

                Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
                switch (i)
                {
                    case 0:
                        rot = Quaternion.Euler(0f, 0f, 90f);
                        break;
                    case 1:
                        rot = Quaternion.Euler(0f, 0f, 0f);
                        break;
                    case 2:
                        rot = Quaternion.Euler(0f, 0f, 270f);
                        break;
                    case 3:
                        rot = Quaternion.Euler(0f, 0f, 180f);
                        break;
                    default:
                        Debug.Log("no");
                        break;
                }
                groundMap.SetTransformMatrix(pLL, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
                groundMap.SetTransformMatrix(pLR, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
                wallMap.SetTransformMatrix(pUL, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
                wallMap.SetTransformMatrix(pUR, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
            }

            currRoom.itemsPos = new List<int[]>();
            currRoom.faesPos =new List<int[]>();

            // locate middle and set boss there
            int midX = currRoom.x % 2 == 0 ? currRoom.x / 2 : (currRoom.x - 1) / 2;
            int midY = currRoom.y % 2 == 0 ? currRoom.y / 2 : (currRoom.y - 1) / 2;
            currRoom.faesPos.Add(new int[] {midX-2, midX+2, midY-2, midY+2});
            currRoom.cells[midX, midY].hasObj = true;
            currRoom.cells[midX, midY].prefab = 5;
            GameObject tilePrefab = Instantiate(prefabs[5]);
            tilePrefab.layer = 2;
            Tile tile = groundMap.GetTile<Tile>(new Vector3Int(midX, midY, 0));
            tile.gameObject = tilePrefab;

            // load log
            foreach (string text in currRoom.textList)
            {
                GameObject currRow = Instantiate(logTextTemplate, logContents.transform);
                TMP_Text logText = currRow.GetComponentInChildren<TMP_Text>();
                logText.text = text;
                currRow.SetActive(true);
            }
            // instructions
            GameObject newRow = Instantiate(logTextTemplate, logContents.transform);
            TMP_Text newText = newRow.GetComponentInChildren<TMP_Text>();
            DateTime dt = DateTime.Now;
            string dtStr = dt.ToString("HH:mm");
            string finalText = "[" + dtStr + "] " + "You've found a boss room! Walk towards the centre or who knows when you'll encounter it ever again!";
            newText.text = finalText;
            newRow.SetActive(true);

            // set player pos - whenever scene is left, need to set coords!
            if (CharStats.haveLastCoords)
            {
                player.transform.position = new Vector3(CharStats.xCoord, CharStats.yCoord, 0);
            }
            else
            {
                Debug.Log("ono"); // tbh idk why it keeps coming here but it works so
                player.transform.position = new Vector3(CharStats.xCoord, CharStats.yCoord, 0); // better way for dis?
            }

            groundMap.RefreshAllTiles();

            return;
        }

        // load the room
        for (int x = currRoom.cells.GetLength(0) - 1; x >= 0; x--)
        {
            for (int y = currRoom.cells.GetLength(1) - 1; y >= 0; y--)
            {
                Vector3Int p = new Vector3Int(x,y,0);
                int cellNum = currRoom.cells[x, y].mainCell;
                TileBase tile1 = allCells[cellNum].groundTile;
                TileBase testTile = Instantiate(tile1);
                if (cellNum >= 0)
                {
                    groundMap.SetTile(p, testTile);
                    if (!currRoom.cells[x, y].hasObj && random.Next(0, 100) == 5) // 1%
                    {
                        currX = x;
                        currY = y;
                    }
                }
                else
                {
                    wallMap.SetTile(p, testTile);
                }

                if (currRoom.cells[x, y].hasObj)
                {
                    int prefabNum = currRoom.cells[x, y].prefab;
                    GameObject tilePrefab = Instantiate(prefabs[prefabNum]);
                    //tilePrefab.transform.position = p;
                    tilePrefab.layer = 2;
                    Tile tile = groundMap.GetTile<Tile>(p);
                    tile.gameObject = tilePrefab;

                    // set stats for the obj within tile?
                }
            }
        }

        // load doors
        for (int i = 0; i < 4; i++)
        {
            if (currRoom.doors[i,0] == -1 || currRoom.doors[i,1] == -1)
            {
                continue;
            }

            int doorXLL = currRoom.doors[i,0];
            int doorYLL = currRoom.doors[i,1];
            Vector3Int pLL = new Vector3Int(doorXLL,doorYLL,0);
            Vector3Int pLR = new Vector3Int(0,0,0);
            Vector3Int pUL = new Vector3Int(0,0,0);
            Vector3Int pUR = new Vector3Int(0,0,0);
            TileBase tile1 = doorLL.groundTile;
            TileBase tileLL = Instantiate(tile1);
            TileBase tile2 = doorLR.groundTile;
            TileBase tileLR = Instantiate(tile2);
            TileBase tile3 = doorUL.groundTile;
            TileBase tileUL = Instantiate(tile3);
            TileBase tile4 = doorUR.groundTile;
            TileBase tileUR = Instantiate(tile4);

            switch (i)
            {
                case 0: // west
                    pLR = new Vector3Int(doorXLL, doorYLL+1, 0);
                    pUL = new Vector3Int(doorXLL-1, doorYLL, 0);
                    pUR = new Vector3Int(doorXLL-1, doorYLL+1, 0);
                    break;
                case 1: // north
                    pLR = new Vector3Int(doorXLL+1, doorYLL, 0);
                    pUL = new Vector3Int(doorXLL, doorYLL+1, 0);
                    pUR = new Vector3Int(doorXLL+1, doorYLL+1, 0);
                    break;
                case 2: // east
                    pLR = new Vector3Int(doorXLL, doorYLL-1, 0);
                    pUL = new Vector3Int(doorXLL+1, doorYLL, 0);
                    pUR = new Vector3Int(doorXLL+1, doorYLL-1, 0);
                    break;
                case 3: // south
                    pLR = new Vector3Int(doorXLL-1, doorYLL, 0);
                    pUL = new Vector3Int(doorXLL, doorYLL-1, 0);
                    pUR = new Vector3Int(doorXLL-1, doorYLL-1, 0);
                    break;
                default:
                    Debug.Log("no");
                    break;
            }
            groundMap.SetTile(pLL, tileLL);
            groundMap.SetTile(pLR, tileLR);
            wallMap.SetTile(pUL, tileUL);
            wallMap.SetTile(pUR, tileUR);
            wallMap.SetTile(pLL, null);
            wallMap.SetTile(pLR, null);

            Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
            switch (i)
            {
                case 0:
                    rot = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case 1:
                    rot = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case 2:
                    rot = Quaternion.Euler(0f, 0f, 270f);
                    break;
                case 3:
                    rot = Quaternion.Euler(0f, 0f, 180f);
                    break;
                default:
                    Debug.Log("no");
                    break;
            }
            groundMap.SetTransformMatrix(pLL, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
            groundMap.SetTransformMatrix(pLR, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
            wallMap.SetTransformMatrix(pUL, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
            wallMap.SetTransformMatrix(pUR, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
        }

        // load merchant and coin and feather
        if (CharStats.tempCounter == 2 && currRoom.merchantLocation[0] == -1)
        {
            // merchant spawns (clear spot beside charstats xcoord ycoord +- 1)
            currRoom.hasMerchant = true;
            int charX = (int) CharStats.xCoord;
            int charY = (int) CharStats.yCoord;
            int merchantX = charX + 1;
            if (merchantX > currRoom.x)
            {
                merchantX -= 2;
            }
            int cellNum = currRoom.cells[merchantX, charY].mainCell;
            if (cellNum < 0)
            {
                wallMap.SetTile(new Vector3Int(merchantX, charY, 0), null);
                TileBase tile1 = floorNothing.groundTile;
                TileBase testTile1 = Instantiate(tile1);
                groundMap.SetTile(new Vector3Int(merchantX, charY, 0), testTile1);
            }
            else
            {
                if (currRoom.cells[merchantX, charY].hasObj)
                {
                    bool objFound = false;
                    for (int i = 0; i < currRoom.itemsPos.Count; i++)
                    {
                        if (currRoom.itemsPos[i][0] == merchantX && currRoom.itemsPos[i][1] == charY)
                        {
                            objFound = true;
                            currRoom.itemsPos.RemoveAt(i);
                            break;
                        }
                    }
                    if (!objFound)
                    {
                        for (int i = 0; i < currRoom.faesPos.Count; i++)
                        {
                            if (currRoom.faesPos[i][0] + 2 == merchantX && currRoom.faesPos[i][2] + 2 == charY)
                            {
                                objFound = true;
                                currRoom.faesPos.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            currRoom.coinLocation[0] = merchantX;
            currRoom.coinLocation[1] = charY;
            GameObject tilePrefabCoin = Instantiate(prefabs[-105]);
            tilePrefabCoin.layer = 3;
            Tile coinTile = groundMap.GetTile<Tile>(new Vector3Int(merchantX, charY, 0));
            coinTile.gameObject = tilePrefabCoin;

            int merchantY = charY + 1;
            if (merchantY > currRoom.y)
            {
                merchantY -= 2;
            }
            cellNum = currRoom.cells[merchantX, merchantY].mainCell;
            Vector3Int p = new Vector3Int(merchantX, merchantY, 0);
            if (cellNum < 0)
            {
                wallMap.SetTile(p, null);
                TileBase tile2 = floorNothing.groundTile;
                TileBase testTile2 = Instantiate(tile2);
                groundMap.SetTile(p, testTile2);
            }
            else
            {
                if (currRoom.cells[merchantX, merchantY].hasObj)
                {
                    bool objFound = false;
                    for (int i = 0; i < currRoom.itemsPos.Count; i++)
                    {
                        if (currRoom.itemsPos[i][0] == merchantX && currRoom.itemsPos[i][1] == merchantY)
                        {
                            objFound = true;
                            currRoom.itemsPos.RemoveAt(i);
                            break;
                        }
                    }
                    if (!objFound)
                    {
                        for (int i = 0; i < currRoom.faesPos.Count; i++)
                        {
                            if (currRoom.faesPos[i][0] + 2 == merchantX && currRoom.faesPos[i][2] + 2 == merchantY)
                            {
                                objFound = true;
                                currRoom.faesPos.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

            currRoom.merchantLocation[0] = merchantX;
            currRoom.merchantLocation[1] = merchantY;
            GameObject tilePrefab = Instantiate(prefabs[-104]);
            tilePrefab.layer = 3;
            Tile tile = groundMap.GetTile<Tile>(p);
            tile.gameObject = tilePrefab;
        }
        else if (CharStats.tempCounter == 2 && currRoom.merchantLocation[0] != -1)
        {
            int cellNum = cellNum = currRoom.cells[currRoom.merchantLocation[0], currRoom.merchantLocation[1]].mainCell;
            Vector3Int p = new Vector3Int(currRoom.merchantLocation[0], currRoom.merchantLocation[1], 0);
            if (cellNum < 0)
            {
                wallMap.SetTile(p, null);
                TileBase tile2 = floorNothing.groundTile;
                TileBase testTile2 = Instantiate(tile2);
                groundMap.SetTile(p, testTile2);
            }
            else
            {
                if (currRoom.cells[currRoom.merchantLocation[0], currRoom.merchantLocation[1]].hasObj)
                {
                    bool objFound = false;
                    for (int i = 0; i < currRoom.itemsPos.Count; i++)
                    {
                        if (currRoom.itemsPos[i][0] == currRoom.merchantLocation[0] && currRoom.itemsPos[i][1] == currRoom.merchantLocation[1])
                        {
                            objFound = true;
                            currRoom.itemsPos.RemoveAt(i);
                            break;
                        }
                    }
                    if (!objFound)
                    {
                        for (int i = 0; i < currRoom.faesPos.Count; i++)
                        {
                            if (currRoom.faesPos[i][0] + 2 == currRoom.merchantLocation[0] && currRoom.faesPos[i][2] + 2 == currRoom.merchantLocation[1])
                            {
                                objFound = true;
                                currRoom.faesPos.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            GameObject tilePrefab = Instantiate(prefabs[-104]);
            tilePrefab.layer = 3;
            Tile tile = groundMap.GetTile<Tile>(p);
            tile.gameObject = tilePrefab;
        }
        else if (currRoom.resetMerchant)
        {
            int check = random.Next(0, 10);
            if (check == 5 && currX != -1 && currY != -1 && !currRoom.isPlayerRoom) // 10%?
            {
                // random spot (currX / currY maybe)
                currRoom.hasMerchant = true;
                currRoom.merchantLocation[0] = currX;
                currRoom.merchantLocation[1] = currY;

                if (currRoom.cells[currX, currX].hasObj)
                {
                    bool objFound = false;
                    for (int i = 0; i < currRoom.itemsPos.Count; i++)
                    {
                        if (currRoom.itemsPos[i][0] == currX && currRoom.itemsPos[i][1] == currY)
                        {
                            objFound = true;
                            currRoom.itemsPos.RemoveAt(i);
                            break;
                        }
                    }
                    if (!objFound)
                    {
                        for (int i = 0; i < currRoom.faesPos.Count; i++)
                        {
                            if (currRoom.faesPos[i][0] + 2 == currX && currRoom.faesPos[i][2] + 2 == currY)
                            {
                                objFound = true;
                                currRoom.faesPos.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                GameObject tilePrefab = Instantiate(prefabs[-104]);
                tilePrefab.layer = 3;
                Tile tile = groundMap.GetTile<Tile>(new Vector3Int(currX, currY, 0));
                tile.gameObject = tilePrefab;
            }
            else
            {
                currRoom.hasMerchant = false;
                currRoom.merchantLocation[0] = -1;
                currRoom.merchantLocation[1] = -1;
            }
        }
        if (!CharStats.fragmentsFound[8] && currRoom.resetMerchant && random.Next(0, 10) == 5 && !currRoom.isPlayerRoom) // 10% chance
        {
            currRoom.hasFeather = true;
            GameObject tilePrefab = Instantiate(prefabs[-106]);
            tilePrefab.layer = 3;
            Tile tile = groundMap.GetTile<Tile>(new Vector3Int(currRoom.featherLocation[0], currRoom.featherLocation[1], 0));
            tile.gameObject = tilePrefab;
        }
        else
        {
            currRoom.hasFeather = false;
        }
        currRoom.resetMerchant = false;

        // load log
        foreach (string text in currRoom.textList)
        {
            GameObject currRow = Instantiate(logTextTemplate, logContents.transform);
            TMP_Text logText = currRow.GetComponentInChildren<TMP_Text>();
            logText.text = text;
            currRow.SetActive(true);
        }

        // set player pos - whenever scene is left, need to set coords!
        if (CharStats.haveLastCoords)
        {
            player.transform.position = new Vector3(CharStats.xCoord, CharStats.yCoord, 0);
        }
        else
        {
            Debug.Log("ono"); // tbh idk why it keeps coming here but it works so
            player.transform.position = new Vector3(CharStats.xCoord, CharStats.yCoord, 0); // better way for dis?
        }

        groundMap.RefreshAllTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}