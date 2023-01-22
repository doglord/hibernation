using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    public static FadePanel Inst; 
    void Awake(){Inst = this; image = GetComponent<Image>();}
    public UnityEvent onFadeComplete;
    Image image;
    public void RunFade(float fadeTime = 3f)
    {
        IEnumerator DoFade()
        {
            
            var t = 0f;
            if(fadeTime <= 0f)
                t = 1f;
            while(image.color.a < 1f)
            {
                var c = image.color;
                c.a = Mathf.Lerp(0f, 1f, t / fadeTime);
                image.color = c;

                t += Time.deltaTime;
                yield return null;
            }

            onFadeComplete?.Invoke();
        }
        StartCoroutine(DoFade());
    }
}
