using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainLogic
{
    [RequireComponent(typeof(Image))]
    public class UIAnimations : MonoBehaviour
    {
        public float duration;

        [SerializeField] private Sprite[] frames;

        private Image imageComponent;
        private int index = 0;
        private float timer = 0;


        private void Start()
        {
            imageComponent = GetComponent<Image>();
        }

        private void Update()
        {
            if ((timer += Time.deltaTime) >= (duration / frames.Length))
            {
                timer = 0;
                imageComponent.sprite = frames[index];
                index = (index + 1) % frames.Length;
            }
        }
    }
}