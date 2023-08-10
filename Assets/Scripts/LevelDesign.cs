using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new Level design", menuName = "Level", order = 0)]
public class LevelDesign : ScriptableObject
{
    [SerializeField] public Image suspectImage;

    [SerializeField] public string suspectText;
    
    [SerializeField] public List<string> answers;

    [SerializeField] public int correctAnswer = 0;
    [SerializeField] public LevelDesign wrongAnswerLevel;
    [SerializeField] public LevelDesign rightAnswerLevel;
}