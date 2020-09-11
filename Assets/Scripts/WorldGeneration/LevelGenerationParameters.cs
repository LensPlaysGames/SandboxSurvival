using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationParameters : MonoBehaviour
{
    public static LevelGenerationParameters instance;

    void Awake()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multple Level Generation Parameters!!"); Destroy(this.gameObject); }
        else
        {
            instance = this;
            GlobalReferences.levelGenParams = instance;
        }
    }

    [Header("Defaults")]
    public int defaultWidth;
    public int defaultHeight;
    public float defaultScale;

    // Store World Generation Values So Everything Can Just Update them From Here When Need Be
    [Space]
    [Header("Level Generation Characteristics", order = 1)]
    public int worldWidth = 270;
    public int worldHeight = 135;
    public float tileScale = 1.5f;
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
    public float defaultDestroyTime = .54f;
    public float fastestDestroyTime = .02f, fasterDestroyTime = .27f, fastDestroyTime = .36f, slowDestroyTime = .72f, slowerDestroyTime = .9f, slowestDestroyTime = 1.8f;
}
