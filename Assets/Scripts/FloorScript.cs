using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    //Every tile knows what is on it if there is something on it
    GameObject objectOnTile;
    //Getters and Setters
    public GameObject GetObjectOnTile()
    {return objectOnTile; }
    public void SetObjectOnTile(GameObject target)
    { objectOnTile = target;}
}
