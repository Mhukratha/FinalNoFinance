using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene2 : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("MAP1",LoadSceneMode.Single);
    }
}

