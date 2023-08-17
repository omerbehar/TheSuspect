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
            form.AddField("playerNames", string.Join(",", Data.PlayerNames));
            Uri uri = new Uri("http://localhost:81/sqlconnect/savedata.php");
            try
            {
                using UnityWebRequest request = UnityWebRequest.Post(uri, form);
                await request.SendWebRequestAsync();
                Debug.Log(request.downloadHandler.text);
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

    }
}