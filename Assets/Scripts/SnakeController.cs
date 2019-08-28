using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    internal Node head;
    internal void InsertFront(SingleLinkedList singlyList, GameObject new_data, Transform new_Position)
    {
        Node new_node = new Node(new_data, new_Position);
        new_node.next = singlyList.head;
        singlyList.head = new_node;
    }
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
    internal Node GetLastNode(SingleLinkedList singlyList)
    {
        Node temp = singlyList.head;
        while (temp.next != null)
        {
            temp = temp.next;
        }
        return temp;
    }
    internal void Move(SingleLinkedList list, Node target, Transform pos)
    {
        if(null != target)
        {
            if (target.next != null)
            { Move(list, target.next, target.currentPos); }
            else
            {
                target.currentPos.gameObject.GetComponent<FloorScript>().SetObjectOnTile(null);
            }
            target.currentPos = pos;
            target.objectData.transform.position = target.currentPos.transform.position - Vector3.forward;
            pos.gameObject.GetComponent<FloorScript>().SetObjectOnTile(target.objectData);
        } 
    }
}
public class SnakeController : MonoBehaviour
{
    //All floats, positions, directions and a timer
    #region
    float posX;
    float posY;
    float directionX;
    float directionY;
    [SerializeField]
    float timer;
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
    bool started = false;
    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        gc = gameController.GetComponent<GameController>();
        snakeCoordinates = gc.GetCoordinates();
        posY = Random.Range(1, snakeCoordinates.Count - 1);
        posX = Random.Range(1, snakeCoordinates[Mathf.RoundToInt(posY)].Count - 1);
        currentTile = snakeCoordinates[Mathf.RoundToInt(posY)][Mathf.RoundToInt(posX)];
        tail.InsertFront(tail, gameObject, currentTile.transform);
        Move(snakeCoordinates[Mathf.RoundToInt(posY)][Mathf.RoundToInt(posX)]);
        ResetTimer();
        tail.InsertLast(tail, CreateTail(), currentTile.transform);
        tail.InsertLast(tail, CreateTail(), currentTile.transform);

    }
    private void Update()
    {
            if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0) { }
            else
            {
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
            if (timerCounter < 0)
            {
                ResetTimer();
                if (posY + directionY < snakeCoordinates.Count - 1 && posY + directionY > 0)
                { posY += directionY; }
                else
                {GameOver();}
                if (posX + directionX < snakeCoordinates[Mathf.RoundToInt(posY)].Count - 1 && posX + directionX > 0)
                { posX += directionX; }
                else
                {GameOver();}
                Move(snakeCoordinates[Mathf.RoundToInt(posY)][Mathf.RoundToInt(posX)]);
            }
            else
            { timerCounter -= Time.deltaTime; }
    } 
    void ResetTimer()
    {timerCounter = timer;}
    void Eat()
    {
        Destroy(currentTile.GetComponent<FloorScript>().GetObjectOnTile());
        gc.AddScore();
        tail.InsertLast(tail, CreateTail(), currentTile.transform) ;
        gc.createFood();
        Move(currentTile);
    }
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
        currentTile.GetComponent<FloorScript>().SetObjectOnTile(null);
        currentTile = moveTarget;
        if(currentTile.GetComponent<FloorScript>().GetObjectOnTile() == null)
        {
            currentTile.GetComponent<FloorScript>().SetObjectOnTile(gameObject);
            tail.Move(tail, tail.head, currentTile.transform);
        }
        else
        {
            if(currentTile.GetComponent<FloorScript>().GetObjectOnTile().tag == "Food")
            {Eat();}
            else
            {GameOver();}
        }
    }
    void GameOver()
    {SceneManager.LoadScene(0); }
    public GameObject GetCurrentTile()
    {return currentTile; }
}
