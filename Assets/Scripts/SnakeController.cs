using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

//A class node that keeps track of the tail, a node is a class that keeps track of which game object it is attached to and the position of that object, and the next object in line that it is connected to.
internal class Node
{
    internal GameObject objectData;
    internal Transform currentPos;
    internal Node next;
    public Node(GameObject targetGameObject, Transform originalPos)
    {
        objectData = targetGameObject;
        currentPos = originalPos;
        next = null;
    }
}
internal class SingleLinkedList
{
    //The list only keeps track of the first node, the head but allows for several methods to interact with the entire list trough the objects that are connected to the head via next
    internal Node head;
    //Creates a new node, connects it to the current head of the list and then makes the new node the head
    internal void InsertFront(SingleLinkedList singlyList, GameObject new_data, Transform new_Position)
    {
        Node new_node = new Node(new_data, new_Position);
        new_node.next = singlyList.head;
        singlyList.head = new_node;
    }
    //finds the last node in the chain and adds a connection there to a new node so that the new node is the last
    internal void InsertLast(SingleLinkedList singlyList, GameObject new_data, Transform new_Position)
    {
        Node new_node = new Node(new_data, new_Position);
        if (singlyList.head == null)
        {
            singlyList.head = new_node;
            return;
        }
        Node lastNode = GetLastNode(singlyList);
        lastNode.next = new_node;
    }
    //finds the last node and returns it by simply checking the next node until there is no next node
    internal Node GetLastNode(SingleLinkedList singlyList)
    {
        Node temp = singlyList.head;
        while (temp.next != null)
        {
            temp = temp.next;
        }
        return temp;
    }
    //A method that goes through the entire chain of nodes and changes their position both in unity and the data stored in them, also the last node removes their trace from the tile they are on
    internal void Move(SingleLinkedList list, Node target, Transform pos)
    {
        if(null != target)
        {
            if (target.next != null)
            { Move(list, target.next, target.currentPos); }
            else
            { target.currentPos.gameObject.GetComponent<FloorScript>().SetObjectOnTile(null); }
            target.currentPos = pos;
            target.objectData.transform.position = target.currentPos.transform.position - Vector3.forward;
            pos.gameObject.GetComponent<FloorScript>().SetObjectOnTile(target.objectData);
        } 
    }
}
public class SnakeController : MonoBehaviour
{
    //Positions, directions and a timer (All floats used by the object)
    #region
    float posX;
    float posY;
    float directionX;
    float directionY;
    [SerializeField]
    float timer = 1;
    float timerCounter;
    #endregion
    //Gameobjects and components
    #region
    GameObject gameController;
    GameController gc;
    List<List<GameObject>> snakeCoordinates = new List<List<GameObject>>();
    GameObject currentTile;
    [SerializeField]
    Sprite tailSprite;
    #endregion
    SingleLinkedList tail = new SingleLinkedList();
    private void Start()
    {
        //Completely unnecessary and will result in error but it removes the warning label
        if(tailSprite == null) { tailSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Prefab/Tail.png", typeof(Texture2D)); }
        //Code related to the gamecontroller
        #region
        gameController = GameObject.FindGameObjectWithTag("GameController");
        gc = gameController.GetComponent<GameController>();
        snakeCoordinates = gc.GetCoordinates();
        #endregion
        //Position generation
        #region
        posY = Random.Range(1, snakeCoordinates.Count - 1);
        posX = Random.Range(1, snakeCoordinates[Mathf.RoundToInt(posY)].Count - 1);
        currentTile = snakeCoordinates[Mathf.RoundToInt(posY)][Mathf.RoundToInt(posX)];
        Move(snakeCoordinates[Mathf.RoundToInt(posY)][Mathf.RoundToInt(posX)]);
        #endregion
        ResetTimer();
        tail.InsertFront(tail, gameObject, currentTile.transform);
        for (int i = 0; i < 2; i++)
        {tail.InsertLast(tail, CreateTail(), currentTile.transform);}
    }
    private void Update()
    {
            PlayerInput();
            //A timer that handles the movement so that it does not happen too fast
            if (timerCounter < 0)
            {
                ResetTimer();
                HandleMovement();
            }
            else
            { timerCounter -= Time.deltaTime; }
    } 
    void PlayerInput()
    {
        //Checks so that the player does not press 2 buttons at once in which case the snake will continue in the last direction it went in
        if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0) { }
        else
        {
            //Gets the direction the snake is moving through input getaxisraw so that the speed is constant
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) { }
            else
            {
                if (Input.GetAxisRaw("Horizontal") != 0)
                { directionX = 1 * Input.GetAxisRaw("Horizontal"); }
                else
                { directionX = 0; }
                if (Input.GetAxisRaw("Vertical") != 0)
                { directionY = 1 * Input.GetAxisRaw("Vertical"); }
                else
                { directionY = 0; }
            }
        }
    }
    void HandleMovement()
    {
        //Two if statements checking so that the player does not collide with a wall
        if (posY + directionY < snakeCoordinates.Count && posY + directionY >= 0)
        { posY += directionY; }
        if (posX + directionX < snakeCoordinates[Mathf.RoundToInt(posY)].Count && posX + directionX >= 0)
        { posX += directionX; }
        Move(snakeCoordinates[Mathf.RoundToInt(posY)][Mathf.RoundToInt(posX)]);
    }
    void ResetTimer()
    {timerCounter = timer;}
    //The method that is called when the player lands on a tile with food on it
    void Eat()
    {
        //Destroys the food object on the tile and calls for the gamecontroller to both add a point to the score and create a new food somewhere on the map. Also adds a new piece of tail
        Destroy(currentTile.GetComponent<FloorScript>().GetObjectOnTile());
        gc.AddScore();
        tail.InsertLast(tail, CreateTail(), currentTile.transform) ;
        gc.createFood();
        Move(currentTile);
    }
    //Creates a new tail object so that there does not need to be a prefab for the tail, which can use any sprite put into the script
    GameObject CreateTail()
    {
        GameObject temp = new GameObject("snakeTail", typeof(SpriteRenderer));
        temp.tag = "Player";
        temp.transform.localScale = Vector3.up * 0.2f + Vector3.right * 0.2f;
        temp.GetComponent<SpriteRenderer>().color = Color.green;
        temp.GetComponent<SpriteRenderer>().sprite = tailSprite;
        return temp;
    }
    private void Move(GameObject moveTarget)
    {
        //Removes self from the previous tile the object occupied and checks the desired tile to move to if there is an object on it, if there isnt, it simply moves to the tile otherwise it depends on the object on the tile
        currentTile.GetComponent<FloorScript>().SetObjectOnTile(null);
        currentTile = moveTarget;
        if(currentTile.GetComponent<FloorScript>().GetObjectOnTile() == null)
        {
            currentTile.GetComponent<FloorScript>().SetObjectOnTile(gameObject);
            tail.Move(tail, tail.head, currentTile.transform);
        }
        else
        {
            //The only time the player lands on a tile that has a gameobject on it that is not food, it will lose
            if(currentTile.GetComponent<FloorScript>().GetObjectOnTile().tag == "Food")
            {Eat();}
            else
            {GameOver();}
        }
    }
    //Reloads the scene to start over. Note: room left for improvement
    void GameOver()
    {SceneManager.LoadScene(0); }
    public GameObject GetCurrentTile()
    {return currentTile; }
}
