using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using Mirror;
using UnityEngine;

namespace Networking.Platforms.MovingPlatforms
{
    public class NetworkMovingPlatform : NetworkBehaviour
    {
        #region Variables

        private float _duration;
        private List<Transform> _stations;
        private int _currentPlatform;
        private bool _loopMovement;
        public Vector3 CurrentMovementVector { get; private set; }

        #endregion

        #region Global Functions

        

        #endregion

        #region Client Functions

        

        #endregion

        #region Server Functions

        

        #endregion

        public void Initialize(in List<Transform> pStations, in float pDuration, in bool pLoopMovement)
        {
            _duration = pDuration;
            _stations = pStations;
            _loopMovement = pLoopMovement;
        }
    
        private void Update()
        {
            if (DOTween.IsTweening(transform)) return;
            
            if (++_currentPlatform >= _stations.Count)
            {
                if (!_loopMovement)
                {
                    LeanPool.Despawn(gameObject);
                    _currentPlatform = 0;
                }
                else
                    _currentPlatform = -1;
            }
            else
            {
                transform.DOMove(_stations.ElementAt(_currentPlatform).position, _duration).SetEase(Ease.Linear);
                CurrentMovementVector = (_stations.ElementAt(_currentPlatform).position - 
                                         _stations.ElementAt(_currentPlatform - 1 < 0 ? _stations.Count - 1 : _currentPlatform -1).position) / _duration;
            }
        }
    }
}