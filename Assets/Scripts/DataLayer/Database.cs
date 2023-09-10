using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace DataLayer
{
    public static class Database
    {
        public static async Task SaveDataToDatabase()
        {
            WWWForm form = new WWWForm();
            form.AddField("guid", Data.guid);
            form.AddField("teamName", Data.TeamName);
            form.AddField("score", Data.Score);
            form.AddField("instructorName", Data.InstructorName);
            Debug.Log(Data.CompanyName);
            form.AddField("companyName", Data.CompanyName);
            form.AddField("playerNames", string.Join(",", Data.PlayerNames));
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            form.AddField("date", date);
            Uri uri = new Uri("https://icl.eitangames.co.il/savedata.php");
            try
            {
                using UnityWebRequest request = UnityWebRequest.Post(uri, form);
                await request.SendWebRequestAsync();
                //Debug.Log(request.downloadHandler.text);
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        public static async Task LoadDataFromDatabase()
        {
            Uri uri = new Uri("https://icl.eitangames.co.il/savedata.php");
            string fullUrl = $"{uri}?guid={Data.guid}";
            //Debug.Log(Data.guid);
            try
            {
                using UnityWebRequest request = UnityWebRequest.Get(fullUrl);
                await request.SendWebRequestAsync();
                //Debug.Log(request.downloadHandler.text);
                var jsonData = JsonUtility.FromJson<ServerData>(request.downloadHandler.text);
                // Data.guid = jsonData.guid;
                // Data.TeamName = jsonData.teamName;
                // Data.Score = jsonData.score;
                // Data.InstructorName = jsonData.instructorName;
                // Data.PlayerNames = jsonData.playerNames.Split(',');
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError(ex.Message);
            }
            
        }

        private static async Task SendWebRequestAsync(this UnityWebRequest request)
        {
            TaskCompletionSource<UnityWebRequest> tcs = new ();
            UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();


            asyncOp.completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    tcs.SetException(new UnityWebRequestException(request));
                }
                else
                {
                    tcs.SetResult(request);
                }
            };

            await tcs.Task;
        }

        private class UnityWebRequestException : Exception
        {
            public UnityWebRequestException(UnityWebRequest request)
                : base($"Error: {request.error}, URL: {request.url}")
            {
            }
        }
        [Serializable]
        private class ServerData
        {
            //public string id;
            public string guid;
            public string teamName;
            public int score;
            public string instructorName;
            public string playerNames;
            //public DateTime date;
        }
    }
}