using System;
using DG.Tweening;
using UnityEngine;
using Cinemachine;
using Vesper.Core;
using Vesper.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vesper.UniverseMode
{
    public enum InteractionMarkerIconType
    {
        Look = 1,
        Talk = 2
    }
    
    public class InteractionMarker : MonoBehaviour, IBaseLoop
    {
        [SerializeField] private float _interactionDistance = 1.5f;

        [Header("Base References")]
        [SerializeField] private Transform _dotMarker;
        [SerializeField] private SpriteRenderer _dotMarkerRenderer;

        private string _connectedID;
        private ExploreCameraPawn _exploreCameraPawn;
        private Transform _interactionPosition;
        private Transform _playerCharacterTransform;
        private float _distanceSqrToPlayerCharacter;
        private InteractionMarkerIconType _closeRangeIconType;
        private InteractionMarkerVisualConfig _visualConfig;

        public string ConnectedID => _connectedID;
        public float InteractionDistance => _interactionDistance;
        public Transform InteractionPosition => _interactionPosition;

        public void Init(string connectedID, ExploreCameraPawn exploreCameraPawn, InteractionMarkerVisualConfig interactionMarkerConfig,
            Transform interactionPosition, Transform playerCharacterTransform, InteractionMarkerIconType closeRangeIconType)
        {
            name = "InteractionMarker";
            _connectedID = connectedID;
            _exploreCameraPawn = exploreCameraPawn;
            _visualConfig = interactionMarkerConfig;
            _interactionPosition = interactionPosition;
            _playerCharacterTransform = playerCharacterTransform;
            _closeRangeIconType = closeRangeIconType;
            RefreshInteractableIconVisual(false);
        }

        public void EnableVisuals()
        {
            _dotMarkerRenderer.enabled = true;
        }

        public void DisableVisuals()
        {
            _dotMarkerRenderer.enabled = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = _dotMarker.gameObject.activeSelf ? Color.green : Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, _interactionDistance);
        }
#endif

        public void ProcessUpdate()
        {
            _dotMarker.rotation = _exploreCameraPawn.FinalOrientation();
            _distanceSqrToPlayerCharacter = (_playerCharacterTransform.position - _interactionPosition.position).sqrMagnitude;

            float fadePercent = _visualConfig.FadeCurve.Evaluate(_distanceSqrToPlayerCharacter / _visualConfig.FadeOutDistanceSqr);
            _dotMarkerRenderer.color = new Color(1, 1, 1, fadePercent);

            float scalePercent = _visualConfig.SizeCurve.Evaluate(_distanceSqrToPlayerCharacter / _visualConfig.SizeCapDistanceSqr);
            float scaleValue = Mathf.Lerp(_visualConfig.MinSize, _visualConfig.MaxSize, scalePercent);
            _dotMarker.localScale = new Vector3(scaleValue, scaleValue, _dotMarker.localScale.z);
        }

        public void RefreshInteractableIconVisual(bool newState)
        {
            _dotMarkerRenderer.sprite = newState ? _visualConfig.GetInteractionTypeSprite(_closeRangeIconType) : _visualConfig.DefaultInteractionIcon;
        }
    }

}