using System.Threading.Tasks;
using DataLayer;
using Screens.Bases;
using Screens.Interfaces;
using TMPro;
using UnityEngine;
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
            IsAssignmentCompleted();
            AddListeners();
        }
        // private void Update()
        // {
        //     // Check for both touch and mouse input
        //     if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //     {
        //         // Handle touch input
        //         if (IsPointerOverUI(Input.GetTouch(0).position))
        //         {
        //             keyboardActive = false;  // Touch outside the keyboard and input field
        //         }
        //     }
        //     else if (Input.GetMouseButtonDown(0))
        //     {
        //         // Handle mouse input
        //         if (IsPointerOverUI(Input.mousePosition))
        //         {
        //             keyboardActive = false;  // Mouse click outside the keyboard and input field
        //         }
        //     }
        // }
        // private bool IsPointerOverUI(Vector2 position)
        // {
        //     PointerEventData pointerData = new PointerEventData(EventSystem.current)
        //     {
        //         position = position
        //     };
        //
        //     List<RaycastResult> raycastResults = new();
        //     EventSystem.current.RaycastAll(pointerData, raycastResults);
        //
        //     // Filter out any results with the "IgnoreUI" tag
        //     foreach (var result in raycastResults)
        //     {
        //         if (result.gameObject.CompareTag("IgnoreUI"))
        //         {
        //             continue;  // Skip elements with the "IgnoreUI" tag
        //         }
        //
        //         // If we find any other UI element, return true
        //         return true;
        //     }
        //
        //     // No relevant UI elements were found under the pointer
        //     return false;
        // }
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
            teamNameInputField.onSelect.AddListener((string arg) =>
            {
                
                OnInputFieldSelect(arg, teamNameInputField);
            });
            teamNameInputField.onDeselect.AddListener(OnInputFieldDeSelect);
        }

        private void OnInputFieldDeSelect(string arg0)
        {
            if (!keyboardActive)
            {
                keyboardAnimator.SetBool(KeyboardIn, false);
                keyboardAnimator2.SetBool(KeyboardIn, false);
            }
        }

        private void OnInputFieldSelect(string arg0, TMP_InputField inputField)
        {
            // GameManagerKB.Instance.textBox = inputField;
            keyboardActive = true;
            keyboardAnimator.SetBool(KeyboardIn, true);
            keyboardAnimator2.SetBool(KeyboardIn, true);
        }

        public override async void OnNextButtonClicked()
        {
            await SaveData();
            await Database.SaveDataToDatabase();
            base.OnNextButtonClicked();
        }
        
        public async Task SaveData()
        {
            Data.playerCount = playerCountDropdown.value;
            Data.FactoryName = chooseFactoryDropdown.options[chooseFactoryDropdown.value].text;
            Data.TeamName = teamNameInputField.text;
            Data.SaveData();
        }

        public async Task LoadData()
        {
            Data.LoadData();
        }

    }
}
