using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationParameters : MonoBehaviour
{
    // Store World Generation Values So Everything Can Just Update them From Here When Need Be
    [Header("World Generation Characteristics")]
    public float surfaceHeightPercentage = .81f;
    public float undergroundHeightPercentage = .69f;
    [Space] [Space] [Space]
    [Header("Tree Generation Characteristics", order = 3)]
    public float treeSpawnChance = .1f;
    public int minTreeHeight = 4, maxTreeHeight = 9;
    public float leafHeightOnTree = .5f;
}
