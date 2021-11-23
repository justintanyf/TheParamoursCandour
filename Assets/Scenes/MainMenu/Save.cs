using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public float xCoord;
    public float yCoord;
    public bool haveLastCoords;
    // underthing stuff
    public Room[] rooms;
    public List<int>[] adjList;
    public int currRoom;
}
