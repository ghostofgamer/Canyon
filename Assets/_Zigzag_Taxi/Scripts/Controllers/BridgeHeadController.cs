using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClawbearGames
{
    public class BridgeHeadController : MonoBehaviour
    {
        [SerializeField] private Text lengthText = null;

        private void Update()
        {
            if (PlayerController.Instance.PlayerState == PlayerState.Player_Living)
            {
                //Check and disable this bridge head
                float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
                if (distance >= 25f)
                {
                    OnDeactive();
                }
            }
        }


        /// <summary>
        /// Deactive this bridge head.
        /// </summary>
        public void OnDeactive()
        {
            //Deactive all the bridge bodies
            BridgeBodyController[] bridgeBodies = GetComponentsInChildren<BridgeBodyController>();
            foreach (BridgeBodyController bridgeBody in bridgeBodies)
            {
                bridgeBody.OnDeactive();
            }

            //Disable this bridge head
            transform.SetParent(null);
            transform.eulerAngles = Vector3.zero;
            gameObject.SetActive(false);
        }



        /// <summary>
        /// Update the length of the 
        /// </summary>
        /// <param name="bodyAmount"></param>
        public void UpdateLength(int bodyAmount)
        {
            float currentLength = (float)System.Math.Round((bodyAmount + 1) * 0.25f, 2);
            lengthText.text = currentLength.ToString() + "M";
        }
    }
}
