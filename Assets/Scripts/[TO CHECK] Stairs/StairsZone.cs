using UnityEngine;

public class StairZone : MonoBehaviour
{
    public string stairSortingLayer = "Stairs";
    public string defaultSortingLayer = "Default";

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
        CharacterOnStairs character = other.GetComponent<CharacterOnStairs>();

        if (sr != null)
        {
            sr.sortingLayerName = stairSortingLayer;
        }

        if (character != null)
        {
            character.isOnStairs = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
        CharacterOnStairs character = other.GetComponent<CharacterOnStairs>();

        if (sr != null)
        {
            sr.sortingLayerName = defaultSortingLayer;
        }

        if (character != null)
        {
            character.isOnStairs = false;
        }
    }
}
