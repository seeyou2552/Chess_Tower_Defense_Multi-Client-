using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUIManager : Singleton<CommonUIManager>
{
    public LoadingUI LoadingUI;
    private Stack<Action> _uiStack = new Stack<Action>();
    

    public void AddUIStack(Action hideAction)
    {
        _uiStack.Push(hideAction);
    }

    public void BackUI()
    {
        Action topUI = _uiStack.Count > 0 ? _uiStack.Pop() : null;

        if (topUI != null)
            topUI?.Invoke();
        else
            EventBus.Publish(new UIExitEvent());
    }

    public void AllHideUI()
    {
        while (_uiStack.Count > 0)
        {
            var ui = _uiStack.Pop();

            ui?.Invoke();
        }
    }
}

#region EventBus

public readonly struct UIExitEvent {}

#endregion