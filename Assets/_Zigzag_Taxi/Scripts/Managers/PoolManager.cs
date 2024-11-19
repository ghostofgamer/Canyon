using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { private set; get; }

        [SerializeField] private BridgeHeadController bridgeHeadControllerPrefab = null;
        [SerializeField] private BridgeBodyController bridgeBodyControllerPrefab = null;
        [SerializeField] private PlatformPrefabConfiguration[] platformPrefabConfigurations = null;
        [SerializeField] private ItemController[] itemControllerPrefabs = null;


        private List<BridgeHeadController> listBridgeHeadController = new List<BridgeHeadController>();
        private List<BridgeBodyController> listBridgeBodyController = new List<BridgeBodyController>();
        private List<PlatformController> listPlatformController = new List<PlatformController>();
        private List<ItemController> listItemController = new List<ItemController>();

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
        /// Get an inactive PlatformController object.
        /// </summary>
        /// <param name="platformType"></param>
        /// <param name="platformSize"></param>
        /// <returns></returns>
        public PlatformController GetPlatformController(PlatformType platformType, PlatformSize platformSize)
        {
            //Find in the list
            PlatformController platformController = listPlatformController.Where(a => !a.gameObject.activeSelf && a.PlatformType == platformType && a.PlatformSize == platformSize).FirstOrDefault();

            if (platformController == null)
            {
                //Didn't find one -> create new one
                PlatformPrefabConfiguration platformPrefabConfig = platformPrefabConfigurations.Where(a => a.PlatformType == platformType).FirstOrDefault();
                PlatformController prefab = platformPrefabConfig.PlatformControllerPrefabs.Where(a => a.PlatformSize == platformSize).FirstOrDefault();
                platformController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                platformController.gameObject.SetActive(false);
                listPlatformController.Add(platformController);
            }

            return platformController;
        }



        /// <summary>
        /// Get the finish platform based on given platformType.
        /// </summary>
        /// <param name="platformType"></param>
        /// <returns></returns>
        public PlatformController GetFinishPlatform(PlatformType platformType)
        {
            PlatformController prefab = platformPrefabConfigurations.Where(a => a.PlatformType.Equals(platformType)).FirstOrDefault().FinishPlatformControllerPrefab;
            PlatformController finishPlatform = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            finishPlatform.gameObject.SetActive(false);
            return finishPlatform;
        }



        /// <summary>
        /// Get an inactive BridgeHeadController object.
        /// </summary>
        /// <returns></returns>
        public BridgeHeadController GetBridgeHeadController()
        {
            //Find in the list
            BridgeHeadController bridgeHeadController = listBridgeHeadController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (bridgeHeadController == null)
            {
                //Didn't find one -> create new one
                bridgeHeadController = Instantiate(bridgeHeadControllerPrefab, Vector3.zero, Quaternion.identity);
                bridgeHeadController.gameObject.SetActive(false);
                listBridgeHeadController.Add(bridgeHeadController);
            }

            return bridgeHeadController;
        }




        /// <summary>
        /// Get an inactive BridgeBodyController object.
        /// </summary>
        /// <returns></returns>
        public BridgeBodyController GetBridgeBodyController()
        {
            //Find in the list
            BridgeBodyController bridgeBodyController = listBridgeBodyController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (bridgeBodyController == null)
            {
                //Didn't find one -> create new one
                bridgeBodyController = Instantiate(bridgeBodyControllerPrefab, Vector3.zero, Quaternion.identity);
                bridgeBodyController.gameObject.SetActive(false);
                listBridgeBodyController.Add(bridgeBodyController);
            }

            return bridgeBodyController;
        }



        /// <summary>
        /// Get an inactive ItemController with given type.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public ItemController GetItemController(ItemType itemType)
        {
            //Find in the list
            ItemController itemController = listItemController.Where(a => !a.gameObject.activeSelf && a.ItemType == itemType).FirstOrDefault();

            if (itemController == null)
            {
                //Did not find one -> create new one
                ItemController prefab = itemControllerPrefabs.Where(a => a.ItemType == itemType).FirstOrDefault();
                itemController = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                itemController.gameObject.SetActive(false);
                listItemController.Add(itemController);
            }

            return itemController;
        }


    }
}
