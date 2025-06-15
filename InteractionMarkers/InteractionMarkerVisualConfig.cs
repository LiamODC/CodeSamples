using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vesper.UniverseMode;

namespace Vesper.UI
{
    [CreateAssetMenu(menuName = "VESPER/UI/Interaction Marker Visual Config")]
    public class InteractionMarkerVisualConfig : ScriptableObject
    {
        [Header("Interaction Type Sprites")]
        [SerializeField] Sprite _lookInteractionIcon;
        [SerializeField] Sprite _talkInteractionIcon;
        [SerializeField] Sprite _defaultInteractionIcon;

        [Header("Tweakable Values")]
        [SerializeField] AnimationCurve _sizeCurve;
        [SerializeField] AnimationCurve _fadeCurve;
        [SerializeField] float _iconSwapDistance;
        [SerializeField] float _sizeCapDistance;
        [SerializeField] float _fadeOutDistance;
        [SerializeField] float _minSize;
        [SerializeField] float _maxSize;

        public Sprite DefaultInteractionIcon => _defaultInteractionIcon;
        public AnimationCurve SizeCurve => _sizeCurve;
        public AnimationCurve FadeCurve => _fadeCurve;
        public float IconSwapDistanceSqr => _iconSwapDistance * _iconSwapDistance;
        public float SizeCapDistanceSqr => _sizeCapDistance * _sizeCapDistance;
        public float FadeOutDistanceSqr => _fadeOutDistance * _fadeOutDistance;
        public float MinSize => _minSize;
        public float MaxSize => _maxSize;

        public Sprite GetInteractionTypeSprite(InteractionMarkerIconType iconType)
        {
            switch (iconType)
            {
                case InteractionMarkerIconType.Look:
                    return _lookInteractionIcon;
                case InteractionMarkerIconType.Talk:
                    return _talkInteractionIcon;
                default:
                    return _lookInteractionIcon;
            }
        }
    }
}