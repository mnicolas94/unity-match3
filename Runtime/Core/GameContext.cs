using System;
using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEvents;
using Match3.Core.Gravity;
using Match3.Core.Matches;
using UnityEngine;

namespace Match3.Core
{
    [Serializable]
    public class GameContext
    {
        [SerializeField] private SerializableEventsProvider _eventsProvider;
        
        [SerializeReference, SubclassSelector] private List<IDataExtractor> _dataExtractors;
        
        [SerializeField] private MatchGroups _matchGroups;

        [SerializeReference, SubclassSelector] private IBoardGravity _gravity;

        public SerializableEventsProvider EventsProvider
        {
            get => _eventsProvider;
            set => _eventsProvider = value;
        }

        public List<IDataExtractor> DataExtractors => _dataExtractors;

        public MatchGroups MatchGroups => _matchGroups;

        public IBoardGravity Gravity => _gravity;

        public GameContext(
            SerializableEventsProvider eventsProvider,
            List<IDataExtractor> dataExtractors,
            MatchGroups matchGroups,
            IBoardGravity gravity)
        {
            _eventsProvider = eventsProvider;
            _dataExtractors = dataExtractors;
            _matchGroups = matchGroups;
            _gravity = gravity;
        }

        public GameContext() : this(
            SerializableEventsProvider.Create(),
            new List<IDataExtractor>(),
            new MatchGroups(),
            GravityUtils.Default
            )
        {
        }

        /*
         * Copy constructor
         */
        public GameContext(GameContext other)
        {
            _eventsProvider = other._eventsProvider;
            _dataExtractors = new List<IDataExtractor>(other._dataExtractors);
            _matchGroups = new MatchGroups(other._matchGroups);
            _gravity = other._gravity;
        }
    }
}