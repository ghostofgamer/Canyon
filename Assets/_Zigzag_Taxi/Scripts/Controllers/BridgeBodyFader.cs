using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BridgeBodyFader : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] meshRenderers = null;


        /// <summary>
        /// Scale this fader and fade out.
        /// </summary>
        public void ScaleAndFadeOut()
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            StopAllCoroutines();
            StartCoroutine(CRScaleUp());
            foreach(MeshRenderer meshRenderer in meshRenderers) 
            {
                StartCoroutine(CRFadeOut(meshRenderer));
            }
        }


        /// <summary>
        /// Coroutine scale this object up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRScaleUp()
        {
            float scaleTime = 0.3f;
            float t = 0;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = new Vector3(2f, startScale.y, 2f);
            transform.localScale = startScale;
            while (t < scaleTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.Liner, t / scaleTime);
                transform.localScale = Vector3.Lerp(startScale, endScale, factor);
                yield return null;
            }

            transform.localScale = startScale;
            gameObject.SetActive(false);
        }



        /// <summary>
        /// Coroutine fade out.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRFadeOut(MeshRenderer meshRenderer)
        {
            float fadingTime = 0.3f;
            float t = 0;
            Color startColor = meshRenderer.material.color;
            startColor.a = 1f;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            meshRenderer.material.color = startColor;
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.Liner, t / fadingTime);
                meshRenderer.material.color = Color.Lerp(startColor, endColor, factor);
                yield return null;
            }
            meshRenderer.material.color = startColor;
        }
    }
}
