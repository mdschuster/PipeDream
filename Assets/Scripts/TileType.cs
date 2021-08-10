using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : MonoBehaviour
{

    [SerializeField] private string tileType;

    public string getTileType()
    {
        return tileType;
    }
}
