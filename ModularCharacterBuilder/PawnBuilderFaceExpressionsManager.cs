using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vesper.CharacterPawnBuilder;
using Vesper.Core;

namespace Vesper.Workshop.FaceExpressions
{
    public class PawnBuilderFaceExpressionsManager : MonoBehaviour
    {
        [System.Serializable]
        public enum FaceExpression
        {
            None = 0,
            Shock = 1,
            Happy = 2,
            Scared = 3,
            Sad = 4,
            Angry = 5,
            Confused = 6,
            Blink = 7,
            Laugh = 8
        }

        private float[] _noneTargets = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private float[] _shockTargets = new float[] {100f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private float[] _happyTargets = new float[] { 0f, 100f, 0f, 0f, 0f, 0f, 0f, 0f };
        private float[] _scaredTargets = new float[]{0f, 0f, 100f, 0f, 0f, 0f, 0f, 0f };
        private float[] _sadTargets = new float[] {0f, 0f, 0f, 100f, 0f, 0f, 0f, 0f };
        private float[] _angryTargets = new float[] { 0f, 0f, 0f, 0f, 100f, 0f, 0f, 0f };
        private float[] _confusedTargets = new float[] { 0f, 0f, 0f, 0f, 0f, 100f, 0f, 0f };
        private float[] _blinkTargets = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 100f, 0f };
        private float[] _laughTargets = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 100f };

        [SerializeField] private float _transitionDuration;

        [Header("Scene References")]
        [SerializeField] private CharacterPawnBuilderController _pawnBuilder;
        [SerializeField] private Transform _camera;
        [SerializeField] private Transform _pawn;
        [SerializeField] private Transform _nearTransform;
        [SerializeField] private Transform _farTransform;
        [SerializeField] private Slider _zoomSlider;
        [SerializeField] private Slider _rotateSlider;

        [Header("Face BlendShape Buttons")]
        [SerializeField] private Button _noneButton;
        [SerializeField] private Button _shockButton;
        [SerializeField] private Button _happyButton;
        [SerializeField] private Button _scaredButton;
        [SerializeField] private Button _sadButton;
        [SerializeField] private Button _angryButton;
        [SerializeField] private Button _confusedButton;
        [SerializeField] private Button _blinkButton;
        [SerializeField] private Button _laughButton;

        private SkinnedMeshRenderer[] _meshTargets;
        private FaceExpression _currentExpression;

        private void Start()
        {
            SetNewExpression(FaceExpression.None);

            _noneButton.onClick.AddListener(() => SetNewExpression(FaceExpression.None));
            _shockButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Shock));
            _happyButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Happy));
            _scaredButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Scared));
            _sadButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Sad));
            _angryButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Angry));
            _confusedButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Confused));
            _blinkButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Blink));
            _laughButton.onClick.AddListener(() => SetNewExpression(FaceExpression.Laugh));

            _zoomSlider.onValueChanged.AddListener(InputZoom);
            _rotateSlider.onValueChanged.AddListener(InputRotation);
        }

        private void OnDestroy()
        {
            _noneButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.None));
            _shockButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Shock));
            _happyButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Happy));
            _scaredButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Scared));
            _sadButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Sad));
            _angryButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Angry));
            _confusedButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Confused));
            _blinkButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Blink));
            _laughButton.onClick.RemoveListener(() => SetNewExpression(FaceExpression.Laugh));
        }

        public void SetNewExpression(FaceExpression newExpression)
        {
            _meshTargets = _pawnBuilder.CurrentHeadMeshTargets;
            _currentExpression = newExpression;
            switch (_currentExpression)
            {
                case FaceExpression.None:
                    RefreshBlendShapes(_noneTargets);
                    break;
                case FaceExpression.Shock:
                    RefreshBlendShapes(_shockTargets);
                    break;
                case FaceExpression.Happy:
                    RefreshBlendShapes(_happyTargets);
                    break;
                case FaceExpression.Scared:
                    RefreshBlendShapes(_scaredTargets);
                    break;
                case FaceExpression.Sad:
                    RefreshBlendShapes(_sadTargets);
                    break;
                case FaceExpression.Angry:
                    RefreshBlendShapes(_angryTargets);
                    break;
                case FaceExpression.Confused:
                    RefreshBlendShapes(_confusedTargets);
                    break;
                case FaceExpression.Blink:
                    RefreshBlendShapes(_blinkTargets);
                    break;
                case FaceExpression.Laugh:
                    RefreshBlendShapes(_laughTargets);
                    break;
            }
        }

        private void RefreshBlendShapes(float[] targets)
        {
            for (int i = 0; i < _meshTargets.Length; i++)
            {
                for (int b = 0; b < _meshTargets[i].sharedMesh.blendShapeCount; b++)
                {
                    if (b <= targets.Length - 1)
                    {
                        BeginTransitionTween(_meshTargets[i], b, targets[b]);
                    }
                }
            }
        }

        private void BeginTransitionTween(SkinnedMeshRenderer mesh, int index, float target)
        {
            float currentBlendValue = mesh.GetBlendShapeWeight(index);

            DOTween.To(() => currentBlendValue, x => currentBlendValue = x, target, _transitionDuration).OnUpdate(() => mesh.SetBlendShapeWeight(index, currentBlendValue));
        }

        public void InputZoom(float value)
        {
            _camera.position = Vector3.Lerp(_farTransform.position, _nearTransform.position, value);
            _camera.rotation = Quaternion.Lerp(_farTransform.rotation, _nearTransform.rotation, value);
        }

        public void InputRotation(float value)
        {
            float offset = Mathf.Clamp01(value);
            float targetYRotation = Mathf.Lerp(180, -180, offset);
            _pawn.rotation = Quaternion.Euler(0f, targetYRotation, 0f);
        }
    }

}