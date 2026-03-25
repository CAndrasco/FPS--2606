using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    
    void Start()
    {
        //make sure cursor is available
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    
    public void startGame()
    {
        //start level 1
        SceneManager.LoadScene("Level 1");
    }
    public void quitGame()
    {
        Application.Quit();
        Debug.Log("The player has quit the game.");
    }
}
