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
        [SerializeField] private TMP_Dropdown chooseFactoryDropdown;
        [SerializeField] private InputField teamNameInputField;
        [SerializeField] private Button fakeNextButton;
        [SerializeField] private Image teamNameInputFieldRedBorder;
        [SerializeField] private Image playerCountDropdownRedBorder;
        [SerializeField] private Image chooseFactoryDropdownRedBorder;
        
        
        
        protected override async void Start()
        {
            await Initialize();
#if !UNITY_EDITOR && UNITY_WEBGL 
                WebGLInput.mobileKeyboardSupport = true;
#endif
        }

        private async Task Initialize()
        {
            base.Start();
            await LoadData();
            IsAssignmentCompleted();
            AddListeners();
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
