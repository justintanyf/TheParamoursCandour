using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private int level;
    private int maxLevel;
    private Node[] prereqs;
    private string desc;
    private bool needsUpdating = false;

    public Node(int level, int maxLevel, Node[] prereqs, string desc)
    {
        this.level = level;
        this.maxLevel = maxLevel;
        this.prereqs = prereqs;
        this.desc = desc;
    }

    public bool PrereqsCleared()
    {
        bool cleared = true;
        foreach (Node node in prereqs)
        {
            if (!node.IsUnlocked())
            {
                cleared = false;
                break;
            }
        }

        return cleared;
    }

    public bool IsUnlocked()
    {
        return level > 0;
    }

    public bool IsMaxLevel()
    {
        return level == maxLevel;
    }

    public int GetLevel()
    {
        return level;
    }
    public int GetMaxLevel()
    {
        return maxLevel;
    }

    public void Unlock()
    {
        level++;
        needsUpdating = true;
    }

    public string GetDesc()
    {
        return this.desc;
    }
}
