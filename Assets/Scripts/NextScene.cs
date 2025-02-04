using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("Cutscene2",LoadSceneMode.Single);
    }
}
