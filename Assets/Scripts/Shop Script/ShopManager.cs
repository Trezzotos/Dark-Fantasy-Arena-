using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int[,] shopItems = new int[5,5];
    public float coins;
    public Text CoinsTXT;
    private const int numberxRow = 5;
    void Start()
    {
        CoinsTXT.text = "Coins: " + coins.ToString();

        // 1) ID's  [1,2,3,...]
        for (int i = 1; i < numberxRow; i++)
            shopItems[1, i] = i;

        // 2) Price  [10,20,30,...]
        for (int i = 1; i < numberxRow; i++)
            shopItems[2, i] = i * 10;
        
        // 3) Quantity [0,0,0,...]
        for (int i = 1; i < numberxRow; i++)
            shopItems[3, i] = 0;
    }

    public void Buy()
    {
        // Reference to the bottom of the Event System
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        // check if we have enought coin for buing the item
        if (coins >= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID])
        {
            //remove the coins
            coins -= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID];

            //add the quantity 
            shopItems[3, ButtonRef.GetComponent<ButtonInfo>().ItemID]++;

            //update the text on the scene
            CoinsTXT.text = "Coins: " + coins.ToString();
            ButtonRef.GetComponent<ButtonInfo>().QuantityTxt.text = shopItems[3, ButtonRef.GetComponent<ButtonInfo>().ItemID].ToString();
        }
    }
}
