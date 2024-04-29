using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    private static StoneSpawner inst;
    public static StoneSpawner Inst { get { return inst; } }

    [SerializeField]
    private Stone largeStonePrefab;
    [SerializeField]
    private Stone smallStonePrefab;

    [SerializeField]
    private int size;
    [SerializeField]
    private int capacity;

    private Stack<Stone> largeStonePool = new Stack<Stone>();
    private Stack<Stone> smallStonePool = new Stack<Stone>();


}
