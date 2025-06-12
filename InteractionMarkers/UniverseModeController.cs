using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vesper.Core;
using Vesper.Zone;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vesper.UniverseMode
{
    public class UniverseModeController : MonoBehaviour
    {
        // NB: Unrelated code has been removed from original script for sake of
        // brevity and clarity to focus on interaction marker functionality

        private List<InteractionMarker> _interactionMarkers = new List<InteractionMarker>();
        private List<InteractionMarker> _interactionMarkersInRange = new List<InteractionMarker>();
        private InteractionMarker _closestInteractionMarker;
        private InteractionMarker _currentInteractionMarker;

        // Called as part of level instantiation
        private void CreateZoneInteractionMarkers()
        {
            foreach (CharacterBrain npc in _npcCharacters)
            {
                string desiredBlockID = Utils.ConvertIDToBlockID(npc.Data.ID);
                if (CheckIfBlockExists(desiredBlockID))
                {
                    CreateInteractionMarker(npc.Data.ID, npc.GetCharacterPawn.GetInteractionMarkerPosition, npc.GetCharacterPawn.transform,
                        npc.GetCharacterPawn.transform, InteractionMarkerIconType.Talk);
                }
            }

            foreach (LandmarkPawn landmarkZoneAnchorID in CurrentZone.Pawn.GetLandmarkIDs)
            {
                string desiredBlockID = Utils.ConvertIDToBlockID(landmarkZoneAnchorID.GetID);

                if (CheckIfBlockExists(desiredBlockID))
                {
                    CreateInteractionMarker(landmarkZoneAnchorID.GetID, landmarkZoneAnchorID.InteractionMarkerPosition, landmarkZoneAnchorID.InteractionPosition,
                        landmarkZoneAnchorID.GetTargetGameObject.transform, InteractionMarkerIconType.Look);

                    if (landmarkZoneAnchorID.SecondaryInteractionPosition != null)
                    {
                        CreateInteractionMarker(landmarkZoneAnchorID.GetID,
                            landmarkZoneAnchorID.SecondaryInteractionMarkerPosition,
                            landmarkZoneAnchorID.SecondaryInteractionPosition,
                            landmarkZoneAnchorID.GetTargetGameObject.transform, InteractionMarkerIconType.Look);
                    }
                }
            }

            foreach (ItemObjectBrain itemObject in _itemObjects)
            {
                if (CheckIfBlockExists(itemObject.Data.BlockID))
                {
                    InteractionMarker newMarker = CreateInteractionMarker(itemObject.Data.ID, itemObject.GetPawn.transform, itemObject.GetPawn.transform,
                        itemObject.GetPawn.transform, InteractionMarkerIconType.Look);

                    newMarker.transform.position = itemObject.GetPawn.InteractionMarkerPosition.position;
                }
            }

            if (_interactionMarkers.Count > 0)
            {
                _closestInteractionMarker = _interactionMarkers[0];
            }
        }

        private InteractionMarker CreateInteractionMarker(string id, Transform targetTransform, Transform interactionPosition, Transform targetParent, InteractionMarkerIconType iconType)
        {
            InteractionMarker newMarker = Instantiate(_assetsConfig.InteractionMarkerPrefab, targetTransform.position, targetTransform.rotation);
            newMarker.transform.parent = targetParent;
            newMarker.Init(id, _cameraBrain.ExploreCameraPawn, _universesConfig.GetUniverseConfig(_currentUniverse.Data.ID).InteractionMarkersConfig,
                interactionPosition, _playerCharacter.GetCharacterPawn.transform, iconType);
            _interactionMarkers.Add(newMarker);
            return newMarker;
        }

        // Called during regular update loop
        private void ProcessUpdateInteractionMarkers()
        {

            if (_interactionMarkers.Count == 0)
                return;

            float closestDistance = 300f;

            for (int i = 0; i < _interactionMarkers.Count; i++)
            {
                if (!_interactionMarkers[i].transform.parent.gameObject.activeSelf)
                {
                    continue;
                }

                float distance = (_playerCharacter.GetCharacterPawn.transform.position - _interactionMarkers[i].InteractionPosition.position).sqrMagnitude;

                float interactionDistanceSqr = _interactionMarkers[i].InteractionDistance * _interactionMarkers[i].InteractionDistance;

                if (distance < interactionDistanceSqr && !_interactionMarkersInRange.Contains(_interactionMarkers[i]))
                {
                    _interactionMarkersInRange.Add(_interactionMarkers[i]);
                }
                else if (distance > interactionDistanceSqr && _interactionMarkersInRange.Contains(_interactionMarkers[i]))
                {
                    _interactionMarkersInRange.Remove(_interactionMarkers[i]);
                    if (_currentInteractionMarker && _interactionMarkers[i].ConnectedID == _currentInteractionMarker.ConnectedID)
                    {
                        _currentInteractionMarker.RefreshInteractableIconVisual(false);
                        _currentInteractionMarker = null;
                        RefreshInteractToast(false);
                    }
                }

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestInteractionMarker = _interactionMarkers[i];
                }
            }

            float distanceToClosestMarker =
                (_playerCharacter.GetCharacterPawn.transform.position - _closestInteractionMarker.InteractionPosition.position)
                .sqrMagnitude;

            if (distanceToClosestMarker < _closestInteractionMarker.InteractionDistance *
                _closestInteractionMarker.InteractionDistance)
            {

                if (_currentInteractionMarker == null)
                {
                    _currentInteractionMarker = _closestInteractionMarker;
                    _currentInteractionMarker.RefreshInteractableIconVisual(true);
                    RefreshInteractToast(true);
                }
            }
            else
            {
                if (_currentInteractionMarker)
                {
                    _currentInteractionMarker.RefreshInteractableIconVisual(false);
                    _currentInteractionMarker = null;
                    RefreshInteractToast(false);
                }
            }

            for (int i = 0; i < _interactionMarkers.Count; i++)
            {
                _interactionMarkers[i].ProcessUpdate();
            }
        }

        // Handling for button input to switch between two markers both in interaction range
        private void InputExploreSwitchFocus(Dictionary<string, object> data)
        {
            if (_feedbackPopupOpen)
                return;

            if (_interactionMarkersInRange.Count > 1)
            {
                int currentMarkerIndex = _interactionMarkersInRange.IndexOf(_currentInteractionMarker);
                int nextMarkerIndex = currentMarkerIndex + 1 >= _interactionMarkersInRange.Count ? 0 : currentMarkerIndex + 1;

                _currentInteractionMarker.RefreshInteractableIconVisual(false);
                _currentInteractionMarker = _interactionMarkersInRange[nextMarkerIndex];
                _currentInteractionMarker.RefreshInteractableIconVisual(true);
                RefreshInteractToast(true);
            }
        }
    }
}