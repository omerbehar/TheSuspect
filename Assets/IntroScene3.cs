using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroScene3 : MonoBehaviour
{
    const int MAX_INPUT_FIELDS = 8;
    const int INITIAL_INPUT_FIELDS = 3;
    [SerializeField] private Button addNameInputFieldButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<TMP_InputField> nameInputFields = new();
    [SerializeField] private Transform inputFieldsLayout;
    [SerializeField] private GameObject inputFieldPrefab;
    [SerializeField] private string[] names = new string[MAX_INPUT_FIELDS];
    private int inputFieldsCount = INITIAL_INPUT_FIELDS;
    [SerializeField] private TMP_InputField instructorNameInputField;
    [SerializeField] private string instructorName;
    void Start()
    {
        addNameInputFieldButton.onClick.AddListener(AddNameInputField);
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
    }

    private void AddNameInputField()
    {
        GameObject inputFieldGameObject = Instantiate(inputFieldPrefab, inputFieldsLayout);
        nameInputFields.Add(inputFieldGameObject.GetComponent<TMP_InputField>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
        inputFieldsCount++;
        if (inputFieldsCount == MAX_INPUT_FIELDS)
        {
            addNameInputFieldButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateNames();
    }

    private void UpdateNames()
    {
        for (int i = 0; i < nameInputFields.Count; i++)
        {
            if (nameInputFields[i] != null)
            {
                names[i] = nameInputFields[i].text;
            }
        }
        instructorName = instructorNameInputField.text;
    }
}
