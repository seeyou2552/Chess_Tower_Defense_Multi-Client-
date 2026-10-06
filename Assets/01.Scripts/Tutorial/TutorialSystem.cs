using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TutorialSystem : MonoBehaviour
{
    [Header("Tutorial Data")]
    [SerializeField] private TutorialData _data;
    public int CurrentStep { get; private set; }
    public bool IsRunning { get; private set; }

    private List<TutorialStep> _steps;

    public void Init()
    {
        if (_data == null)
            _data = GetComponent<TutorialData>();
            
        _steps = _data.InitializeTutorialSteps();
    }

    public TutorialStep GetStep()
    {
        return _steps[CurrentStep];
    }

    public bool NextStep()
    {
        CurrentStep++;

        return _steps.Count > CurrentStep;
    }

    public void Reset()
    {
        Init();
        CurrentStep = 0;
        IsRunning = true;
    }
}




