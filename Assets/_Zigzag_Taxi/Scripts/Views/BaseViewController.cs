using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class BaseViewController : MonoBehaviour
    {
        [SerializeField] private ViewType viewType = ViewType.HOME_VIEW;
        public ViewType ViewType { get { return viewType; } }


        /// <summary>
        /// Handle actions when this view is shown.
        /// </summary>
        public virtual void OnShow() { }


        /// <summary>
        /// Handle actions when this view is closed.
        /// </summary>
        public virtual void OnClose() { }




        /// <summary>
        /// Move the given RectTransform from startPos to endPos with movingTime,
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="movingTime"></param>
        public void MoveRectTransform(RectTransform rect, Vector2 startPos, Vector2 endPos, float movingTime)
        {
            StartCoroutine(CRMoveRect(rect, startPos, endPos, movingTime));
        }


        /// <summary>
        /// Coroutine move the given RectTransform.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="movingTime"></param>
        /// <returns></returns>
        private IEnumerator CRMoveRect(RectTransform rect, Vector2 startPos, Vector2 endPos, float movingTime)
        {
            Vector2 currentPos = new Vector2(Mathf.RoundToInt(rect.anchoredPosition.x), Mathf.RoundToInt(rect.anchoredPosition.y));
            if (!currentPos.Equals(endPos))
            {
                rect.anchoredPosition = startPos;
                float t = 0;
                while (t < movingTime)
                {
                    t += Time.deltaTime;
                    float factor = EasyType.MatchedLerpType(LerpType.EaseOutQuart, t / movingTime);
                    rect.anchoredPosition = Vector2.Lerp(startPos, endPos, factor);
                    yield return null;
                }
            }
        }




        /// <summary>
        /// Scale the given RectTransform from startScale to endScale with scalingTime.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="startScale"></param>
        /// <param name="endScale"></param>
        /// <param name="scalingTime"></param>
        public void ScaleRectTransform(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime)
        {
            StartCoroutine(CRScaleRect(rect, startScale, endScale, scalingTime));
        }

        /// <summary>
        /// Coroutine scale the given RectTransform.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="startScale"></param>
        /// <param name="endScale"></param>
        /// <param name="scalingTime"></param>
        /// <returns></returns>
        private IEnumerator CRScaleRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime)
        {
            rect.localScale = startScale;
            float t = 0;
            while (t < scalingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutQuart, t / scalingTime);
                rect.localScale = Vector2.Lerp(startScale, endScale, factor);
                yield return null;
            }
        }



        /// <summary>
        /// Fade in the given CanvasGroup with fadingTime.
        /// </summary>
        /// <param name="canvasGroup"></param>
        /// <param name="fadingTime"></param>
        /// <param name="delay"></param>
        public void FadeInCanvasGroup(CanvasGroup canvasGroup, float fadingTime, float delay = 0)
        {
            StartCoroutine(CRFadeInCanvasGroup(canvasGroup, fadingTime, delay));
        }


        /// <summary>
        /// Coroutine fade in the given CanvasGroup with fadingTime.
        /// </summary>
        /// <param name="canvasGroup"></param>
        /// <param name="fadingTime"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRFadeInCanvasGroup(CanvasGroup canvasGroup, float fadingTime, float delay)
        {
            yield return new WaitForSeconds(delay);
            canvasGroup.alpha = 0f;
            float t = 0;
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutQuart, t / fadingTime);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, factor);
                yield return null;
            }
        }



        /// <summary>
        /// Fade out the given CanvasGroup with fadingTime.
        /// </summary>
        /// <param name="canvasGroup"></param>
        /// <param name="fadingTime"></param>
        public void FadeOutCanvasGroup(CanvasGroup canvasGroup, float fadingTime)
        {
            StartCoroutine(CRFadeOutCanvasGroup(canvasGroup, fadingTime));
        }



        /// <summary>
        /// Coroutine fade out the given CanvasGroup with fadingTime.
        /// </summary>
        /// <param name="canvasGroup"></param>
        /// <param name="fadingTime"></param>
        /// <returns></returns>
        private IEnumerator CRFadeOutCanvasGroup(CanvasGroup canvasGroup, float fadingTime)
        {
            canvasGroup.alpha = 1f;
            float t = 0;
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutQuart, t / fadingTime);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, factor);
                yield return null;
            }
        }




        /// <summary>
        /// Load Loading scene with a delay time then use LoadSceneAsync to load the given scene.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="delay"></param>
        public void LoadScene(string sceneName, float delay)
        {
            StartCoroutine(CRLoadScene(sceneName, delay));
        }


        /// <summary>
        /// Coroutine load Loading scene then load the given scene with sceneName.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRLoadScene(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            LoadingManager.SetTargetScene(sceneName);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
        }
    }
}
