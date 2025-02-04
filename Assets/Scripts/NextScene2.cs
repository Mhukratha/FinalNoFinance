using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene2 : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("MainGame1",LoadSceneMode.Single);
    }
}

