using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DataLayer
{
    public class Database : MonoBehaviour
    {
        public void CallSaveData()
        {
            StartCoroutine(SaveData());
        }
        private IEnumerator SaveData()
        {
            WWWForm form = new WWWForm();
            form.AddField("name", Data.TeamName);
            form.AddField("score", Data.Score);
            
            UnityWebRequest www = new UnityWebRequest("http://localhost/sqlconnect/savedata.php", "POST");
            www.uploadHandler = new UploadHandlerRaw(form.data);
            yield return www;
            if (www.error == null)
            {
                Debug.Log("Data saved successfully");
            }
            else
            {
                Debug.Log("Error saving data: " + www.error);
            }
        }

        public void VerifyInput()
        {
            
        }
        
    }
}