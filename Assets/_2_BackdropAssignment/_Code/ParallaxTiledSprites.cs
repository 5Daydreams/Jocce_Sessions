using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FG
{
    public class ParallaxTiledSprites : MonoBehaviour
    {
        [SerializeField] private string _shaderOffsetVariableName;
        [SerializeField] private float _scrollSpeed;
        
        private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        private Vector2 _offsetCache = new Vector2();

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child != null)
                {
                    _renderers.Add(child.GetComponent<SpriteRenderer>());
                }
            }
            SetParallaxMovementSpeed(Vector2.zero);
        }
        
        public void SetParallaxMovementSpeed(Vector2 direction)
        {
            _offsetCache += Vector2.right * _scrollSpeed * direction.x * Time.deltaTime;
            foreach (var spriteRenderer in _renderers)
            {
                spriteRenderer.sharedMaterial.SetVector(_shaderOffsetVariableName, _offsetCache);
            }
        }
    }
}