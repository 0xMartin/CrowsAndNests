using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Menu
{

    public class TextMeshFader : MonoBehaviour
    {
        public float fadeDuration = 1f;

        public TextMeshProUGUI textMesh;
        public bool isFading {private set; get; }
        private float fadeTimer = 0f;
        private float startAlpha = 0f;
        private float endAlpha = 1.0f;

        private void Start()
        {
            isFading = false;
        }

        private void Update()
        {
            if (isFading)
            {
                fadeTimer += Time.deltaTime;
                float t = fadeTimer / fadeDuration;
                Color color = textMesh.color;
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                textMesh.color = color;
                if (fadeTimer >= fadeDuration)
                {
                    isFading = false;
                }
                if(color.a == 0.0) 
                {
                    textMesh.enabled = false;
                } 
                else 
                {
                    textMesh.enabled = true;
                }
            }
        }

        public void FadeIn()
        {
            if (!isFading)
            {
                isFading = true;
                startAlpha = textMesh.color.a;
                endAlpha = 1f;
                fadeTimer = 0f;
            }
        }

        public void FadeOut()
        {
            if (!isFading)
            {
                isFading = true;
                startAlpha = textMesh.color.a;
                endAlpha = 0f;
                fadeTimer = 0f;
            }
        }

    }


}
