using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    GameObject objectOnTile;
  
    public GameObject GetObjectOnTile()
    {return objectOnTile; }
    public void SetObjectOnTile(GameObject target)
    { objectOnTile = target;}
}
