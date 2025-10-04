using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private bool facingRight = true;

    void Update()
    {
        Flip();
    }

    public void Flip()
    {
        // Se premo A → voglio guardare a sinistra
        if (Input.GetKey(KeyCode.A) && facingRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            facingRight = false;
        }
        // Se premo D → voglio guardare a destra
        else if (Input.GetKey(KeyCode.D) && !facingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            facingRight = true;
        }
    }
}
