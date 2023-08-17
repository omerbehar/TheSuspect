using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DataLayer
{
    public class Database : MonoBehaviour
    {

        public void Start()
        {
            Data.LoadData();
            StartCoroutine(SaveData());
        }
        private IEnumerator SaveData()
        {
            WWWForm form = new WWWForm();
            form.AddField("guid", Data.guid);
            form.AddField("name", Data.TeamName);
            form.AddField("score", Data.Score);
            Uri uri = new Uri("http://localhost/sqlconnect/savedata.php");
            UnityWebRequest www = UnityWebRequest.Post(uri, form);
            www.SendWebRequest();
            yield return www;
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error saving data: " + www.error);
            }
            else
            {
                Debug.Log("Data saved successfully");
            }
        }

        public void VerifyInput()
        {
            
        }
        
    }
}