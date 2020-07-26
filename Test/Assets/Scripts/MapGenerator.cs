using System.Collections;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{

    // Variable Initialization
    // Map Size
    public int width;
    public int height;

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
                if (neighbourX >=0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
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

    // Draws the visual representation of the random map
    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + 0.5f, 0, -height/2 + y + 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }

    }
}
