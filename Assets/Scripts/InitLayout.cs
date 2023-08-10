using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitLayout : MonoBehaviour
{
    [SerializeField] private Image suspectTextImagePrefab;

    [SerializeField] private VerticalLayoutGroup answersLayout;

    [SerializeField] private Slider scoreSlider;

    [SerializeField] private Button answerPrefab;
    private LevelDesign levelDesign;
    private Image suspectTextImage;
    private Transform answerLayoutGameObject;
    private List<Button> answerButtons = new();
    private void Start()
    {
        Instantiate(scoreSlider, transform);
        answerLayoutGameObject = Instantiate(answersLayout, transform).transform;
        suspectTextImage = Instantiate(suspectTextImagePrefab, transform);
        InitLevel();
    }

    private void InitLevel()
    {
        levelDesign = GameManager.instance.currentLevel;
        Image suspectImage = Instantiate(levelDesign.suspectImage, transform);
        suspectImage.transform.SetSiblingIndex(0);
        suspectTextImage.GetComponentInChildren<TextMeshProUGUI>().text = levelDesign.suspectText;
        foreach (Button answerButton in answerButtons)
        {
            if (answerButton != null) Destroy(answerButton.gameObject);
        }
        for (int i = 0; i < levelDesign.answers.Count; i++)
        {
            Button answerButton = Instantiate(answerPrefab, answerLayoutGameObject);
            answerButton.GetComponentInChildren<TextMeshProUGUI>().text = levelDesign.answers[i];
            int i1 = i;
            answerButton.onClick.AddListener(() => AnswerButtonClicked(i1));
            answerButtons.Add(answerButton);
        }
    }

    private void AnswerButtonClicked(int selectedAnswer)
    {
        GameManager.instance.currentLevel = selectedAnswer == levelDesign.correctAnswer ? levelDesign.rightAnswerLevel : levelDesign.wrongAnswerLevel;
        InitLevel();
    }
}
