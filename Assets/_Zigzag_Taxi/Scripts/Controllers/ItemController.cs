using System.Collections;
using UnityEngine;


namespace ClawbearGames
{
    public class ItemController : MonoBehaviour
    {

        [Header("Item Config")]
        [SerializeField] private float minRotatingSpeed = 150f;
        [SerializeField] private float maxRotatingSpeed = 350f;

        [Header("Item References")]
        [SerializeField] private MeshRenderer meshRenderer = null;
        [SerializeField] private ItemType itemType = ItemType.COIN;
        [SerializeField] private LayerMask playerLayerMask = new LayerMask();

        public ItemType ItemType { get { return itemType; } }
        private float rotatingSpeed = 0;


        /// <summary>
        /// Setup this coin item.
        /// </summary>
        public void OnSetup()
        {
            rotatingSpeed = Random.Range(minRotatingSpeed, maxRotatingSpeed);
            transform.localScale = Vector3.zero;
            StartCoroutine(CRScaleUp());
        }


        /// <summary>
        /// Coroutine scale this item up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRScaleUp()
        {
            float t = 0;
            float scaleTime = 0.25f;
            while (t < scaleTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutCubic, t / scaleTime);
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, factor);
                yield return null;
            }


            while (gameObject.activeSelf)
            {
                //Rotate this item
                transform.eulerAngles += Vector3.up * rotatingSpeed * Time.deltaTime;

                //Check collide with player
                Collider[] colliders = Physics.OverlapBox(meshRenderer.bounds.center, meshRenderer.bounds.extents, transform.rotation, playerLayerMask);
                if (colliders.Length > 0)
                {
                    if (itemType == ItemType.COIN)
                    {
                        ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.CollectCoinItem);
                        ServicesManager.Instance.CoinManager.AddCollectedCoins(1);
                        EffectManager.Instance.PlayCollectCoinItemEffect(meshRenderer.bounds.center);
                        transform.SetParent(null);
                        gameObject.SetActive(false);
                    }
                }


                //Check and disable object
                if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
                {
                    float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
                    if (distance >= 25f)
                    {
                        transform.SetParent(null);
                        gameObject.SetActive(false);
                    }
                }

                yield return null;
            }        
        }
    }
}
