using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainLogic
{
    [RequireComponent(typeof(Image))]
    public class UIAnimations : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
    {
        public float duration;

        [SerializeField] private Sprite[] frames;
        [SerializeField] private bool isButton = false;

        private bool playAnimation = true;
        private Sprite startImage;
        
        private Image imageComponent;
        private int index = 0;
        private float timer = 0;


        private void Start()
        {
            imageComponent = GetComponent<Image>();
            startImage = imageComponent.sprite;

            if (isButton)
            {
                playAnimation = false;
            }
        }

        private void Update()
        {
            if (!playAnimation) return;
            
            if ((timer += Time.deltaTime) >= (duration / frames.Length))
            {
                timer = 0;
                imageComponent.sprite = frames[index];
                index = (index + 1) % frames.Length;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isButton)
            {
                playAnimation = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isButton)
            {
                imageComponent.sprite = startImage;
                playAnimation = false;
            }
        }
    }
}