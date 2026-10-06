using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CommonUIManager.Instance.BackUI();
        }
    }
}
