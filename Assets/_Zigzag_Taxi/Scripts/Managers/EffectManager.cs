using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class EffectManager : MonoBehaviour
    {

        public static EffectManager Instance { private set; get; }

        [SerializeField] private ParticleSystem collectCoinItemEffectPrefab = null;
        [SerializeField] private ParticleSystem playerExplodeEffectPrefab = null;
        [SerializeField] private BridgeBodyFader bridgeBodyFaderPrefab = null;
        [SerializeField] private SquareController[] squareControllerPrefabs = null;

        private List<ParticleSystem> listCollectCoinItemEffect = new List<ParticleSystem>();
        private List<ParticleSystem> listPlayerExplodeEffect = new List<ParticleSystem>();
        private List<BridgeBodyFader> listBridgeBodyFader = new List<BridgeBodyFader>();
        private List<SquareController> listSquareController = new List<SquareController>();

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


        /// <summary>
        /// Play the given particle then disable it 
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        private IEnumerator CRPlayParticle(ParticleSystem par)
        {
            par.Play();
            yield return new WaitForSeconds(2f);
            par.gameObject.SetActive(false);
        }



        /// <summary>
        /// Play a collect coin effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        public void PlayCollectCoinItemEffect(Vector3 pos)
        {
            //Find in the list
            ParticleSystem collectCoinItemEffect = listCollectCoinItemEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (collectCoinItemEffect == null)
            {
                //Didn't find one -> create new one
                collectCoinItemEffect = Instantiate(collectCoinItemEffectPrefab, pos, Quaternion.identity);
                collectCoinItemEffect.gameObject.SetActive(false);
                listCollectCoinItemEffect.Add(collectCoinItemEffect);
            }

            collectCoinItemEffect.transform.position = pos;
            collectCoinItemEffect.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(collectCoinItemEffect));
        }


        /// <summary>
        /// Create a player explode effect with given mesh and position.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="pos"></param>
        public void CreatePlayerExplodeEffect(Mesh mesh, Vector3 pos, Vector3 forward)
        {
            //Find in the list
            ParticleSystem playerExpode = listPlayerExplodeEffect.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (playerExpode == null)
            {
                //Didn't find one -> create new one
                playerExpode = Instantiate(playerExplodeEffectPrefab, Vector3.zero, Quaternion.identity);
                playerExpode.gameObject.SetActive(false);
                listPlayerExplodeEffect.Add(playerExpode);
            }

            playerExpode.transform.position = pos;
            playerExpode.transform.forward = forward;
            var shape = playerExpode.shape;
            shape.mesh = mesh;
            playerExpode.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(playerExpode));
        }



        /// <summary>
        /// Create a bridge fader effect at given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="forward"></param>
        public void CreateBridgeFaderEffect(Vector3 pos, Vector3 forward)
        {
            //Find in the list
            BridgeBodyFader bodyFader = listBridgeBodyFader.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (bodyFader == null)
            {
                //Didn't find one -> create new one
                bodyFader = Instantiate(bridgeBodyFaderPrefab, pos, Quaternion.identity);
                bodyFader.gameObject.SetActive(false);
                listBridgeBodyFader.Add(bodyFader);
            }

            bodyFader.transform.position = pos;
            bodyFader.transform.forward = forward;
            bodyFader.gameObject.SetActive(true);
            bodyFader.ScaleAndFadeOut();
        }



        /// <summary>
        /// Create square fading effect at next platform position.
        /// </summary>
        public void CreateSquareFadingEffect()
        {
            //Find in the list
            PlatformSize size = IngameManager.Instance.NextPlatform.PlatformSize;
            SquareController squareController = listSquareController.Where(a => !a.gameObject.activeSelf && a.PlatformSize.Equals(size)).FirstOrDefault();

            if (squareController == null)
            {
                //Didn't find one -> create new one
                SquareController prefab = squareControllerPrefabs.Where(a => a.PlatformSize.Equals(size)).FirstOrDefault();
                squareController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                squareController.gameObject.SetActive(false);
                listSquareController.Add(squareController);
            }

            squareController.transform.position = IngameManager.Instance.NextPlatform.transform.position;
            squareController.gameObject.SetActive(true);
            squareController.ScaleAndFadeOut();
        }
    }
}