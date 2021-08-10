using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{

    //Singleton Region
    //********************************
    private static MouseManager _instance = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public static MouseManager instance()
    {
        return _instance;
    }
    //********************************


    [SerializeField] private Grid mainGrid;
    [SerializeField] private Tilemap wiresMap;
    [SerializeField] private Tilemap selectionMap;
    [SerializeField] private Tilemap peekMap;
    [SerializeField] private Tile selectionTile;



    private Vector3Int previousMousePos = new Vector3Int();

    public System.Action onLeftClicked;


    // Update is called once per frame
    void Update()
    {
        updateSelectedTile();
        if (Input.GetMouseButtonDown(0))
        {
            onLeftClicked?.Invoke();
        }
    }

    private void updateSelectedTile()
    {
        Vector3Int mousePos = getMousePosition();
        if (mousePos != previousMousePos)
        {
            selectionMap.SetTile(previousMousePos, null);
            peekMap.SetTile(previousMousePos, null);
            if (wiresMap.GetTile(mousePos) != null)
            {
                selectionMap.SetTile(mousePos, selectionTile);
                peekMap.SetTile(mousePos, GameManager.instance().getNextPlaceableTile());
            }
            previousMousePos = mousePos;
        }
    }

    public Vector3Int getMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mainGrid.WorldToCell(mouseWorldPos);
    }


}
