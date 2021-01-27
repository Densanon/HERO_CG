using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonHold : MonoBehaviour
{
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //Checking what to do with said touch

            if(Input.touchCount == 2)
            {
                touch = Input.GetTouch(1);
                //checking what to do with a second touch
            }
        }
    }
}
