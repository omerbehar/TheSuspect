using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using Screens.Bases;
using Screens.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Screens
{
    public class Screen3 : ScreenBase, ISaveData
    {
        private static readonly int KeyboardIn = Animator.StringToHash("keyboardIn");
        [SerializeField] private TMP_Dropdown playerCountDropdown;
        [SerializeField] private TMP_Dropdown chooseFactoryDropdown;
        
        [SerializeField] private TMP_InputField teamNameInputField;
        [SerializeField] private Button fakeNextButton;
        [SerializeField] private Image teamNameInputFieldRedBorder;
        [SerializeField] private Image playerCountDropdownRedBorder;
        [SerializeField] private Image chooseFactoryDropdownRedBorder;
        [SerializeField] private Animator keyboardAnimator;
        [SerializeField] private Animator keyboardAnimator2;
        private TouchScreenKeyboard keyboard;
        private bool keyboardActive;
        private bool isKeyboardActive;


        protected override async void Start()
        {
            await Initialize();
#if !UNITY_EDITOR && UNITY_WEBGL 
            keyboard.active = false;
            UnityEngine.WebGLInput.mobileKeyboardSupport = true;
#endif
        }

        private async Task Initialize()
        {
            base.Start();
            await LoadData();
            ShowKeyboard(false);
            IsAssignmentCompleted();
            AddListeners();
        }
        private void Update()
        {
            WasClickedToCloseKeyboard();
        }

        private void WasClickedToCloseKeyboard()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (IsPointerOverKeyboardRelatedUI(Input.GetTouch(0).position)) return;
                if (isKeyboardActive) ShowKeyboard(false);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverKeyboardRelatedUI(Input.mousePosition)) return;
                Debug.Log(isKeyboardActive);
                if (isKeyboardActive) ShowKeyboard(false);
            }
        }

        private void ShowKeyboard(bool showKeyboard)
        {
            keyboardAnimator.SetBool(KeyboardIn, showKeyboard);
            keyboardAnimator2.SetBool(KeyboardIn, showKeyboard);
            isKeyboardActive = showKeyboard;
        }

        private static bool IsPointerOverKeyboardRelatedUI(Vector2 position)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = position
            };
            List<RaycastResult> raycastResults = new();
            EventSystem.current.RaycastAll(pointerData, raycastResults);
            foreach (RaycastResult result in raycastResults)
            {
                if (!result.gameObject.CompareTag("KeyboardRelatedUI"))
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void OnKeyboardClick()
        {
            // Function to be called by buttons on the keyboard to keep it active
            keyboardActive = true;
        }

        private void OnFakeNextButtonClicked()
        {
            teamNameInputFieldRedBorder.color = teamNameInputField.text == "" ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 0);
            playerCountDropdownRedBorder.color = playerCountDropdown.value == 0 ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 0);
            chooseFactoryDropdownRedBorder.color = chooseFactoryDropdown.value == 0 ? new Color(1,0, 0,1) : new Color(1, 1, 1, 0);
        }

        private void IsAssignmentCompleted()
        {

            if (chooseFactoryDropdown.value != 0 && playerCountDropdown.value != 0 && teamNameInputField.text != "")
            {
                EventManager.AssignmentCompleted.Invoke();
                fakeNextButton.gameObject.SetActive(false);
            }
            else
            {
                EventManager.AssignmentNotCompleted.Invoke();
                fakeNextButton.gameObject.SetActive(true);
            }
        }

        private void AddListeners()
        {
            teamNameInputField.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            playerCountDropdown.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            chooseFactoryDropdown.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            fakeNextButton.onClick.AddListener(OnFakeNextButtonClicked);
            teamNameInputField.onSelect.AddListener(_ =>
            {
                OnInputFieldSelect();
            });
            teamNameInputField.onDeselect.AddListener(OnInputFieldDeSelect);
        }

        private void OnInputFieldDeSelect(string arg0)
        {
            if (!keyboardActive)
            {
                ShowKeyboard(false);
            }
        }

        private void OnInputFieldSelect()
        {
            keyboardActive = true;
            ShowKeyboard(true);
        }

        public override async void OnNextButtonClicked()
        {
            await SaveData();
            await Database.SaveDataToDatabase();
            base.OnNextButtonClicked();
        }
        
        public Task SaveData()
        {
            Data.playerCount = playerCountDropdown.value;
            Data.FactoryName = chooseFactoryDropdown.options[chooseFactoryDropdown.value].text;
            Data.TeamName = teamNameInputField.text;
            Data.SaveData();
            return Task.CompletedTask;
        }

        private static Task LoadData()
        {
            Data.LoadData();
            return Task.CompletedTask;
        }

    }
}
