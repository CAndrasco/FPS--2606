using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        gamemanager.instance.stateUnpause();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void loadLevel(int lvl)
    {
        gamemanager.instance.stateUnpause();

        SceneManager.LoadScene(lvl);
    }
}
