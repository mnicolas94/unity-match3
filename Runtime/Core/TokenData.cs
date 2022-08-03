using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Resolvers;
using NaughtyAttributes;
using UnityEngine;
using Utils.Attributes;
using Utils.Serializables;

namespace Match3.Core
{
    public enum TokenType
    {
        Bottom,
        Main,
        Top
    }
    
    public class TokenData : ScriptableObject
    {
        [SerializeField, BoxGroup("Appearance"), ShowAssetPreview, Required]
        private Sprite _tokenSprite;
        [SerializeField, BoxGroup("Appearance"), ShowAssetPreview]
        private List<Sprite> _healthSprites;
        [SerializeField, BoxGroup("Appearance"), ShowAssetPreview]
        private List<Sprite> _destroySprites;
        [SerializeField, BoxGroup("Appearance")]
        private float _destroyAnimDuration;
        [SerializeField, BoxGroup("Appearance")]
        private GameObject _particlesPrefab;
        [SerializeField, BoxGroup("Appearance")]
        private AudioClip _destructionSound;

        [SerializeField, Min(1), BoxGroup("Behaviour")] private int _initialHealth;
        [SerializeField, BoxGroup("Behaviour")] private TokenType _type;
        [SerializeField, BoxGroup("Behaviour")] private bool _canMove;
        [SerializeField, BoxGroup("Behaviour")] private bool _canMatchWithItself;
        [SerializeField, BoxGroup("Behaviour")] private bool _isIndestructible;
        [SerializeField, ToStringLabel, BoxGroup("Behaviour")] private List<EventTypeToResolver> _resolvers;

        public Sprite TokenSprite
        {
            get
            {
                int spritesCount = -_healthSprites.Count;
                
                return spritesCount > 0 ? _healthSprites[spritesCount - 1] : _tokenSprite;
            }
#if UNITY_EDITOR
            set => _tokenSprite = value;
#endif
        }

        public List<Sprite> HealthSprites => _healthSprites;

        public List<Sprite> DestroySprites => _destroySprites;

        public float DestroyAnimDuration => _destroyAnimDuration;

        public GameObject ParticlesPrefab => _particlesPrefab;

        public AudioClip DestructionSound => _destructionSound;


        public int InitialHealth => _initialHealth;

        public TokenType Type
        {
            get => _type;
#if UNITY_EDITOR
            set => _type = value;
#endif
        }

        public bool CanMove
        {
            get => _canMove;
#if UNITY_EDITOR
            set => _canMove = value;
#endif
        }

        public bool CanMatchWithItself
        {
            get => _canMatchWithItself;
#if UNITY_EDITOR
            set => _canMatchWithItself = value;
#endif
        }

        public bool IsIndestructible
        {
            get => _isIndestructible;
#if UNITY_EDITOR
            set => _isIndestructible = value;
#endif
        }

        public ReadOnlyCollection<EventTypeToResolver> Resolvers => _resolvers.AsReadOnly();

#if UNITY_EDITOR
        public void AddResolver<TEvent>(IEventResolver resolver)
        {
            var eventResolver = EventTypeToResolver.Create<TEvent>(resolver);
            _resolvers.Add(eventResolver);
        }
#endif

        public bool ApplyDamage(Token token, Vector2Int position)
        {
            return !_isIndestructible;
        }
        
        public override string ToString()
        {
            return name;
        }
    }
    
    [Serializable]
    public class EventTypeToResolver
    {
        [SerializeField] private TypeReference<TokenEventInput> eventType;
        [SerializeReference, SubclassSelector] private IEventResolver resolver;

        public TypeReference<TokenEventInput> EventType => eventType;

        public IEventResolver Resolver => resolver;

        public EventTypeToResolver(TypeReference<TokenEventInput> eventType, IEventResolver resolver)
        {
            this.eventType = eventType;
            this.resolver = resolver;
        }

        public static EventTypeToResolver Create<T>(IEventResolver resolver)
        {
            var eventType = new TypeReference<TokenEventInput>();
            eventType.Type = typeof(T);
            return new EventTypeToResolver(eventType, resolver);
        }

        public override string ToString()
        {
            return $"{EventType.Type.Name} -> {Resolver.GetType().Name}";
        }
    }

    public static class EventTypeToResolverListExtensions
    {
        public static bool HasResolvers<T>(this IList<EventTypeToResolver> resolvers) where T : TokenEventInput
        {
            var count = resolvers.Count(typeResolver => typeof(T).IsAssignableFrom(typeResolver.EventType));
            return count > 0;
        }
        
        public static IEnumerable<IEventResolver> GetResolvers<T>(this IList<EventTypeToResolver> resolvers)
            where T : TokenEventInput
        {
            var filtered = resolvers
                .Where(typeResolver => typeof(T).IsAssignableFrom(typeResolver.EventType))
                .Select(typeResolver => typeResolver.Resolver);
            
            return filtered;
        }
    }
}