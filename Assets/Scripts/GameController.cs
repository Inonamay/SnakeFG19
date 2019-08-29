using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Prefab Objects
    #region
    [SerializeField]
    GameObject tileObject;
    [SerializeField]
    GameObject foodObject;
    [SerializeField]
    GameObject playerObject;
    #endregion
    //Score related variables
    #region
    int score;
    [SerializeField]
    Text scoreText;
    #endregion
    //Coordinates used by the player to locate where to go
    List<List<GameObject>> coordinates;
    //Setup
    private void Start()
    {
        //Declaring all variables, genereting the grid, creating the player and spawning in a piece of food
        coordinates = new List<List<GameObject>>();
        GenerateGrid();
        SpawnPlayer();
        createFood();
        score = 0;
    }
    //spawn the player
    void SpawnPlayer()
    {Instantiate(playerObject);}
    //Handles the food spawning
    public void createFood()
    {
        //Simple check so that there are no two pieces of food on the map at any given time, it is public so that the player can call the method when it has destroyed the last piece of food creted
        bool isFoodOnMap = false;
        while (!isFoodOnMap)
        {
            //Spawns the food at a random location on the map that does not have an object located on it
            int y = Random.Range(1, coordinates.Count - 1);
            int x = Random.Range(1, coordinates[y].Count - 1);
            if (coordinates[y][x].GetComponent<FloorScript>().GetObjectOnTile() == null)
            {
                coordinates[y][x].GetComponent<FloorScript>().SetObjectOnTile(Instantiate(foodObject));
                coordinates[y][x].GetComponent<FloorScript>().GetObjectOnTile().transform.position = coordinates[y][x].transform.position + Vector3.back;
                isFoodOnMap = true;
            }
        }
    }
    //handles the entire gridgeneration and adds the coordinates
    void GenerateGrid()
    {
        //Double for loop creating all the tiles and adding their coordinates to a list for ease of access
        for (int y = 0; y < 20; y++)
        {
            coordinates.Add(new List<GameObject>());
            for (int x = 0; x < 36; x++)
            {
                coordinates[y].Add(Instantiate(tileObject));
                coordinates[y][x].transform.position = Vector3.up * (y * 0.5f + 0.25f) + Vector3.right * (x * 0.5f + 0.25f);
                if(y == 0 || y == 19 || x == 0 || x == 35)
                {coordinates[y][x].GetComponent<MeshRenderer>().material.color = Color.black;}
            }
        }
    }
    //Adds one score to the player and updates the scoreboard
   public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }
    //A getter
    public List<List<GameObject>> GetCoordinates()
    {return coordinates;}
}
