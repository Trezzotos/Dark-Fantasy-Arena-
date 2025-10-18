using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Examples.Observer
{
    [RequireComponent(typeof(Collider2D))]
    public class Pickable : MonoBehaviour
    {
        public static event Action<PickableData> ItemPickedUp;

        public PickableData itemToGive;

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                ItemPickedUp.Invoke(itemToGive);
                Destroy(gameObject);
            }
        }
    }
}