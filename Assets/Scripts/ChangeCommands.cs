using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCommands : MonoBehaviour
{
    bool cmdset = false;    // default: WASD
    public void ToggleCommands()
    {
        cmdset = !cmdset;
        if (cmdset)
        {
            // player prefs
        }
        else
        {
            // player prefs
        }
        Debug.LogWarning("ToggleCommands non è ancora implementato");
    }
}
