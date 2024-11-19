using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class CameraRootController : MonoBehaviour
    {
        public static CameraRootController Instance { private set; get; }

        [Header("Camera Root Configurations")]
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeAmount = 0.25f;
        [SerializeField] private float decreaseFactor = 1.5f;

        [Header("Camera Root References")]
        [SerializeField] private Transform cameraTrans = null;

        private bool isFollowPlayer = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private Vector3 offset = Vector3.zero;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            offset = transform.position - PlayerController.Instance.transform.position;
        }

        private void LateUpdate()
        {
            if (PlayerController.Instance.PlayerState == PlayerState.Player_Living && isFollowPlayer)
            {
                Vector3 targetPos = PlayerController.Instance.transform.position + offset;
                targetPos.y = transform.position.y;
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, PlayerController.Instance.transform.rotation, smoothTime);
            }
        }



        /// <summary>
        /// Active or deactive the camera to follow the player.
        /// </summary>
        /// <param name="followPlayer"></param>
        public void FollowPlayer(bool followPlayer)
        {
            isFollowPlayer = followPlayer;
        }



        /// <summary>
        /// Shake the camera.
        /// </summary>
        public void Shake()
        {
            StartCoroutine(CRShake());
        }


        /// <summary>
        /// Coroutine skae the camera.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRShake()
        {
            yield return new WaitForSeconds(0.15f);
            Vector3 originalPos = cameraTrans.localPosition;
            float shakeDurationTemp = shakeDuration;
            while (shakeDurationTemp > 0)
            {
                Vector3 newPos = originalPos + Random.insideUnitSphere * shakeAmount;
                newPos.z = originalPos.z;
                cameraTrans.localPosition = newPos;
                shakeDurationTemp -= Time.deltaTime * decreaseFactor;
                yield return null;
            }

            cameraTrans.localPosition = originalPos;
        }
    }
}
