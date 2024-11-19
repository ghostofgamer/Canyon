using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class CenterController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer = null;
        [SerializeField] private Material centerMaterial = null;
        [SerializeField] private Material fadeMaterial = null;

        private void OnDisable()
        {
            meshRenderer.sharedMaterial = centerMaterial;
            meshRenderer.enabled = true;
        }



        /// <summary>
        /// Determine the given position is inside this center object.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsInside(Vector3 pos)
        {
            Vector3 maxPos = transform.position + transform.forward * 0.5f;
            Vector3 minPos = transform.position - transform.forward * 0.5f;
            if (Vector3.Distance(pos, maxPos) <= 1f && Vector3.Distance(pos, minPos) <= 1f)
                return true;
            else return false;
        }




        /// <summary>
        /// Scale this fader and fade out.
        /// </summary>
        public void ScaleAndFadeOut()
        {
            meshRenderer.sharedMaterial = fadeMaterial;
            transform.localScale = new Vector3(1f, 1f, 1f);
            StopAllCoroutines();
            StartCoroutine(CRScaleUpAndFadeOut());
        }


        /// <summary>
        /// Coroutine scale this object up and fade it out.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRScaleUpAndFadeOut()
        {
            float scaleTime = 0.3f;
            float t = 0;

            Vector3 startScale = transform.localScale;
            Vector3 endScale = new Vector3(2f, startScale.y, 2f);
            transform.localScale = startScale;

            Color startColor = meshRenderer.material.color;
            startColor.a = 1f;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            meshRenderer.material.color = startColor;

            while (t < scaleTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.Liner, t / scaleTime);
                transform.localScale = Vector3.Lerp(startScale, endScale, factor);
                meshRenderer.material.color = Color.Lerp(startColor, endColor, factor);
                yield return null;
            }

            transform.localScale = startScale;
            meshRenderer.material.color = startColor;
            meshRenderer.enabled = false;
        }
    }
}
