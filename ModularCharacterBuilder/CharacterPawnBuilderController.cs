using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vesper.Graphics;
using Vesper.UI;

namespace Vesper.CharacterPawnBuilder
{
    public enum CharacterPawnBodyParts
    {
        Hair,
        Head,
        Ears,
        Torso,
        Hands,
        Legs,
        Feet,
        Back
    }

    [ExecuteAlways]
    public class CharacterPawnBuilderController : MonoBehaviour
    {
        [SerializeField] private string _savePath = "Assets/_WORKSHOPS/CharacterPawnBuilder/Pawns/";

        [Header("Body Parts")]
        [SerializeField] private GameObject[] _torsos;
        [SerializeField] private GameObject[] _legs;
        [SerializeField] private GameObject[] _feet;
        [SerializeField] private GameObject[] _hands;
        [SerializeField] private GameObject[] _heads;
        [SerializeField] private GameObject[] _hair;
        [SerializeField] private GameObject[] _ears;
        [SerializeField] private GameObject[] _backs;

        [Header("Character References")]
        [SerializeField] private Transform _targetPawn;
        [SerializeField] private MeshColorPaletteBehaviour _torsoParent;
        [SerializeField] private MeshColorPaletteBehaviour _legsParent;
        [SerializeField] private MeshColorPaletteBehaviour _feetParent;
        [SerializeField] private MeshColorPaletteBehaviour _handsParent;
        [SerializeField] private Transform _headParent;
        [SerializeField] private Transform _hairParent;
        [SerializeField] private Transform _earsParent;
        [SerializeField] private Transform _backParent;
        [SerializeField] private GameObject _currentHead;
        [SerializeField] private GameObject _currentHair;
        [SerializeField] private GameObject _currentEars;
        [SerializeField] private GameObject _currentBack;

        [Header("Body Part Buttons")]
        [SerializeField] private Button _previousHairButton;
        [SerializeField] private Button _nextHairButton;
        [SerializeField] private Button _previousHeadButton;
        [SerializeField] private Button _nextHeadButton;
        [SerializeField] private Button _previousEarsButton;
        [SerializeField] private Button _nextEarsButton;
        [SerializeField] private Button _previousTorsoButton;
        [SerializeField] private Button _nextTorsoButton;
        [SerializeField] private Button _previousHandsButton;
        [SerializeField] private Button _nextHandsButton;
        [SerializeField] private Button _previousLegsButton;
        [SerializeField] private Button _nextLegsButton;
        [SerializeField] private Button _previousFeetButton;
        [SerializeField] private Button _nextFeetButton;
        [SerializeField] private Button _previousBackButton;
        [SerializeField] private Button _nextBackButton;

        [Header("Mesh Buttons")]
        [SerializeField] private Button[] _hairMeshButtons;
        [SerializeField] private Button[] _headMeshButtons;
        [SerializeField] private Button[] _earsMeshButtons;
        [SerializeField] private Button[] _torsoMeshButtons;
        [SerializeField] private Button[] _handsMeshButtons;
        [SerializeField] private Button[] _legsMeshButtons;
        [SerializeField] private Button[] _feetMeshButtons;
        [SerializeField] private Button[] _backMeshButtons;

        [Header("Color Picker")]
        [SerializeField] private TextMeshProUGUI _colorPickerHeader;
        [SerializeField] private Button[] _colorPickerButtons;

        [Header("Misc UI")]
        [SerializeField] private Button _saveButton;

        public SkinnedMeshRenderer[] CurrentHeadMeshTargets => _currentHead.GetComponentsInChildren<SkinnedMeshRenderer>();

        private int _currentTorsoIndex = -1;
        private int _currentLegsIndex = -1;
        private int _currentFeetIndex = -1;
        private int _currentHandsIndex = -1;
        private int _currentHeadIndex = -1;
        private int _currentHairIndex = -1;
        private int _currentEarsIndex = -1;
        private int _currentBackIndex = -1;

        private MeshColorPaletteBehaviour _selectedMeshPalette;
        private int _selectedMeshPaletteIndex = -1;
        private Button _currentColorButton;

#region Setup/Teardown
        void Start()
        {
            // Instantiate fresh character to modify
            UpdateTorso(0);
            UpdateLegs(0);
            UpdateFeet(0);
            UpdateHands(0);
            UpdateHead(0);
            UpdateHair(0);
            UpdateEars(0);
            UpdateBack(0);

            SetupBodyPartNavigationButtons();
            SetupBodyPartMeshButtons();
            SetupColorPickerButtons();

            _saveButton.onClick.AddListener(CreatePrefab);
        }

        private void OnDestroy()
        {
            _previousHairButton.onClick.RemoveAllListeners();
            _nextHairButton.onClick.RemoveAllListeners();
            _previousHeadButton.onClick.RemoveAllListeners();
            _nextHeadButton.onClick.RemoveAllListeners();
            _previousEarsButton.onClick.RemoveAllListeners();
            _nextEarsButton.onClick.RemoveAllListeners();
            _previousTorsoButton.onClick.RemoveAllListeners();
            _nextTorsoButton.onClick.RemoveAllListeners();
            _previousHandsButton.onClick.RemoveAllListeners();
            _nextHandsButton.onClick.RemoveAllListeners();
            _previousLegsButton.onClick.RemoveAllListeners();
            _nextLegsButton.onClick.RemoveAllListeners();
            _previousFeetButton.onClick.RemoveAllListeners();
            _nextFeetButton.onClick.RemoveAllListeners();
            _previousBackButton.onClick.RemoveAllListeners();
            _nextBackButton.onClick.RemoveAllListeners();

            foreach (Button button in _hairMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _headMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _earsMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _torsoMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _handsMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _legsMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _feetMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (Button button in _backMeshButtons)
            {
                button.onClick.RemoveAllListeners();
            }

            _saveButton.onClick.RemoveAllListeners();
        }

        private void SetupBodyPartNavigationButtons()
        {
            _previousHairButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Hair));
            _nextHairButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Hair));

            _previousHeadButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Head));
            _nextHeadButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Head));

            _previousEarsButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Ears));
            _nextEarsButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Ears));

            _previousTorsoButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Torso));
            _nextTorsoButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Torso));

            _previousHandsButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Hands));
            _nextHandsButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Hands));

            _previousLegsButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Legs));
            _nextLegsButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Legs));

            _previousFeetButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Feet));
            _nextFeetButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Feet));

            _previousBackButton.onClick.AddListener(() => PreviousBodyPart(CharacterPawnBodyParts.Back));
            _nextBackButton.onClick.AddListener(() => NextBodyPart(CharacterPawnBodyParts.Back));
        }

        private void SetupBodyPartMeshButtons()
        {
            for (int i = 0; i < _hairMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _hairMeshButtons[i].onClick.AddListener(() => HairColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _headMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _headMeshButtons[i].onClick.AddListener(() => HeadColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _earsMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _earsMeshButtons[i].onClick.AddListener(() => EarsColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _torsoMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _torsoMeshButtons[i].onClick.AddListener(() => TorsoColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _handsMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _handsMeshButtons[i].onClick.AddListener(() => HandsColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _legsMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _legsMeshButtons[i].onClick.AddListener(() => LegsColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _feetMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _feetMeshButtons[i].onClick.AddListener(() => FeetColorButtonPressed(buttonIndex));
            }

            for (int i = 0; i < _backMeshButtons.Length; i++)
            {
                int buttonIndex = i;
                _backMeshButtons[i].onClick.AddListener(() => BackColorButtonPressed(buttonIndex));
            }
        }

        private void SetupColorPickerButtons()
        {
            for (int i = 0; i < ColorOptionValues.ColorDatas.Count; i++)
            {
                UITooltip tooltip = _colorPickerButtons[i].GetComponent<UITooltip>();
                tooltip.Setup(ColorOptionValues.ColorDatas[i].DisplayName);
                _colorPickerButtons[i].GetComponent<Image>().color = ColorOptionValues.ColorDatas[i].ColorValue;
                _colorPickerButtons[i].onClick.AddListener(() => ColorPickerButtonPressed(tooltip));
            }
        }
#endregion Setup/Teardown

        public void CreatePrefab()
        {
            string localPath = _savePath + "NewPawn.prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            if (PrefabUtility.SaveAsPrefabAsset(_targetPawn.gameObject, localPath))
            {
                Debug.Log("Pawn prefab created successfully");
            }
            else
            {
                Debug.LogError("Failed to create Pawn prefab");
            }
        }

        private int GetCurrentIndexForBodyPart(CharacterPawnBodyParts bodyPart)
        {
            switch (bodyPart)
            {
                case CharacterPawnBodyParts.Hair:
                    return _currentHairIndex;
                case CharacterPawnBodyParts.Head:
                    return _currentHeadIndex;
                case CharacterPawnBodyParts.Ears:
                    return _currentEarsIndex;
                case CharacterPawnBodyParts.Torso:
                    return _currentTorsoIndex;
                case CharacterPawnBodyParts.Hands:
                    return _currentHandsIndex;
                case CharacterPawnBodyParts.Legs:
                    return _currentLegsIndex;
                case CharacterPawnBodyParts.Feet:
                    return _currentFeetIndex;
                case CharacterPawnBodyParts.Back:
                    return _currentBackIndex;
                default:
                    Debug.LogError("Failed to find current index for " + bodyPart.ToString());
                    return -1;
            }
        }

        private int GetCountForBodyPart(CharacterPawnBodyParts bodyPart)
        {
            switch (bodyPart)
            {
                case CharacterPawnBodyParts.Hair:
                    return _hair.Length;
                case CharacterPawnBodyParts.Head:
                    return _heads.Length;
                case CharacterPawnBodyParts.Ears:
                    return _ears.Length;
                case CharacterPawnBodyParts.Torso:
                    return _torsos.Length;
                case CharacterPawnBodyParts.Hands:
                    return _hands.Length;
                case CharacterPawnBodyParts.Legs:
                    return _legs.Length;
                case CharacterPawnBodyParts.Feet:
                    return _feet.Length;
                case CharacterPawnBodyParts.Back:
                    return _backs.Length;
                default:
                    Debug.LogError("Failed to find body part array for " + bodyPart.ToString());
                    return -1;
            }
        }

        private void UpdateBodyPart(CharacterPawnBodyParts bodyPart, int newIndex)
        {
            switch (bodyPart)
            {
                case CharacterPawnBodyParts.Hair:
                    UpdateHair(newIndex);
                    break;
                case CharacterPawnBodyParts.Head:
                    UpdateHead(newIndex);
                    break;
                case CharacterPawnBodyParts.Ears:
                    UpdateEars(newIndex);
                    break;
                case CharacterPawnBodyParts.Torso:
                    UpdateTorso(newIndex);
                    break;
                case CharacterPawnBodyParts.Hands:
                    UpdateHands(newIndex);
                    break;
                case CharacterPawnBodyParts.Legs:
                    UpdateLegs(newIndex);
                    break;
                case CharacterPawnBodyParts.Feet:
                    UpdateFeet(newIndex);
                    break;
                case CharacterPawnBodyParts.Back:
                    UpdateBack(newIndex);
                    break;
                default:
                    Debug.LogError("Failed to update " + bodyPart.ToString());
                    break;
            }
        }

        private void PreviousBodyPart(CharacterPawnBodyParts bodyPart)
        {
            int currentIndex = GetCurrentIndexForBodyPart(bodyPart);
            int bodyPartCount = GetCountForBodyPart(bodyPart);
            int newIndex = (currentIndex - 1 >= 0) ? currentIndex - 1 : bodyPartCount - 1;

            UpdateBodyPart(bodyPart, newIndex);
        }    

        private void NextBodyPart(CharacterPawnBodyParts bodyPart)
        {
            int currentIndex = GetCurrentIndexForBodyPart(bodyPart);
            int bodyPartCount = GetCountForBodyPart(bodyPart);
            int newIndex = (currentIndex + 1 < bodyPartCount) ? currentIndex + 1 : 0;

            UpdateBodyPart(bodyPart, newIndex);
        }

        private void UpdateBodyPartMesh(MeshColorPaletteBehaviour parent, MeshColorPaletteBehaviour targetPalette, Button[] colorButtons, int index)
        {
            SkinnedMeshRenderer[] renderers = parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.enabled = false;
            }

            for (int i = 0; i < targetPalette.Targets.Length; i++)
            {
                renderers[i].enabled = true;
                renderers[i].sharedMesh = targetPalette.Targets[i].TargetRenderer.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                parent.Targets[i].ID = targetPalette.Targets[i].ID;
                parent.Targets[i].ColorOption = targetPalette.Targets[i].ColorOption;
            }

            parent.RefreshAllColors();
            UpdateColorButtons(colorButtons, parent, targetPalette.Targets.Length);
        }

        private void UpdateTorso(int index)
        {
            _currentTorsoIndex = index;
            UpdateBodyPartMesh(_torsoParent, _torsos[index].GetComponent<MeshColorPaletteBehaviour>(), _torsoMeshButtons, index);
        }

        private void UpdateLegs(int index)
        {
            _currentLegsIndex = index;
            UpdateBodyPartMesh(_legsParent, _legs[index].GetComponent<MeshColorPaletteBehaviour>(), _legsMeshButtons, index);
        }

        private void UpdateFeet(int index)
        {
            _currentFeetIndex = index;
            UpdateBodyPartMesh(_feetParent, _feet[index].GetComponent<MeshColorPaletteBehaviour>(), _feetMeshButtons, index);
        }

        private void UpdateHands(int index)
        {
            _currentHandsIndex = index;
            UpdateBodyPartMesh(_handsParent, _hands[index].GetComponent<MeshColorPaletteBehaviour>(), _handsMeshButtons, index);
        }

        private GameObject ReplaceBodyPart(GameObject prefab, Transform parent, GameObject currentBodyPart, Button[] colorButtons)
        {
            GameObject newBodyPart = Instantiate(prefab);
            newBodyPart.transform.parent = parent;
            newBodyPart.transform.SetPositionAndRotation(currentBodyPart.transform.position, currentBodyPart.transform.rotation);
            MeshColorPaletteBehaviour newMeshPalette = newBodyPart.GetComponent<MeshColorPaletteBehaviour>();

            UpdateColorButtons(colorButtons, newMeshPalette, newMeshPalette.Targets.Length);

            return newBodyPart;
        }

        private void UpdateHead(int index)
        {
            GameObject newHead = ReplaceBodyPart(_heads[index], _headParent, _currentHead.gameObject, _headMeshButtons);
            DestroyImmediate(_currentHead);
            _currentHead = newHead;
            _currentHeadIndex = index;
        }

        private void UpdateHair(int index)
        {
            GameObject newHair = ReplaceBodyPart(_hair[index], _hairParent, _currentHair, _hairMeshButtons);
            DestroyImmediate(_currentHair);
            _currentHair = newHair;
            _currentHairIndex = index;
        }

        private void UpdateEars(int index)
        {
            GameObject newEars = ReplaceBodyPart(_ears[index], _earsParent, _currentEars, _earsMeshButtons);
            DestroyImmediate(_currentEars);
            _currentEars = newEars;
            _currentEarsIndex = index;
        }

        private void UpdateBack(int index)
        {
            GameObject newBack = ReplaceBodyPart(_backs[index], _backParent, _currentBack, _backMeshButtons);
            DestroyImmediate(_currentBack);
            _currentBack = newBack;
            _currentBackIndex = index;
        }

        private void UpdateColorButtons(Button[] buttons, MeshColorPaletteBehaviour meshPalette, int usedMeshCount)
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }

            for (int i = 0; i < usedMeshCount; i++)
            {
                buttons[i].GetComponent<Image>().color = ColorOptionValues.GetColorValueFromOptions(meshPalette.Targets[i].ColorOption);
                buttons[i].GetComponent<UITooltip>().Setup(meshPalette.Targets[i].ID);
                buttons[i].gameObject.SetActive(true);
            }

            _colorPickerHeader.text = "Color Picker - " + meshPalette.Targets[0].ID;
            _selectedMeshPalette = meshPalette;
            _selectedMeshPaletteIndex = 0;
        }

        private void SetSelectedMesh(MeshColorPaletteBehaviour meshPalette, int index)
        {
            _colorPickerHeader.text = "Color Picker - " + meshPalette.Targets[index].ID;
            _selectedMeshPalette = meshPalette;
            _selectedMeshPaletteIndex = index;
        }

        public void HairColorButtonPressed(int index)
        {
            MeshColorPaletteBehaviour meshPalette = _currentHair.GetComponent<MeshColorPaletteBehaviour>();
            SetSelectedMesh(meshPalette, index);
            _currentColorButton = _hairMeshButtons[index];
        }

        private void HeadColorButtonPressed(int index)
        {
            MeshColorPaletteBehaviour meshPalette = _currentHead.GetComponent<MeshColorPaletteBehaviour>();
            SetSelectedMesh(meshPalette, index);
            _currentColorButton = _headMeshButtons[index];
        }

        private void EarsColorButtonPressed(int index)
        {
            MeshColorPaletteBehaviour meshPalette = _currentEars.GetComponent<MeshColorPaletteBehaviour>();
            SetSelectedMesh(meshPalette, index);
            _currentColorButton = _earsMeshButtons[index];
        }

        private void TorsoColorButtonPressed(int index)
        {
            SetSelectedMesh(_torsoParent, index);
            _currentColorButton = _torsoMeshButtons[index];
        }

        private void HandsColorButtonPressed(int index)
        {
            SetSelectedMesh(_handsParent, index);
            _currentColorButton = _handsMeshButtons[index];
        }

        private void LegsColorButtonPressed(int index)
        {
            SetSelectedMesh(_legsParent, index);
            _currentColorButton = _legsMeshButtons[index];
        }

        private void FeetColorButtonPressed(int index)
        {
            SetSelectedMesh(_feetParent, index);
            _currentColorButton = _feetMeshButtons[index];
        }

        private void BackColorButtonPressed(int index)
        {
            MeshColorPaletteBehaviour meshPalette = _currentBack.GetComponent<MeshColorPaletteBehaviour>();
            SetSelectedMesh(meshPalette, index);
            _currentColorButton = _backMeshButtons[index];
        }

        private void ColorPickerButtonPressed(UITooltip tooltip)
        {
            _selectedMeshPalette.Targets[_selectedMeshPaletteIndex].ColorOption = ColorOptionValues.GetColorOptionByName(tooltip.Text);
            _selectedMeshPalette.RefreshColor(_selectedMeshPaletteIndex);
            _currentColorButton.GetComponent<Image>().color = ColorOptionValues.GetColorValueFromOptions(_selectedMeshPalette.Targets[_selectedMeshPaletteIndex].ColorOption);
        }
    }
}