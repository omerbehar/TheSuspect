using System.Collections.Generic;
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

        private void Start()
        {
            base.Start();
            LoadData();
            names = new string[Data.MAX_PLAYERS];
            addNameInputFieldButton.onClick.AddListener(AddNameInputField);
            LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
        }
        
        private void AddNameInputField()
        {
            GameObject inputFieldGameObject = Instantiate(inputFieldPrefab, inputFieldsLayout);
            nameInputFields.Add(inputFieldGameObject.GetComponent<TMP_InputField>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
            inputFieldsCount++;
            if (inputFieldsCount == Data.MAX_PLAYERS)
            {
                addInputFieldGO.SetActive(false);
            }
        }

        void Update()
        {
            UpdateNames();
        }

        private void UpdateNames()
        {
            namesAddedCount = 0;
            for (int i = 0; i < nameInputFields.Count; i++)
            {
                if (nameInputFields[i] != null)
                {
                    names[i] = nameInputFields[i].text;
                    if (names[i] != "")
                    {
                        namesAddedCount++;
                    }
                }
            }
            instructorName = instructorNameInputField.text;
            SaveData();
            if (namesAddedCount >= MINIMUM_NAMES_ALLOWED && instructorName != "")
            {
                EventManager.AssignmentCompleted.Invoke();
            }
        }

        public void SaveData()
        {
            Data.InstructorName = instructorName;
            Data.PlayerNames = names;
            Data.SaveData();
        }

        public void LoadData()
        {
            Data.LoadData();
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
