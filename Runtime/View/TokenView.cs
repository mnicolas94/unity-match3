using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Match3.Core;
using UnityEngine;
using Utils;
using Utils.Tweening;

namespace Match3.View
{
    public class TokenView : MonoBehaviour
    {
        public readonly int DestroyHash = Animator.StringToHash("destroy");
        
        public Action onDestroyed;
        
        [SerializeField, HideInInspector] private Token _token;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private AudioSource _audioSource;
        
        public Token Token => _token;

        public void Initialize(Token token, int layerIndex)
        {
            name = token.TokenData.ToString();
            _token = token;
            _spriteRenderer.sprite = token.TokenData.TokenSprite;
            SetSortingLayerAndOrder(token, layerIndex);
        }
        
        public void Destroy()
        {
            var clip = _token.TokenData.DestructionSound;
            if (clip != null)
                _audioSource.PlayOneShot(clip);
            var coroutine = CoroutineUtils.CoroutineSequence(new List<IEnumerator>
            {
                WaitForAnimationCoroutine(),
                CoroutineUtils.ActionCoroutine(() => onDestroyed?.Invoke())
            });
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitForAnimationCoroutine()
        {
            InstantiateDestroyParticles();
            var sprites = _token.TokenData.DestroySprites;
            if (sprites.Count > 0)
            {
                var duration = _token.TokenData.DestroyAnimDuration;
                return _spriteRenderer.TweenSpritesCoroutine(sprites, duration, Curves.Linear);
            }

            return null;
        }

        private void InstantiateDestroyParticles()
        {
            var prefab = _token.TokenData.ParticlesPrefab;
            if (prefab != null)
            {
                var t = transform;
                var containsPoolableParticles = prefab.TryGetComponent<PoolableParticles>(out _);
                if (containsPoolableParticles)
                {
                    var spawned = LeanPool.Spawn(prefab, t.position, Quaternion.identity);
                    var particles = spawned.GetComponent<PoolableParticles>();
                    particles.OnParticleSystemStoppedCallback.RemoveAllListeners();
                    particles.OnParticleSystemStoppedCallback.AddListener(() =>
                    {
                        LeanPool.Despawn(particles);
                    });
                }
                else
                {
                    Instantiate(prefab, t.position, Quaternion.identity);
                }
            }
        }

        private void SetSortingLayerAndOrder(Token token, int layerIndex)
        {
            var tokenData = token.TokenData;
            int orderSign = 1;
            
            if (tokenData.Type != TokenType.Main)
                layerIndex++;
            
            if (tokenData.Type == TokenType.Bottom)
                orderSign = -1;
            _spriteRenderer.sortingOrder = layerIndex * orderSign;
        }
    }
}