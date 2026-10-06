using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInput : MonoBehaviour
{
    public event Action OnTap;
    public bool CanTouchToProgress = false;
    public bool IsRunning = false;

    void Update()
    {
        if(IsRunning && CanTouchToProgress && Input.touchCount > 0)
            OnTap?.Invoke();
    }
}
