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

        public EmojiContainer emojiContainer;

        [SerializeField, SyncVar(hook = nameof(SetImageActiveState))] private bool displayImages = false;
        private bool triggeredFunction = false;

        public int numberOfPlayersInRange = 0;

        [Description("Minimal time for image change in seconds")]
        public int minimalTimeForImageChange = 5;
        
        [Description("Maximal time for image change in seconds")]
        public int maximalTimeForImageChange = 10;

        public Image imageSlot;

        [SerializeField, SyncVar(hook = nameof(SetSprite))] private int lastImageIndex = 100000;

        #endregion

        #region Global Functions

        

        #endregion

        #region Client Functions

        [ClientCallback]
        private void SetSprite(int oldSpriteIndex, int newSpriteIndex)
        {
            imageSlot.sprite = emojiContainer.emojis[newSpriteIndex];  
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
            if (!emojiContainer) return;
            
            int pictureIndex = Random.Range(0, emojiContainer.emojis.Count);

            if (pictureIndex == lastImageIndex)
            {
                //Debug.Log("Same as last");
                SetSprite();
            }
            else
            {
                //Debug.Log("Unique");
                lastImageIndex = pictureIndex;
                imageSlot.sprite = emojiContainer.emojis[pictureIndex];   
            }
        }
        
        #endregion
    }
}