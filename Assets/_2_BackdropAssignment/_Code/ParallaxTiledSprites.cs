using System;
using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    public class ParallaxTiledSprites : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> _renderers;
        [SerializeField] private float _scrollSpeed;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child != null)
                {
                    _renderers.Add(child.GetComponent<SpriteRenderer>());
                }
            }
            SetParallaxMovementSpeed();
        }

        private void Update()
        {
            SetParallaxMovementSpeed();
        }

        private void SetParallaxMovementSpeed()
        {
            foreach (var spriteRenderer in _renderers)
            {
                spriteRenderer.sharedMaterial.SetFloat("_ParallaxSpeed", _scrollSpeed);
            }
        }
    }
}