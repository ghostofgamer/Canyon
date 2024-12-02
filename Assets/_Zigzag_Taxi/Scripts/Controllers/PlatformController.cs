using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

namespace ClawbearGames
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private PlatformType platformType = PlatformType.GREEN;
        [SerializeField] private PlatformSize platformSize = PlatformSize.HUGE;
        [SerializeField] private CenterController centerController = null;
        [SerializeField] private MeshRenderer meshRenderer = null;
        private int index;
        public CenterController CenterController => centerController;
        public PlatformType PlatformType => platformType;
        public PlatformSize PlatformSize => platformSize;
        public float ZSize => meshRenderer.bounds.extents.z;
        public bool IsFinishPlatform { private set; get; }


        /// <summary>
        /// Determine this platform is created a bridge or not.
        /// </summary>
        public bool IsCreatedBridge => bridgeHead != null;


        /// <summary>
        /// Determine this platform is ready to build a bridge.
        /// </summary>
        public bool IsReady { private set; get; }


        private List<BridgeBodyController> listBridgeBody = new List<BridgeBodyController>();
        private BridgeHeadController bridgeHead = null;
        private float timeCount = 0;
        private int coinItem = 0;


        /// <summary>
        /// Setup this platform.
        /// </summary>
        /// <param name="coinItemAmount"></param>
        /// <param name="finishPlatform"></param>
        public void OnSetup(int coinItemAmount, bool finishPlatform = false)
        {
            IsReady = false;
            IsFinishPlatform = finishPlatform;
            coinItem = coinItemAmount;
            timeCount = 0;
            bridgeHead = null;
            listBridgeBody.Clear();
            transform.position += Vector3.down * 30f;
            StartCoroutine(CRMoveUp());
        }



        /// <summary>
        /// Reset this platform after player revived.
        /// </summary>
        public void OnReset()
        {
            IsReady = true;
            timeCount = 0;
            if (IsCreatedBridge)
            {
                bridgeHead.OnDeactive();
                bridgeHead = null;
                listBridgeBody.Clear();
            }
        }



        /// <summary>
        /// Coroutine move this platform up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRMoveUp()
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(startPos.x, 0f, startPos.z);
            float moveTime = 0.5f;
            float t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutCubic, t / moveTime);
                transform.position = Vector3.Lerp(startPos, endPos, factor);
                yield return null;
            }

            //Ready to build the bridge
            IsReady = true;
        }


        /// <summary>
        /// Create the bridge.
        /// </summary>
        public void CreateBridge()
        {
            bridgeHead = PoolManager.Instance.GetBridgeHeadController();
            bridgeHead.transform.position = transform.position + transform.forward * meshRenderer.bounds.extents.z;
            bridgeHead.transform.eulerAngles = (transform.forward == Vector3.forward) ? new Vector3(270f, 0f, 0f) : new Vector3(270f, 270f, 0f);
            bridgeHead.gameObject.SetActive(true);
            bridgeHead.UpdateLength(listBridgeBody.Count);
        }


        /// <summary>
        /// Build the bridge.
        /// </summary>
        public void BuildBridge()
        {
            if (listBridgeBody.Count >= 51)
                return;
            
            timeCount += Time.deltaTime;
            if (timeCount > 1 / (float)IngameManager.Instance.BridgeBuildingSpeed)
            {
                ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.BuiltBridge);
                timeCount = 0;
                BridgeBodyController bridgeBody = PoolManager.Instance.GetBridgeBodyController();
                bridgeBody.transform.SetParent(bridgeHead.transform);
                bridgeBody.transform.localEulerAngles = Vector3.zero;
                bridgeBody.transform.localPosition = new Vector3(0f, 0f, 0.25f * (listBridgeBody.Count + 1));
                bridgeBody.gameObject.SetActive(true);
                listBridgeBody.Add(bridgeBody);
                bridgeHead.UpdateLength(listBridgeBody.Count);
            }
            
            index++;
            // Debug.Log("Create True " + index);
        }



        /// <summary>
        /// Place the bridge.
        /// </summary>
        public void PlaceBridge()
        {
            IsReady = false;
            StartCoroutine(CRRotateTheBridge());
        }


        /// <summary>
        /// Coroutine rotate the bridge.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRRotateTheBridge()
        {
            float t = 0;
            float rotateTime = 0.35f;
            Quaternion startQuaternion = bridgeHead.transform.rotation;
            Quaternion endQuaternion = transform.forward.Equals(Vector3.forward) ? Quaternion.Euler(new Vector3(250f, 0f, 0f)) : Quaternion.Euler(new Vector3(250f, 270f, 0f));
            while (t < rotateTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutCubic, t / rotateTime);
                bridgeHead.transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, factor);
                yield return null;
            }

           

            t = 0;
            rotateTime = 0.25f;
            startQuaternion = bridgeHead.transform.rotation;
            endQuaternion = transform.forward.Equals(Vector3.forward) ? Quaternion.Euler(new Vector3(0f, 0f, 0f)) : Quaternion.Euler(new Vector3(0f, 270f, 0f));
            while (t < rotateTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseInQuint, t / rotateTime);
                bridgeHead.transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, factor);
                yield return null;
            }

            Vector3 finalPosition = bridgeHead.transform.position;
            finalPosition.y += 0.01f; 
            bridgeHead.transform.position = finalPosition;


            if (IngameManager.Instance.IngameState != IngameState.Ingame_Playing)
                yield break;

            //Calculate the distance between this platforn and the next platform
            float platformDis = Vector3.Distance(transform.position, IngameManager.Instance.NextPlatform.transform.position);
            float minTargetDis = (float)System.Math.Round(platformDis - meshRenderer.bounds.extents.z - IngameManager.Instance.NextPlatform.ZSize, 2);
            float maxTargetDis = (float)System.Math.Round(platformDis - meshRenderer.bounds.extents.z + IngameManager.Instance.NextPlatform.ZSize, 2);

            //Calculate the distance of the bridge
            float bridgeDis = (float)System.Math.Round((listBridgeBody.Count + 1) * 0.25f, 2);

            if (bridgeDis < minTargetDis || bridgeDis > maxTargetDis) //The bridge did not place at the next platform
            {
                CameraRootController.Instance.FollowPlayer(false);
                PlayerController.Instance.MoveToEndOfBridge(listBridgeBody[listBridgeBody.Count - 1].transform.position);
                if (bridgeDis < minTargetDis && bridgeDis < maxTargetDis)
                {
                    float waitTime = Vector3.Distance(PlayerController.Instance.transform.position, listBridgeBody[listBridgeBody.Count - 1].transform.position) / PlayerController.Instance.MovementSpeed;
                    yield return new WaitForSeconds(waitTime);

                    //Rotate down the bridge
                    t = 0;
                    rotateTime = 0.5f;
                    startQuaternion = transform.rotation;
                    endQuaternion = (transform.forward.Equals(Vector3.forward)) ? Quaternion.Euler(90f, 0f, 0f) : Quaternion.Euler(90f, -90f, 0f);
                    while (t < rotateTime)
                    {
                        t += Time.deltaTime;
                        float factor = EasyType.MatchedLerpType(LerpType.EaseOutCubic, t / rotateTime);
                        bridgeHead.transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, factor);
                        yield return null;
                    }
                }
            }
            else //The bridge is placed at the next platform
            {            
                //Check if the bridge is placed at center of the next ground
                Vector3 checkPos = listBridgeBody[listBridgeBody.Count - 1].transform.position + transform.forward * 0.25f;
                if (IngameManager.Instance.NextPlatform.CenterController.IsInside(checkPos))
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlacedCenter);

                    //Fade out the center and create multiple coin items
                    IngameManager.Instance.NextPlatform.CenterController.ScaleAndFadeOut();

                    //Create coin items
                    List<int> listItemIndex = new List<int>();
                    int itemAmount = (coinItem > listBridgeBody.Count) ? listBridgeBody.Count : coinItem;
                    while (itemAmount > 0)
                    {
                        int itemIndex = Random.Range(0, listBridgeBody.Count);
                        while (listItemIndex.Contains(itemIndex))
                        {
                            itemIndex = Random.Range(0, listBridgeBody.Count);
                        }
                        listItemIndex.Add(itemIndex);

                        
                        
                        
                        
                        
                        
                        

                        ItemController itemController = PoolManager.Instance.GetItemController(ItemType.COIN);
                        itemController.transform.position = listBridgeBody[itemIndex].CenterPosition;
                        itemController.gameObject.SetActive(true);
                        itemController.OnSetup();
                        itemAmount--;
                    }
                }
                else //Create only one coin item
                {
                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlacedBridge);
                    EffectManager.Instance.CreateSquareFadingEffect();
                    ItemController itemController = PoolManager.Instance.GetItemController(ItemType.COIN);
                    itemController.transform.position = listBridgeBody[Mathf.RoundToInt(listBridgeBody.Count / 2f)].CenterPosition;
                    itemController.gameObject.SetActive(true);
                    itemController.OnSetup();
                }


                //Remove the extra bridge parts
                float extraDis = (float)System.Math.Round(bridgeDis - minTargetDis, 2);
                while (extraDis > 0)
                {
                    EffectManager.Instance.CreateBridgeFaderEffect(listBridgeBody[listBridgeBody.Count - 1].transform.position, listBridgeBody[listBridgeBody.Count - 1].transform.forward);
                    listBridgeBody[listBridgeBody.Count - 1].OnDeactive();
                    listBridgeBody.RemoveAt(listBridgeBody.Count - 1);
                    yield return new WaitForSeconds(0.05f);
                    extraDis = (float)System.Math.Round(extraDis - 0.25f, 2);
                    bridgeHead.UpdateLength(listBridgeBody.Count);
                }
                listBridgeBody[listBridgeBody.Count - 1].ChangeToTail();


                //Move the player to next platform
                PlayerController.Instance.MoveToNextPlatform();
            }


            //Check and disable this platform
            while (gameObject.activeSelf)
            {
                if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
                {
                    float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
                    if (distance >= 25f)
                    {
                        timeCount = 0;
                        bridgeHead = null;
                        listBridgeBody.Clear();

                        //Disable this platform
                        gameObject.SetActive(false);
                    }
                }
                yield return null;
            }

    
        }
    }
}
