using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.ChatBubble
{
    [CreateAssetMenu(fileName = "New Emoji Container", menuName = "NPC Containers/New Emoji Container", order = 0)]
    public class EmojiContainer : ScriptableObject
    {
        public List<Sprite> emojis = new List<Sprite>();
    }
}