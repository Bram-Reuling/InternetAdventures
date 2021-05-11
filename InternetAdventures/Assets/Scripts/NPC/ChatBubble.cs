using System.Collections;
using System.ComponentModel;
using ScriptableObjects.ChatBubble;
using UnityEngine;
using UnityEngine.UI;

namespace NPC
{
    public class ChatBubble : MonoBehaviour
    {
        public EmojiContainer emojiContainer;

        private bool displayImages = false;
        private bool triggeredFunction = false;

        [Description("Minimal time for image change in seconds")]
        public int minimalTimeForImageChange = 5;
        
        [Description("Maximal time for image change in seconds")]
        public int maximalTimeForImageChange = 10;

        public Image imageSlot;

        private int lastImageIndex = 100000;

        public void EnableEmojis()
        {
            if (triggeredFunction) return;
            displayImages = true;
            imageSlot.enabled = true;
            StartCoroutine(EmojiCoroutine());
            triggeredFunction = true;
            //Debug.Log("Enabled Emojis");
        }

        private IEnumerator EmojiCoroutine()
        {
            while (displayImages)
            {
                //Debug.Log("YUUUP");
                SetSprite();
                yield return new WaitForSeconds(Random.Range(minimalTimeForImageChange, maximalTimeForImageChange));
            }
        }
        
        public void DisableEmojis()
        {
            if (!triggeredFunction) return;
            displayImages = false;
            StopCoroutine(EmojiCoroutine());
            triggeredFunction = false;
            imageSlot.enabled = false;
            //Debug.Log("Disabled Emojis");
        }

        private void SetSprite()
        {
            if (emojiContainer)
            {
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
        }
    }
}
