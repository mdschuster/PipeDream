using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{


    //Singleton Region
    //********************************
    private static GameManager _instance = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public static GameManager instance()
    {
        return _instance;
    }
    //********************************


    [Header("Power Settings")]
    private float maxPower = 1f;
    private float currentPower;
    [SerializeField] private float maxTime;


    [Header("UI")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Image sliderFill;

    [Header("Managers")]
    [SerializeField] private MouseManager mouseManager;
    [SerializeField] private SpawnManager spawnManager;

    [Header("Materials")]
    [SerializeField] private Material underMapMat;
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color powerColor;


    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        gameReset();
        mouseManager.onLeftClicked += spawnManager.updateQueue;
    }

    // Update is called once per frame
    void Update()
    {
        updateSlider();
        if (isOutOfPower() && !gameOver)
        {
            gameOver = true;
            bool win = spawnManager.spawnFinalPath();
            underMapMat.SetColor("_maincolor", powerColor);
            if (win)
            {
                print("You Win");
            }
            else
            {
                print("You Lose");
            }
        }

    }

    private void gameReset()
    {
        gameOver = false;
        maxPower = 1;
        currentPower = maxPower;
        powerSlider.value = currentPower;
        sliderFill.color = new Color(0f, 1f, 0f);
        underMapMat.SetColor("_maincolor", Color.black);
    }


    private void updateSlider()
    {
        currentPower -= Time.deltaTime / maxTime;
        powerSlider.value = currentPower;
    }

    public bool isOutOfPower()
    {
        if (currentPower <= 0)
            return true;
        else
            return false;
    }

    public Vector3Int getMousePos()
    {
        return mouseManager.getMousePosition();
    }

    public GameObject getNextTile()
    {
        return spawnManager.getNextTile();
    }

    public Tile getNextPlaceableTile()
    {
        return spawnManager.getNextPlaceableTile();
    }


}
