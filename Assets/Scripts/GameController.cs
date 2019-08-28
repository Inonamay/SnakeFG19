using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject tile;
    List<List<GameObject>> coordinates;
    [SerializeField]
    float timer;
    float timerCounter;
    [SerializeField]
    GameObject food;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject tail;
    int score;
    [SerializeField]
    Text scoreText;
    private void Start()
    {
        coordinates = new List<List<GameObject>>();
        GenerateGrid();
        SpawnPlayer();
        createFood();
        score = 0;
    }
    private void Update()
    {
        
    }
    void SpawnPlayer()
    {
        GameObject temp = Instantiate(player);
    }
    public void createFood()
    {
        bool isFood = false;
        while (!isFood)
        {
            int y = Random.Range(1, coordinates.Count - 1);
            int x = Random.Range(1, coordinates[y].Count - 1);
            if (coordinates[y][x].GetComponent<FloorScript>().GetObjectOnTile() == null)
            {
                coordinates[y][x].GetComponent<FloorScript>().SetObjectOnTile(Instantiate(food));
                coordinates[y][x].GetComponent<FloorScript>().GetObjectOnTile().transform.position = coordinates[y][x].transform.position + Vector3.back;
                isFood = true;
            }
        }
    }

    void GenerateGrid()
    {
        for (int y = 0; y < 20; y++)
        {
            coordinates.Add(new List<GameObject>());
            for (int x = 0; x < 36; x++)
            {
                coordinates[y].Add(Instantiate(tile));
                coordinates[y][x].transform.position = Vector3.up * (y * 0.5f + 0.25f) + Vector3.right * (x * 0.5f + 0.25f);
                if(y == 0 || y == 19 || x == 0 || x == 35)
                {
                    coordinates[y][x].GetComponent<MeshRenderer>().material.color = Color.black;
                }
            }
        }
    }
   public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }
    public List<List<GameObject>> GetCoordinates()
    {
        return coordinates;
    }
}
