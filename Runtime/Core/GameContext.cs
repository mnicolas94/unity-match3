using System;
using System.Collections.Generic;
using Match3.Core.GameActions.TokensDamage;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEvents;
using Match3.Core.Gravity;
using Match3.Core.Levels;
using Match3.Core.Matches;
using UnityEngine;
using Utils.Attributes;

namespace Match3.Core
{
    [Serializable]
    public class GameContext
    {
        public SerializableEventsProvider EventsProvider
        {
            get => _eventsProvider;
            set => _eventsProvider = value;
        }
        [SerializeField] private SerializableEventsProvider _eventsProvider;
        
        public IBoardGravity Gravity => _gravity;
        [SerializeReference, SubclassSelector] private IBoardGravity _gravity;
        
        public MatchGroups MatchGroups => _matchGroups;
        [SerializeField] private MatchGroups _matchGroups;

        public IDamageController DamageController => _damageController;
        [SerializeReference, SubclassSelector] private IDamageController _damageController;
        
        public List<EventTypeToResolver> GlobalResolvers => _globalResolvers;
        [SerializeField, ToStringLabel] private List<EventTypeToResolver> _globalResolvers;

        public List<ConditionalTokenCreation> TokenCreationRequests => _tokenCreationRequests;
        [SerializeField, ToStringLabel] private List<ConditionalTokenCreation> _tokenCreationRequests;


        private GameContext()
        {
        }
        
        /*
         * Copy constructor
         */
        public GameContext(GameContext other)
        {
            _eventsProvider = other._eventsProvider;
            _gravity = other._gravity;
            _matchGroups = new MatchGroups(other._matchGroups);
            _damageController = other._damageController;
            _globalResolvers = new List<EventTypeToResolver>(other._globalResolvers);
            _tokenCreationRequests = new List<ConditionalTokenCreation>(other._tokenCreationRequests);
        }

        public static GameContext GetDefault()
        {
            var context = new GameContext
            {
                _eventsProvider = SerializableEventsProvider.Create(),
                _gravity = GravityUtils.Default,
                _matchGroups = new MatchGroups(),
                _damageController = new DefaultDamageController(),
                _globalResolvers = new List<EventTypeToResolver>(),
                _tokenCreationRequests = new List<ConditionalTokenCreation>(),
            };
            return context;
        }
    }
}