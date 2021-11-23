using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public CellExtended[,] cells = new CellExtended[102,102]; // assume max dimensions 100x100
    public int[,] doors = new int[4,2]; // 4 directions, x-y coords
    public bool diffDoor;
    public int[] actualDoor;
    public int[] connections; // WNES
    public bool isPlayerRoom = false;
    public bool hasMerchant = false;
    public List<int[]> itemsPos = new List<int[]>();
    public List<int[]> faesPos = new List<int[]>();
    public int[,] playerRoomItems = new int[4,2]; // auri - kvothe bed - chest - crafting
    public int[] merchantLocation = new int[] {-1, -1};
    public int[] coinLocation = new int[2];
    public bool hasFeather = false;
    public int[] featherLocation = new int[2];
    System.Random random = new System.Random();

    public bool resetMerchant = true;
    public bool auriInteracted = false;
    public List<string> textList = new List<string>();

    // settings
    public int minXY = 25;
    public int maxXY = 100;
    public int x;
    public int y;
    public int minSteps = 2;
    public int maxSteps;
    public int numTurns;
    public int midTurns;
    public int pathsLeft;
    public Queue<int[]> nextPath = new Queue<int[]>(); // x, y, dir coming from
    public int numDoors = 0;
    public int[] intersection = new int[]{-1, -1};
    public bool hasIntersection = false;

    public Room(int[] connections, bool diffDoor, int[] actualDoor)
    {
        this.connections = connections;
        this.diffDoor = diffDoor;
        this.actualDoor = actualDoor;

        //dimensions
        x = random.Next(minXY, maxXY);
        y = random.Next(minXY, maxXY);
        maxSteps = (int) Math.Floor(Math.Sqrt(x * y) * 0.2);
        numTurns = (x+y)%2==0 ? (x+y)/2 : (x+y-1)/2;
        midTurns = numTurns%2==0 ? numTurns/2 : (numTurns-1)/2;
        pathsLeft = maxSteps; // might have to change

        for (int j = 0; j <= y + 1; j++)
        {
            cells[0,j] = GenerateWallCell(); // left wall
            cells[x+1,j] = GenerateWallCell(); // right wall
        }
        for (int j = y + 2; j < cells.GetLength(1); j++)
        {
            cells[0,j] = GenerateEmptyCell();
            cells[x+1,j] = GenerateEmptyCell();
        }

        for (int i = 1; i <= x; i++)
        {
            cells[i,0] = GenerateWallCell(); // bottom wall
            for (int j = 1; j <= y; j++)
            {
                cells[i,j] = GenerateWallCell(); // fill whole room first
            }
            cells[i,y+1] = GenerateWallCell(); // top wall
            for (int j = y + 2; j < cells.GetLength(1); j++)
            {
                cells[i,j] = GenerateEmptyCell();
            }
        }
        for (int i = x + 2; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i,j] = GenerateEmptyCell();
            }
        }

        // doors
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == -1) // no door in that direction
            {
                doors[i,0] = -1;
                doors[i,1] = -1;
            }
            else // door in that direction
            {
                numDoors++;
                int doorX = -1;
                int doorY = -1;
                switch (i)
                {
                    case 0: // west
                        doorX = 0;
                        doorY = random.Next(2, y - 1);
                        cells[1,doorY] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{1, doorY, i});
                        break;
                    case 1: // north
                        doorX = random.Next(2, x - 1);
                        doorY = y + 1;
                        cells[doorX,y] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{doorX, y, i});
                        break;
                    case 2: // east
                        doorX = x + 1;
                        doorY = random.Next(2, y - 1);
                        cells[x,doorY] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{x, doorY, i});
                        break;
                    case 3: // south
                        doorX = random.Next(2, x - 1);
                        doorY = 0;
                        cells[doorX,1] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{doorX, 1, i});
                        break;
                    default:
                        Debug.Log("uhhh");
                        break;
                }
            
                doors[i,0] = doorX;
                doors[i,1] = doorY;
            }
        }

        if (numDoors == 1)
        {
            hasIntersection = true;
        }

        // start generating paths
        while (pathsLeft > 0 || !hasIntersection)
        {
            GeneratePath(nextPath.Dequeue());
        }

        int[] temp = nextPath.Dequeue();
        featherLocation[0] = temp[0];
        featherLocation[1] = temp[1];
    }

    private void GeneratePath(int[] starting)
    {
        int currX = starting[0];
        int currY = starting[1];
        int pathNum = cells[currX,currY].mainCell;
        int currDirection = starting[2];

        // generate path; enqueue midway through; check for intersections along the way
        for (int i = 0; i < numTurns; i++)
        {
            int numSteps = random.Next(minSteps, maxSteps + 1);
            int midSteps = numSteps%2==0 ? numSteps/2 : (numSteps+1)/2;
            int nextDirection = GetNextDirection(currX, currY, currDirection, numSteps);

            // set all steps in that direction, checking for intersections
            for (int j = 0; j < numSteps; j++)
            {
                // set new coords
                switch (nextDirection)
                {
                    case 0:
                        currX--;
                        break;
                    case 1:
                        currY++;
                        break;
                    case 2:
                        currX++;
                        break;
                    case 3:
                        currY--;
                        break;
                    default:
                        Debug.Log("hewp");
                        break;
                }

                // enqueue for midpoint
                if (i+1 == midTurns && j+1 == midSteps)
                {
                    nextPath.Enqueue(new int[]{currX, currY, nextDirection});
                }

                // check for intersection
                int otherNum = cells[currX, currY].mainCell;
                if (otherNum <= 3 && otherNum >= 0 && otherNum != pathNum && !hasIntersection) // intersection
                {
                    // if numDoors < 3, we r done; else need to check what's inside the coords
                    if (numDoors < 3)
                    {
                        hasIntersection = true;
                    }
                    else if (intersection[0] == -1) // first intersection for three doors
                    {
                        intersection[0] = pathNum;
                        intersection[1] = otherNum;
                    }
                    else if ((intersection[0] == pathNum && intersection[1] == otherNum) || (intersection[0] == otherNum && intersection[1] == pathNum))
                    {
                        // same intersection for three doors
                    }
                    else // second intersection for three doors
                    {
                        hasIntersection = true;
                    }
                }

                // create the path
                if (otherNum > 3 || otherNum < 0)
                {
                    // no floor tile there yet
                    cells[currX,currY] = GenerateFloorCell(pathNum, GetPrefab(currX, currY));
                }
            }
            currDirection = nextDirection;
        }

        // decrement pathsLeft
        pathsLeft--;
    }

    private int GetNextDirection(int currX, int currY, int currDirection, int numSteps)
    {
        int nextDirection = currDirection;
        bool settled = false;
        while (!settled)
        {
            nextDirection = random.Next(0, 4);
            if (nextDirection == currDirection) continue;

            // check if there's space in that direction
            switch (nextDirection)
            {
                case 0: // west - check x > 0
                    if (currX - numSteps > 0)
                    {
                        settled = true;
                    }
                    break;
                case 1: // north - check y <= y
                    if (currY + numSteps <= y)
                    {
                        settled = true;
                    }
                    break;
                case 2: // east - check x <= x
                    if (currX + numSteps <= x)
                    {
                        settled = true;
                    }
                    break;
                case 3: // south - check y > 0
                    if (currY - numSteps > 0)
                    {
                        settled = true;
                    }
                    break;
                default:
                    Debug.Log("pwease,,,,");
                    break;
            }
        }
        return nextDirection;
    }

    public Room(int[] connections, int temp, bool diffDoor, int[] actualDoor)
    {
        // player room
        this.connections = connections;
        this.isPlayerRoom = true;
        this.diffDoor = diffDoor;
        this.actualDoor = actualDoor;

        //dimensions
        x = minXY; 
        y = minXY;

        for (int j = 0; j <= y + 1; j++)
        {
            cells[0,j] = GenerateWallCell(); // left wall
            cells[x+1,j] = GenerateWallCell(); // right wall
        }
        for (int j = y + 2; j < cells.GetLength(1); j++)
        {
            cells[0,j] = GenerateEmptyCell();
            cells[x+1,j] = GenerateEmptyCell();
        }

        for (int i = 1; i <= x; i++)
        {
            cells[i,0] = GenerateWallCell(); // bottom wall
            for (int j = 1; j <= y; j++)
            {
                if (j == 13 && i != 12 && i != 13 && i != 14)
                {
                    cells[i,j] = GenerateWallCell();
                }
                else
                {
                    cells[i,j] = GenerateFloorCell(0, 0); // fill whole room
                }
            }
            cells[i,y+1] = GenerateWallCell(); // top wall
            for (int j = y + 2; j < cells.GetLength(1); j++)
            {
                cells[i,j] = GenerateEmptyCell();
            }
        }
        for (int i = x + 2; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i,j] = GenerateEmptyCell();
            }
        }

        // doors
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == -1) // no door in that direction
            {
                doors[i,0] = -1;
                doors[i,1] = -1;
            }
            else // door in that direction
            {
                numDoors++;
                int doorX = -1;
                int doorY = -1;
                switch (i)
                {
                    case 0: // west
                        doorX = 0;
                        doorY = random.Next(15, 24);
                        cells[1,doorY] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{1, doorY, i});
                        break;
                    case 1: // north
                        doorX = random.Next(2, x - 1);
                        doorY = y + 1;
                        cells[doorX,y] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{doorX, y, i});
                        break;
                    case 2: // east
                        doorX = x + 1;
                        doorY = random.Next(3, 12);
                        cells[x,doorY] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{x, doorY, i});
                        break;
                    case 3: // south
                        doorX = random.Next(2, x - 1);
                        doorY = 0;
                        cells[doorX,1] = GenerateFloorCell(i, 0);
                        nextPath.Enqueue(new int[]{doorX, 1, i});
                        break;
                    default:
                        Debug.Log("uhhh");
                        break;
                }
            
                doors[i,0] = doorX;
                doors[i,1] = doorY;
            }
        }

        // place beds, chest, etc...
        cells[1,1] = GenerateFloorCell(1, -101); //bed1+auri
        cells[5,5] = GenerateFloorCell(1, -100); //bed2
        cells[15,7] = GenerateFloorCell(1, -102); //chest
        cells[7,18] = GenerateFloorCell(1, -103); //crafting

        // auri - kvothe bed - chest - crafting
        playerRoomItems[0,0] = 1;
        playerRoomItems[0,1] = 1;
        playerRoomItems[1,0] = 5;
        playerRoomItems[1,1] = 5;
        playerRoomItems[2,0] = 15;
        playerRoomItems[2,1] = 7;
        playerRoomItems[3,0] = 7;
        playerRoomItems[3,1] = 18;
    }

    // -2 for wall, -1 for empty, 0-3 for floor (WNES)
    // private CellExtended GenerateDoorLLCell()
    // {
    //     return new CellExtended(0, false);
    // }
    private int GetPrefab(int currX, int currY)
    {
        int curr = random.Next(0, 100);
        if (curr == 10 || curr == 20)
        {
            //Debug.Log("item");
            itemsPos.Add(new int[] {currX, currY});
            return ChooseItem();
        }
        else if (curr == 50) {
            //Debug.Log("enemy");
            faesPos.Add(new int[] {currX-2, currX+2, currY-2, currY+2});
            return ChooseEnemy();
        }
        else {
            return 0;
        }
    }

    private int ChooseItem() // currently -1 to -3
    {
        int curr = random.Next(0, 10);
        if (curr < 5)
        {
            return -1;
        }
        else if (curr < 8)
        {
            return -2;
        }
        else
        {
            return -3;
        }
    }

    private int ChooseEnemy() // currently 1 to 4
    {
        int curr = random.Next(0, 10);
        if (curr < 4)
        {
            return 1;
        }
        else if (curr < 7)
        {
            return 2;
        }
        else if (curr < 9)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    private CellExtended GenerateWallCell()
    {
        return new CellExtended(-2, false);
    }

    private CellExtended GenerateEmptyCell()
    {
        return new CellExtended(-1, false);
    }

    private CellExtended GenerateFloorCell(int door, int prefab)
    {
        if (prefab == 0)
        {
            return new CellExtended(door, false);
        }
        else 
        {
            return new CellExtended(door, true, prefab);
        }
    }

    public void Debugger()
    {
        for (int i = 0; i < itemsPos.Count; i++)
        {
            Debug.Log("x is " + itemsPos[i][0] + " and y is " + itemsPos[i][1]);
        }
    }
}
