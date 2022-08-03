using System;
using System.Collections.Generic;
using Match3.Core.GameActions.TokensDamage;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEvents;
using Match3.Core.Gravity;
using Match3.Core.Matches;
using UnityEngine;
using Utils.Attributes;

namespace Match3.Core
{
    [Serializable]
    public class GameContext
    {
        [SerializeField] private SerializableEventsProvider _eventsProvider;
        
        [SerializeReference, SubclassSelector] private IBoardGravity _gravity;
        
        [SerializeField] private MatchGroups _matchGroups;

        [SerializeReference, SubclassSelector] private IDamageController _damageController;
        
        [SerializeReference, SubclassSelector] private List<IDataExtractor> _dataExtractors;
        
        [SerializeField, ToStringLabel] private List<EventTypeToResolver> _globalResolvers;

        public SerializableEventsProvider EventsProvider
        {
            get => _eventsProvider;
            set => _eventsProvider = value;
        }

        public IBoardGravity Gravity => _gravity;
        
        public MatchGroups MatchGroups => _matchGroups;

        public IDamageController DamageController => _damageController;

        public List<IDataExtractor> DataExtractors => _dataExtractors;

        public List<EventTypeToResolver> GlobalResolvers => _globalResolvers;

        public GameContext(
            SerializableEventsProvider eventsProvider,
            IBoardGravity gravity,
            MatchGroups matchGroups,
            IDamageController damageController,
            List<IDataExtractor> dataExtractors,
            List<EventTypeToResolver> globalResolvers)
        {
            _eventsProvider = eventsProvider;
            _gravity = gravity;
            _matchGroups = matchGroups;
            _damageController = damageController;
            _dataExtractors = dataExtractors;
            _globalResolvers = globalResolvers;
        }

        public GameContext() : this(
            SerializableEventsProvider.Create(),
            GravityUtils.Default,
            new MatchGroups(),
            new DefaultDamageController(),
            new List<IDataExtractor>(),
            new List<EventTypeToResolver>()
            )
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
            _dataExtractors = new List<IDataExtractor>(other._dataExtractors);
            _globalResolvers = new List<EventTypeToResolver>(other._globalResolvers);
        }
    }
}