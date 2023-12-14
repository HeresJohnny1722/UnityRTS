using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipTest : MonoBehaviour
{
    private float timer;

    private void Start()
    {
        System.Func<string> gettooltipTextfunc = () =>
        {
            return "This is my tooltip!\nThere are many like it but this one is mine!\n" + timer;
        };
        TooltipCanvas.ShowTooltip_Static("This is my tooltip!\nThere are many like it but this one is mine!");
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }
}
