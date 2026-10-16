using System.Collections;
using UnityEngine;

public class SceneStartFade : MonoBehaviour
{
    /// <summary>
    /// êÌì¨ÉVÅ[ÉìÇ≈égÇ§
    /// </summary>
    private IEnumerator Start()
    {
        FadeController fade = FindObjectOfType<FadeController>();

        if (fade != null)
        {
            StartCoroutine(fade.FadeOutToLeft());

            while (fade.IsFading)
            {
                yield return null;
            }
        }

        // Ç†Ç∆Ç≈çÏÇÈ
        //GameStart();
    }
}
