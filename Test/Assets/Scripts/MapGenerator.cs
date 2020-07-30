using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{

    // Variable Initialization
    // Map Size
    public int width;
    public int height;
    public int borderSize;

    // How many times to smooth the map
    public int smoothAmount;

    // Seed variables
    public string seed;
    public bool useRandomSeed;

    // Determines the likely of random filling
    [Range(0, 100)]
    public int randomFillPercent;

    // Map array
    int[,] map;

    public int wallThresholdSize;
    public int roomThresholdSize;

    void Start()
    {
        GenerateMap();
    }


    // Test new map generating
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    // Generates a map layout
    void GenerateMap()
    {

        // Creates the map array size
        map = new int[width,height];
        RandomFillMap();

        // Loops smoothAmount times to smooth the map
        for (int i = 0; i < smoothAmount; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        // Makes a border around the map
        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if ( x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                } else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        // Makes the map appear
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);
    }

    // Method to make the map have less small pockets and small pillars
    void ProcessMap()
    {

        // List to get all the wall regions
        List<List<Coord>> wallRegions = GetRegions(1);

        // Makes all regions of wall less than the wall threshold size an empty pocket
        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        // List to get all the rooms regions
        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        // Makes all regions of rooms less than the room threshold size an wall then adds the surviving rooms to a list
        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            } else
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }

        ConnectClosestRooms(survivingRooms);
    }

    // Connects rooms with the closest room
    void ConnectClosestRooms(List<Room> allRooms)
    {

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        // Runs through each room forming a best connection spot between two rooms and connecting them together via passage
        foreach(Room roomA in allRooms)
        {
            possibleConnectionFound = false;
            foreach(Room roomB in allRooms)
            {
                if (roomA == roomB)
                {
                    continue;
                }
                if (roomA.IsConnected(roomB))
                {
                    possibleConnectionFound = false;
                    break;
                }
                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }
    }

    // Connects two rooms based on best connection points
    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);
    }

    // Creates a real spot representation of a coord
    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    // Method to get all the regions
    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];


        // Runs through each tile in the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                // Checks if the tile is our current tile type and not already handled
                if (mapFlags[x,y] == 0 && map[x,y] == tileType)
                {

                    // Creates a new region and fills the list with the coords
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    // Sets the flags on all the tiles to handled
                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    // Gets all the tiles in a given region
    List<Coord> GetRegionTiles(int startX, int startY)
    {

        // Variables needed for this method
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;


        // While there are more tiles to check
        while (queue.Count > 0)
        {

            // Removes the tile from the queue and adds it to the tiles list
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            // Loops untill the 4 tiles surrounding the current tile are checked
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {

                    // If tile is in map and one of the tiles we need to look at
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {

                        // Checks if the tile matches our current tile type and not already handled
                        if (mapFlags[x, y] == 0 && map[x,y] == tileType)
                        {

                            // Sets the flag to be checked and adds the new region tile to the queue
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x,y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    // Checks if the current position is in the map size
    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // Fills in the map
    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random MapTileRNG = new System.Random(seed.GetHashCode());

        // Fills in the map array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                // Makes sure the edges of the map are walls
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (MapTileRNG.Next(0, 100) < randomFillPercent) ? 1 : 0;

                }
            }
        }
    }


    // Smooths the map
    void SmoothMap()
    {

        // Loops through the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                // Gets the amount of walls around current node
                int neighbourWallTiles = GetSurrondingWallCount(x, y);

                // If node has more than 4 wall tiles around it becomes a wall, less than 4 becomes air, if 4 stays at current value
                if (neighbourWallTiles > 4)
                {
                    map[x, y] = 1;
                } else if (neighbourWallTiles < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    // Counts how many walls are around the current node
    int GetSurrondingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        // Looping through 3x3 grind centered on gridX,gridY
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {

                // Make sure inside the map
                if (IsInMapRange(neighbourX, neighbourY))
                {

                    // For all nodes except current node
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    // Sets up a structure to easily get tile coords
    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    // Class to organize rooms
    class Room
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;

        // Constructor without info
        public Room()
        {
        }

        // Constructor when given info
        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            
            // Gets a list of all the edge tiles
            edgeTiles = new List<Coord>();
            foreach( Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        // Adds the two rooms to each other connected rooms
        public static void ConnectRooms(Room roomA, Room roomB)
        {
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        // Checks if the room is connected
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

    }
}
