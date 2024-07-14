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
        [SerializeField] private TMP_Dropdown playerCountDropdown;
        [SerializeField] private TMP_Dropdown companyDropdown;
        // [SerializeField] private TMP_Dropdown instructorDropdown;
        [SerializeField] private TMP_Dropdown chooseFactoryDropdown;
        [SerializeField] private InputField teamNameInputField;
        [SerializeField] private Button fakeNextButton;
        [SerializeField] private Image companyDropdownRedBorder;
        // [SerializeField] private Image instructorDropdownRedBorder;
        [SerializeField] private Image teamNameInputFieldRedBorder;
        [SerializeField] private Image playerCountDropdownRedBorder;
        [SerializeField] private Image chooseFactoryDropdownRedBorder;
        
        
        
        protected override async void Start()
        {
            await Initialize();
            #if !UNITY_EDITOR && UNITY_WEBGL 
                // disable WebGLInput.mobileKeyboardSupport so the built-in mobile keyboard support is disabled.
                WebGLInput.mobileKeyboardSupport = true;
            #endif
        }

        private async Task Initialize()
        {
            base.Start();
            await LoadData();
            IsAssignmentCompleted();
            AddListeners();
            OnCompanyChanged();
        }

        private void OnFakeNextButtonClicked()
        {
            Debug.Log("Fake Next Button Clicked");
            companyDropdownRedBorder.color = companyDropdown.value == 0 ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 0);
            // instructorDropdownRedBorder.color = instructorDropdown.value == 0 ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 0);
            teamNameInputFieldRedBorder.color = teamNameInputField.text == "" ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 0);
            playerCountDropdownRedBorder.color = playerCountDropdown.value == 0 ? new Color(1, 0, 0, 1) : new Color(1, 1, 1, 0);
            chooseFactoryDropdownRedBorder.color = chooseFactoryDropdown.value == 0 ? new Color(1,0, 0,1) : new Color(1, 1, 1, 0);
        }

        private void IsAssignmentCompleted()
        {

            // if (chooseFactoryDropdown.value != 0 && playerCountDropdown.value != 0 && companyDropdown.value != 0 && instructorDropdown.value != 0 && teamNameInputField.text != "")
            if (chooseFactoryDropdown.value != 0 && playerCountDropdown.value != 0 && companyDropdown.value != 0 && teamNameInputField.text != "")
            {
                EventManager.AssignmentCompleted.Invoke();
                fakeNextButton.gameObject.SetActive(false);
                // fakeNextButton.interactable = false;
            }
            else
            {
                EventManager.AssignmentNotCompleted.Invoke();
                fakeNextButton.gameObject.SetActive(true);
                //fakeNextButton.interactable = true;
            }
        }

        private void AddListeners()
        {
            companyDropdown.onValueChanged.AddListener(delegate { OnCompanyChanged(); });
            // instructorDropdown.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            teamNameInputField.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            playerCountDropdown.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            chooseFactoryDropdown.onValueChanged.AddListener(delegate { IsAssignmentCompleted(); });
            fakeNextButton.onClick.AddListener(OnFakeNextButtonClicked);
        }

        private void OnCompanyChanged()
        {
            // switch (companyDropdown.value)
            // {
            //     case 0:
            //         instructorDropdown.ClearOptions();
            //         instructorDropdown.AddOptions(Data.NoInstructors);
            //         instructorDropdown.interactable = false;
            //         break;
            //     case 1:
            //         instructorDropdown.ClearOptions();
            //         instructorDropdown.AddOptions(Data.IndieInstructor);
            //         instructorDropdown.interactable = true;
            //         break;
            //     default:
            //         instructorDropdown.ClearOptions();
            //         instructorDropdown.AddOptions(Data.Instructors);
            //         instructorDropdown.interactable = true;
            //         break;
            // }
            IsAssignmentCompleted();
        }

        public override async void OnNextButtonClicked()
        {
            await SaveData();
            await Database.SaveDataToDatabase();
            base.OnNextButtonClicked();
        }
        
        public async Task SaveData()
        {
            // Data.InstructorName = instructorDropdown.options[instructorDropdown.value].text;
            Data.CompanyName = companyDropdown.options[companyDropdown.value].text;
            Data.playerCount = playerCountDropdown.value;
            Data.FactoryName = chooseFactoryDropdown.options[chooseFactoryDropdown.value].text;
            Data.TeamName = teamNameInputField.text;
            Data.SaveData();
        }

        public async Task LoadData()
        {
            Data.LoadData();
            // instructorDropdown.value = Data.InstructorName == ""
            //     ? 0
            //     : instructorDropdown.options.FindIndex(option => option.text == Data.InstructorName);
            // companyDropdown.value = Data.CompanyName == ""
            //     ? 0
            //     : companyDropdown.options.FindIndex(option => option.text == Data.CompanyName);
            // Debug.Log(Data.CompanyName);
            // instructorDropdown.value = Data.InstructorName == ""
            //     ? 0
            //     : instructorDropdown.options.FindIndex(option => option.text == Data.InstructorName);
            // OnCompanyChanged();
        }
    }
}
