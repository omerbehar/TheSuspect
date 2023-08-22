using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using Screens.Bases;
using Screens.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    public class Screen3 : ScreenBase, ISaveData, ILoadData
    {
        const int INITIAL_INPUT_FIELDS = 3;
        private const int MINIMUM_NAMES_ALLOWED = 2;
        [SerializeField] private GameObject addInputFieldGO;
        [SerializeField] private Button addNameInputFieldButton;
        [SerializeField] private List<TMP_InputField> nameInputFields = new();
        [SerializeField] private Transform inputFieldsLayout;
        [SerializeField] private GameObject inputFieldPrefab;
        [SerializeField] private string[] names;
        private int inputFieldsCount = INITIAL_INPUT_FIELDS;
        [SerializeField] private TMP_InputField instructorNameInputField;
        [SerializeField] private string instructorName;
        
        private int namesAddedCount;

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
            LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
            await LoadData();
            names = new string[Data.MAX_PLAYERS];
            IsAssignmentCompleted();
            AddListeners();
        }

        private void IsAssignmentCompleted()
        {
            namesAddedCount = 0;
            foreach (TMP_InputField nameInputField in nameInputFields)
            {
                if (nameInputField.text != "")
                {
                    namesAddedCount++;
                }
            }
            if (namesAddedCount >= MINIMUM_NAMES_ALLOWED && instructorName != "")
            {
                EventManager.AssignmentCompleted.Invoke();
            }
        }

        private void AddListeners()
        {
            addNameInputFieldButton.onClick.AddListener(AddNameInputField);
            foreach (TMP_InputField nameInputField in nameInputFields)
            {
                nameInputField.onSelect.AddListener(delegate { OnInputFieldSelected(nameInputField); });
                nameInputField.onValueChanged.AddListener(delegate { UpdateNames(); });
            }

            instructorNameInputField.onSelect.AddListener(delegate { OnInputFieldSelected(instructorNameInputField); });
            instructorNameInputField.onValueChanged.AddListener(delegate { UpdateNames(); });
        }

        private void OnInputFieldSelected(TMP_InputField inputField)
        {
            // if (TouchScreenKeyboard.isSupported)
            // {
            //     TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "");
            //     inputField.ActivateInputField();
            //     inputField.Select();
            // }
            // else
            // {
            //     Debug.Log("TouchScreenKeyboard not supported");
            //     inputField.Select();
            // }
        }

        private void AddNameInputField()
        {
            GameObject inputFieldGameObject = Instantiate(inputFieldPrefab, inputFieldsLayout);
            TMP_InputField nameInputField = inputFieldGameObject.GetComponent<TMP_InputField>();
            nameInputFields.Add(nameInputField);
            nameInputField.onSelect.AddListener(delegate { OnInputFieldSelected(nameInputField); });
            nameInputField.onValueChanged.AddListener(delegate { UpdateNames(); });
            LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
            inputFieldsCount++;
            if (inputFieldsCount == Data.MAX_PLAYERS)
            {
                addInputFieldGO.SetActive(false);
            }
        }

        private async Task UpdateNames()
        {
            for (int i = 0; i < nameInputFields.Count; i++)
            {
                if (nameInputFields[i] != null)
                {
                    names[i] = nameInputFields[i].text;
                }
            }
            instructorName = instructorNameInputField.text;
            await SaveData();
            IsAssignmentCompleted();
        }

        public async Task SaveData()
        {
            Data.InstructorName = instructorName;
            Data.PlayerNames = names;
            Data.SaveData();
            await Database.SaveDataToDatabase();
        }

        public async Task LoadData()
        {
            Data.LoadData();
            await Database.LoadDataFromDatabase();
            instructorName = Data.InstructorName;
            names = Data.PlayerNames;
            instructorNameInputField.text = instructorName;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] != null && names[i] != "")
                {
                    if (i > nameInputFields.Count - 1)
                    {
                        AddNameInputField();
                    }
                    nameInputFields[i].text = names[i];
                }
            }
        }
    }
}
