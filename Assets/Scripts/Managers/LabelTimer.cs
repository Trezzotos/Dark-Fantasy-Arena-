using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelTimer : Timer
{
    public TMP_Text label;

    protected override IEnumerator PassTime()
    {
        label.text = "" + GetCurrentTime();
        return base.PassTime();
    }
    public override void StartTimer()
    {
        base.StartTimer();
    }
}
