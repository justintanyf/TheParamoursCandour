using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Underthing
{
    public static Room[] rooms;
    public static int i = 4; // 0-3 are main rooms
    // public static List<int>[] adjList = new List<int>[2];
    public static bool initialised = false;
    private static int totalRooms = 4;
    private static Dictionary<int, int> numRoomsConn = new Dictionary<int, int> {
        { 1, 4 }, { 2, 4 }, { 3, 4 }, { 4, 5 }, { 5, 5 }, { 6, 5 }, { 7, 5 }, { 8, 6 }, { 9, 6 }, { 10, 6 }
    };
    private static int[] main0Conns = {-1, -1, -1, -1};
    private static int[] main1Conns = {-1, -1, -1, -1};
    private static int[] main2Conns = {-1, -1, -1, -1};
    private static int[] main3Conns = {-1, -1, -1, -1};

    public static void Initialise()
    {
        if (initialised)
            return;
        
        // decide on connection type for each room - first 6 in array
        int[] connArr = new int[10];
        for (int i = 0; i < 10; i++)
        {
            connArr[i] = i + 1;
        }
        System.Random random = new System.Random();
        connArr = connArr.OrderBy(x => random.Next()).ToArray();

        // need to store in array, then sum based on how many rooms to initialise room array
        for (int i = 0; i < 6; i++)
        {
            totalRooms += numRoomsConn[connArr[i]];
        }
        rooms = new Room[totalRooms];

        for (int i = 0; i < 6; i++)
        {
            Debug.Log("chose connection type " + connArr[i] + " for the connection " + i);
            TemplateChooser(connArr[i], i);
        }

        rooms[0] = GenerateMainRoom(main0Conns);
        rooms[1] = GenerateMainRoom(main1Conns);
        rooms[2] = GenerateMainRoom(main2Conns);
        rooms[3] = GenerateMainRoom(main3Conns);

        // player room
        int playerRoom = random.Next(4, totalRooms);
        Debug.Log("playerRoom is " + playerRoom);
        rooms[playerRoom] = GeneratePlayerRoom(rooms[playerRoom].connections, rooms[playerRoom].diffDoor, rooms[playerRoom].actualDoor);
        // set player last coords to the bed
        CharStats.currRoom = playerRoom;
        CharStats.haveLastCoords = true;
        CharStats.xCoord = 6;
        CharStats.yCoord = 5;

        initialised = true;
    }

    public static void TemplateChooser(int template, int type)
    {
        switch (template)
        {
            case 1:
                Template1(type);
                break;
            case 2:
                Template2(type);
                break;
            case 3:
                Template3(type);
                break;
            case 4:
                Template4(type);
                break;
            case 5:
                Template5(type);
                break;
            case 6:
                Template6(type);
                break;
            case 7:
                Template7(type);
                break;
            case 8:
                Template8(type);
                break;
            case 9:
                Template9(type);
                break;
            case 10:
                Template10(type);
                break;
            default:
                Debug.Log("how did dis happen");
                break;
        }
    }

    public static void Template1(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {-1, i+1, i+2, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, -1, i});
                rooms[i+2] = GenerateRoom(new int[] {i, i+3, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, 1, i+2});
                main1Conns[0] = i + 3;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 0, i+2, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, 3, -1});
                main3Conns[0] = i + 3;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, -1, i+1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, 2, -1});
                main2Conns[0] = i + 3;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, -1, i+1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, 2});
                main2Conns[1] = i + 3;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 1, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, -1, 3});
                main3Conns[1] = i + 3;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 2, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3});
                rooms[i+3] = GenerateRoom(new int[] {3, i+2, -1, -1});
                main3Conns[2] = i + 3;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[1];
    }

    public static void Template2(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {-1, i+2, i+1, 0}, new int[] {-1, 0, 0, 1});
                rooms[i+1] = GenerateRoom(new int[] {i, i+2, -1, -1}, new int[] {2, 2, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, i+3, i+1, -1}, new int[] {1, 3, 1, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, 1, i+2});
                main1Conns[0] = i + 3;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 0, i+1, i+2}, new int[] {-1, 3, 0, 0});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+2}, new int[] {2, -1, -1, 2});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, i+1, i+3}, new int[] {3, -1, 3, 1});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, 3, -1});
                main3Conns[0] = i + 3;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, -1, i+2, i+1}, new int[] {2, -1, 1, 1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, i+2, -1}, new int[] {-1, 3, 3, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, i+1}, new int[] {-1, 2, 0, 2});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, 2, -1});
                main2Conns[0] = i + 3;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, -1, i+2, i+1}, new int[] {2, -1, 1, 1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, i+2, -1}, new int[] {-1, 3, 3, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, i+1}, new int[] {-1, 2, 0, 2});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, 2});
                main2Conns[1] = i + 3;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 1, -1, i+2}, new int[] {2, 3, -1, 2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+2}, new int[] {-1, -1, 0, 0});
                rooms[i+2] = GenerateRoom(new int[] {i+1, -1, i, i+3}, new int[] {3, -1, 3, 1});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, -1, 3});
                main3Conns[1] = i + 3;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 2, -1, i+2}, new int[] {2, 3, -1, 2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+2}, new int[] {-1, -1, 0, 0});
                rooms[i+2] = GenerateRoom(new int[] {i+1, -1, i, i+3}, new int[] {3, -1, 3, 1});
                rooms[i+3] = GenerateRoom(new int[] {3, i+2, -1, -1});
                main3Conns[2] = i + 3;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[2];
    }

    public static void Template3(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {-1, i+1, -1, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, i+2, -1, i});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+3, -1, i+1});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, 1, i+2});
                main1Conns[0] = i + 3;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 0, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, i+2});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, 3, -1});
                main3Conns[0] = i + 3;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, -1, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, i+2, -1});
                rooms[i+2] = GenerateRoom(new int[] {i+1, -1, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, 2, -1});
                main2Conns[0] = i + 3;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, -1, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, i+2, -1});
                rooms[i+2] = GenerateRoom(new int[] {i+1, -1, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, 2});
                main2Conns[1] = i + 3;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 1, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, i+2});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, -1, 3});
                main3Conns[1] = i + 3;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 2, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, i+2});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {3, i+2, -1, -1});
                main3Conns[2] = i + 3;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[3];
    }

    public static void Template4(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {-1, i+1, i+2, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i});
                rooms[i+2] = GenerateRoom(new int[] {i, i+3, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+1, i+4, -1, i+2});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, 1, i+3});
                main1Conns[0] = i + 4;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 0, i+2, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, i+3, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {i+1, i+2, -1, i+4});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, 3, -1});
                main3Conns[0] = i + 4;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, -1, i+1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, i+1, i+4, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, 2, -1});
                main2Conns[0] = i + 4;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, -1, i+1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, i+1, i+4, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, -1, 2});
                main2Conns[1] = i + 4;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 1, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {i+3, i, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, i+1, i+4});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, -1, 3});
                main3Conns[1] = i + 4;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 2, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {i+3, i, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, i+1, i+4});
                rooms[i+4] = GenerateRoom(new int[] {3, i+3, -1, -1});
                main3Conns[2] = i + 4;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[4];
    }

    public static void Template5(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {-1, i+1, -1, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, i+2, -1, i});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i+3, i+1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, i+4, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, 1, -1});
                main1Conns[0] = i + 4;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 0, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, i+2});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+1, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, i+4, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, 3, -1});
                main3Conns[0] = i + 4;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, -1, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, i+2, -1});
                rooms[i+2] = GenerateRoom(new int[] {i+1, i+3, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, i+4, i+2});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, 2, -1});
                main2Conns[0] = i + 4;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, -1, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, i+2, -1});
                rooms[i+2] = GenerateRoom(new int[] {i+1, -1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, -1, i+4});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, -1, 2});
                main2Conns[1] = i + 4;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 1, -1, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+2});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+1, i+3, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, i+4});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, -1, 3});
                main3Conns[1] = i + 4;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 2, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {-1, i, -1, i+2});
                rooms[i+2] = GenerateRoom(new int[] {i+3, i+1, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+4, -1, i+2, -1});
                rooms[i+4] = GenerateRoom(new int[] {3, -1, i+3, -1});
                main3Conns[2] = i + 4;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[5];
    }

    public static void Template6(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, -1, i+2, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, i+3, i, -1}, new int[] {-1, 0, 0, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, i+3, -1, -1}, new int[] {2, 2, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+1, i+4, i+2, -1}, new int[] {1, 3, 1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, 1, i+3});
                main1Conns[0] = i + 4;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 0, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+3}, new int[] {-1, -1, 0, 0});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3}, new int[] {2, -1, -1, 2});
                rooms[i+3] = GenerateRoom(new int[] {i+1, -1, i+2, i+4}, new int[] {3, -1, 3, 1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, 3, -1});
                main3Conns[0] = i + 4;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, i+1, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i}, new int[] {-1, -1, 1, 1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1}, new int[] {-1, 3, 3, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, i+4, i+2}, new int[] {-1, 2, 0, 2});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, 2, -1});
                main2Conns[0] = i + 4;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, i+1, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i}, new int[] {-1, -1, 1, 1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1}, new int[] {-1, 3, 3, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, i+4, i+2}, new int[] {-1, 2, 0, 2});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, -1, 2});
                main2Conns[1] = i + 4;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 1, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+3}, new int[] {-1, -1, 0, 0});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3}, new int[] {2, -1, -1, 2});
                rooms[i+3] = GenerateRoom(new int[] {i+1, -1, i+2, i+4}, new int[] {3, -1, 3, 1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, -1, 3});
                main3Conns[1] = i + 4;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 2, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+3}, new int[] {-1, -1, 0, 0});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3}, new int[] {2, -1, -1, 2});
                rooms[i+3] = GenerateRoom(new int[] {i+1, -1, i+2, i+4}, new int[] {3, -1, 3, 1});
                rooms[i+4] = GenerateRoom(new int[] {3, i+3, -1, -1});
                main3Conns[2] = i + 4;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[6];
    }

    public static void Template7(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, -1, i+2, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, i+3, i, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, i+4, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, -1, i+1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, 1, i+2});
                main1Conns[0] = i + 4;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 0, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+3});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+4});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+2, 3, -1});
                main3Conns[0] = i + 4;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, i+1, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+4, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+1, -1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+2, -1, 2, -1});
                main2Conns[0] = i + 4;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, i+1, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+4, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+1, -1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+2, -1, -1, 2});
                main2Conns[1] = i + 4;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 1, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+4});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+2, -1, 3});
                main3Conns[1] = i + 4;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 2, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+4});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {3, i+2, -1, -1});
                main3Conns[2] = i + 4;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[7];
    }

    public static void Template8(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, i+1, -1, 0});
                rooms[i+1] = GenerateRoom(new int[] {i+3, -1, i+4, i});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+3, i, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, i+1, i+2});
                rooms[i+4] = GenerateRoom(new int[] {i+1, i+5, -1, -1});
                rooms[i+5] = GenerateRoom(new int[] {-1, -1, 1, i+4});
                main1Conns[0] = i + 5;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 0, -1, i+1});
                rooms[i+1] = GenerateRoom(new int[] {i+3, i, i+4, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, i+1, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+1, -1, -1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {-1, i+4, 3, -1});
                main3Conns[0] = i + 5;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, i+2, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, i+3, -1, i+4});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i+3, i});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, i+1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+1, i+5, -1});
                rooms[i+5] = GenerateRoom(new int[] {i+4, -1, 2, -1});
                main2Conns[0] = i + 5;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, i+2, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, i+3, -1, i+4});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i+3, i});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, i+1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+1, i+5, -1});
                rooms[i+5] = GenerateRoom(new int[] {i+4, -1, -1, 2});
                main2Conns[1] = i + 5;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 1, i+2, i+1});
                rooms[i+1] = GenerateRoom(new int[] {i+4, i, i+3, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {i+1, i+2, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, i+1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {-1, i+4, -1, 3});
                main3Conns[1] = i + 5;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 2, i+2, i+1});
                rooms[i+1] = GenerateRoom(new int[] {i+4, i, i+3, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3});
                rooms[i+3] = GenerateRoom(new int[] {i+1, i+2, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, i+1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {3, i+4, -1, -1});
                main3Conns[2] = i + 5;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[8];
    }

    public static void Template9(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, i+2, -1, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i+5, i+3, i});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, i+5, -1});
                rooms[i+5] = GenerateRoom(new int[] {i+4, -1, 1, i+2});
                main1Conns[0] = i + 5;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 0, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, -1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, i+5});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, i+5, -1});
                rooms[i+5] = GenerateRoom(new int[] {i+4, i+2, 3, -1});
                main3Conns[0] = i + 5;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, i+1, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, -1, i});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, i+5, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, -1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {i+2, i+4, 3, -1});
                main2Conns[0] = i + 5;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, i+1, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, -1, i});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, i+5, i+3});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+2, -1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, -1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {i+2, i+4, -1, 2});
                main2Conns[1] = i + 5;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 1, i+1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {i+3, i, -1, i+5});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, i+2, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+5, -1, -1, -1});
                rooms[i+5] = GenerateRoom(new int[] {-1, i+2, i+4, 3});
                main3Conns[1] = i + 5;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {-1, 2, i+1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, -1});
                rooms[i+2] = GenerateRoom(new int[] {i+3, i, -1, i+5});
                rooms[i+3] = GenerateRoom(new int[] {-1, -1, i+2, -1});
                rooms[i+4] = GenerateRoom(new int[] {i+5, -1, -1, -1});
                rooms[i+5] = GenerateRoom(new int[] {3, i+2, i+4, -1});
                main3Conns[2] = i + 5;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[9];
    }

    public static void Template10(int type)
    {
        switch (type)
        {
            case 0:
                main0Conns[1] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, -1, i+2, 0});
                rooms[i+1] = GenerateRoom(new int[] {-1, i+3, i, -1}, new int[] {-1, 0, 0, -1});
                rooms[i+2] = GenerateRoom(new int[] {i, i+3, -1, -1}, new int[] {2, 2, -1, -1});
                rooms[i+3] = GenerateRoom(new int[] {i+1, i+4, i+2, -1}, new int[] {1, 3, 1, -1});
                rooms[i+4] = GenerateRoom(new int[] {-1, -1, i+5, i+3});
                rooms[i+5] = GenerateRoom(new int[] {i+4, -1, 1, -1});
                main1Conns[0] = i + 5;
                break;
            case 1:
                main0Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+1, 0, i+2, -1});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i, i+3}, new int[] {-1, -1, 0, 0});
                rooms[i+2] = GenerateRoom(new int[] {i, -1, -1, i+3}, new int[] {2, -1, -1, 2});
                rooms[i+3] = GenerateRoom(new int[] {i+1, -1, i+2, i+4}, new int[] {3, -1, 3, 1});
                rooms[i+4] = GenerateRoom(new int[] {-1, i+3, i+5, -1});
                rooms[i+5] = GenerateRoom(new int[] {i+4, -1, 3, -1});
                main3Conns[0] = i + 5;
                break;
            case 2:
                main0Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {0, i+1, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i}, new int[] {-1, -1, 1, 1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1}, new int[] {-1, 3, 3, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, i+4, i+2}, new int[] {-1, 2, 0, 2});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, -1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {-1, i+4, 2, -1});
                main2Conns[0] = i + 5;
                break;
            case 3:
                main1Conns[2] = i;
                rooms[i] = GenerateRoom(new int[] {1, i+1, -1, i+2});
                rooms[i+1] = GenerateRoom(new int[] {-1, -1, i+3, i}, new int[] {-1, -1, 1, 1});
                rooms[i+2] = GenerateRoom(new int[] {-1, i, i+3, -1}, new int[] {-1, 3, 3, -1});
                rooms[i+3] = GenerateRoom(new int[] {-1, i+1, i+4, i+2}, new int[] {-1, 2, 0, 2});
                rooms[i+4] = GenerateRoom(new int[] {i+3, -1, -1, i+5});
                rooms[i+5] = GenerateRoom(new int[] {-1, i+4, -1, 2});
                main2Conns[1] = i + 5;
                break;
            case 4:
                main1Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 1, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+3}, new int[] {2, -1, -1, 2});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3}, new int[] {-1, -1, 0, 0});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, i+1, i+4}, new int[] {3, -1, 3, 1});
                rooms[i+4] = GenerateRoom(new int[] {i+5, i+3, -1, -1});
                rooms[i+5] = GenerateRoom(new int[] {-1, -1, i+4, 3});
                main3Conns[1] = i + 5;
                break;
            case 5:
                main2Conns[3] = i;
                rooms[i] = GenerateRoom(new int[] {i+2, 2, i+1, -1});
                rooms[i+1] = GenerateRoom(new int[] {i, -1, -1, i+3}, new int[] {2, -1, -1, 2});
                rooms[i+2] = GenerateRoom(new int[] {-1, -1, i, i+3}, new int[] {-1, -1, 0, 0});
                rooms[i+3] = GenerateRoom(new int[] {i+2, -1, i+1, i+4}, new int[] {3, -1, 3, 1});
                rooms[i+4] = GenerateRoom(new int[] {i+5, i+3, -1, -1});
                rooms[i+5] = GenerateRoom(new int[] {3, -1, i+4, -1});
                main3Conns[2] = i + 5;
                break;
            default:
                Debug.Log("ono what");
                break;
        }

        i += numRoomsConn[10];
    }
    
    public static Room GenerateRoom(int[] connections)
    {
        Room newRoom = new Room(connections, false, new int[] {-1, -1, -1, -1});
        return newRoom;
    }

    public static Room GenerateRoom(int[] connections, int[] actualDoor)
    {
        Room newRoom = new Room(connections, true, actualDoor);
        return newRoom;
    }
    // just in case need predefined stuff
    public static Room GenerateMainRoom(int[] connections)
    {
        Room newRoom = new Room(connections, false, new int[] {-1, -1, -1, -1});
        return newRoom;
    }

    public static Room GeneratePlayerRoom(int[] connections, bool diffDoor, int[] actualDoor)
    {
        Room newRoom = new Room(connections, 1, diffDoor, actualDoor);
        return newRoom;
    }
}
