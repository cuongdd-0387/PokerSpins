using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPlus : Button
{
    float startTime;
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        startTime = Time.time;
        base.DoStateTransition(state, instant);
    }

    private void Update()
    {
        if(currentSelectionState == SelectionState.Pressed)
        {
            if(Time.time - startTime>1)
            {
                startTime = Time.time - 0.9f;
                onClick.Invoke();
            }
        }
    }
}
