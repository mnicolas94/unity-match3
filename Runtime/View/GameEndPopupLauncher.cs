using System;
using System.Threading;
using AsyncUtils;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Match3.View
{
    public class GameEndPopupLauncher : MonoBehaviour, IAtomListener<bool>
    {
        [SerializeField] private GameControllerView _gameControllerView;
        [SerializeField] private BoolEvent _gameEndEvent;
        [SerializeField] private PopupGameEnded _victoryPopupPrefab;
        [SerializeField] private PopupGameEnded _defeatPopupPrefab;

        private CancellationTokenSource _cts;
        
        private void Start()
        {
            _cts = new CancellationTokenSource();
            _gameEndEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            _gameEndEvent.UnregisterListener(this);
            _cts.Cancel();
            _cts.Dispose();
        }

        public void OnEventRaised(bool victory)
        {
            var ct = _cts.Token;
            var controller = _gameControllerView.GameController;
            var popup = victory ? _victoryPopupPrefab : _defeatPopupPrefab;
            Popups.ShowPopup(popup, (controller, victory), ct);
        }
    }
}