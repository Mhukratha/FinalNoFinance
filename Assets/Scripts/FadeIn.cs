using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    private bool fadein = false;

    public float timeToFade;

    void Update()
    {
        if (fadein)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += timeToFade * Time.fixedDeltaTime;
                if (canvasGroup.alpha >= 1)
                {
                    fadein = false;
                }
            }
        }
    }

    public void _FadeIn()
    {
        fadein = true;
    }
}
