using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellExtended
{
    public int mainCell;
    public bool hasObj;
    public int prefab;

    public CellExtended(int mainCell, bool hasObj, int prefab=0) //need to find a better way to deal with no obj
    {
        this.mainCell = mainCell;
        this.hasObj = hasObj;
        if (hasObj)
        {
            this.prefab = prefab;
        }
    }
}
