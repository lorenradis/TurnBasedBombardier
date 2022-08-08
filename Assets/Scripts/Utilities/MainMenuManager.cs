using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    private void Update()
    {
        ManageInput();
    }

    private void ManageInput()
    {
        if (GameManager.instance.gameState != GameManager.GameState.INTRO)
            return;

        if(Input.anyKeyDown)
        {
            GameManager.instance.StartGame();
        }else if(Input.touchCount > 0)
        {
            GameManager.instance.StartGame();
        }
    }
}
