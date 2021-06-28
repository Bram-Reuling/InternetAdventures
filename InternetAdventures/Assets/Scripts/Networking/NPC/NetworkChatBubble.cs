using System.Collections;
using System.ComponentModel;
using Mirror;
using ScriptableObjects.ChatBubble;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.NPC
{
    public class NetworkChatBubble : NetworkBehaviour
    {
        #region Variables

        public EmojiContainer goodContainer;
        public EmojiContainer badContainer;

        [SerializeField, SyncVar(hook = nameof(SetImageActiveState))] private bool displayImages = false;
        private bool triggeredFunction = false;

        public int numberOfPlayersInRange = 0;

        [Description("Minimal time for image change in seconds")]
        public int minimalTimeForImageChange = 5;
        
        [Description("Maximal time for image change in seconds")]
        public int maximalTimeForImageChange = 10;

        public Image imageSlot;

        [SerializeField, SyncVar(hook = nameof(SetSprite))] private int lastImageIndex = 100000;

        [SerializeField]
        private bool IsUsingGoodEmoji = true;

        [SerializeField] private EmojiState EmojiState = EmojiState.Good;

        #endregion

        #region Global Functions

        

        #endregion

        #region Client Functions

        [ClientCallback]
        private void SetSprite(int oldSpriteIndex, int newSpriteIndex)
        {
            imageSlot.sprite = EmojiState switch
            {
                EmojiState.Good => goodContainer.emojis[newSpriteIndex],
                EmojiState.Bad => badContainer.emojis[newSpriteIndex],
                _ => imageSlot.sprite
            };
        }

        [ClientRpc]
        private void RpcChangeChatBubbleState(bool newState, EmojiState emojiState)
        {
            IsUsingGoodEmoji = newState;
            EmojiState = emojiState;
        }

        [ClientCallback]
        private void SetImageActiveState(bool oldState, bool newState)
        {
            displayImages = newState;
            imageSlot.enabled = newState;
        }

        #endregion

        #region Server Functions

        [ServerCallback]
        public void ChangeToBad()
        {
            Debug.Log("Change to bad");
            IsUsingGoodEmoji = false;
            EmojiState = EmojiState.Bad;
            RpcChangeChatBubbleState(false, EmojiState.Bad);
        }
        
        [ServerCallback]
        public void EnableEmojis()
        {
            numberOfPlayersInRange++;
            if (triggeredFunction) return;
            displayImages = true;
            imageSlot.enabled = true;
            StartCoroutine(EmojiCoroutine());
            triggeredFunction = true;
            //Debug.Log("Enabled Emojis");
        }
        
        [ServerCallback]
        public void DisableEmojis()
        {
            numberOfPlayersInRange--;
            if (!triggeredFunction) return;

            if (numberOfPlayersInRange != 0) return;
            
            displayImages = false;
            StopCoroutine(EmojiCoroutine());
            triggeredFunction = false;
            imageSlot.enabled = false;
            //Debug.Log("Disabled Emojis");   
        }

        [ServerCallback]
        private IEnumerator EmojiCoroutine()
        {
            while (displayImages)
            {
                //Debug.Log("YUUUP");
                SetSprite();
                yield return new WaitForSeconds(Random.Range(minimalTimeForImageChange, maximalTimeForImageChange));
            }
        }
        
        [ServerCallback]
        private void SetSprite()
        {
            if (!goodContainer || !badContainer) return;

            int pictureIndex = 0;
            
            if (EmojiState == EmojiState.Good)
            {
                pictureIndex = Random.Range(0, goodContainer.emojis.Count);   
            }
            else if (EmojiState == EmojiState.Bad)
            {
                pictureIndex = Random.Range(0, badContainer.emojis.Count);   
            }

            if (pictureIndex == lastImageIndex)
            {
                //Debug.Log("Same as last");
                SetSprite();
            }
            else
            {
                //Debug.Log("Unique");
                lastImageIndex = pictureIndex;
                if (EmojiState == EmojiState.Good)
                {
                    imageSlot.sprite = goodContainer.emojis[pictureIndex];     
                }
                else if (EmojiState == EmojiState.Bad)
                {
                    imageSlot.sprite = badContainer.emojis[pictureIndex];     
                }
            }
        }
        
        #endregion
    }

    public enum EmojiState
    {
        Good,
        Bad
    }
}