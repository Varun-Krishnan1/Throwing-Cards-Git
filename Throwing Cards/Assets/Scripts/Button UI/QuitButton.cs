using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public MainMenu mainMenu;

    public void QuitGame()
    {
        mainMenu.QuitGame();
    }
}
