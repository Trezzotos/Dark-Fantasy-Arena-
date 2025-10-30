using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public Vector3 playerStartPos;

    public void SetPlayer()
    {
        GetComponent<Health>().ResetToFull();
        transform.position = playerStartPos;
    }
}
