
using DataLayer;
using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private TMP_Text teamName;
    [SerializeField] private TMP_Text teamScore;
    
    void Start()
    {
        teamName.text = Data.TeamName;
        
        teamScore.text = Data.Score.ToString();
    }
}
