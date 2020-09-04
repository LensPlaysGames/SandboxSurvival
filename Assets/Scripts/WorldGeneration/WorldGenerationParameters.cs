using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationParameters : MonoBehaviour
{
    // Store World Generation Values So Everything Can Just Update them From Here When Need Be
    [Header("World Generation Characteristics")]
    public int worldWidth = 270;
    public int worldHeight = 135;
    public float tileScale = 2;
    public float surfaceHeightPercentage = .81f;
    public float undergroundHeightPercentage = .69f;

    [Space] [Space] [Space]
    [Header("Tree Generation Characteristics", order = 3)]
    public float treeSpawnChance = .1f;
    public int minTreeHeight = 4, maxTreeHeight = 9;
    public float leafHeightOnTree = .5f;

    [Space]
    [Space]
    [Space]
    [Header("Tile Destroy Time Based on Type", order = 3)]
    public float dirtDestroyTime = .42f;
    public float grassDestroyTime = .54f, stoneDestroyTime = 1f, logDestroyTime = .72f, leavesDestroyTime = .02f, woodBoardsDestroyTime = .36f, devTileDestroyTime = .2f;
}
