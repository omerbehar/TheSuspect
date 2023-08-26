// using System;
// using System.IO;
// using System.Runtime.InteropServices;
// using SFB;
// using UnityEngine;
// using UnityEngine.Video;
//
// namespace DefaultNamespace
// {
//     public class MobileCameraVideoBridge : MonoBehaviour
//     {
// #if UNITY_WEBGL && !UNITY_EDITOR
//
//         [DllImport("__Internal")]
//         private static extern void CaptureVideo(CaptureVideoCallback callback);
//     
//         delegate void CaptureVideoCallback(string videoData);
//         [AOT.MonoPInvokeCallback(typeof(CaptureVideoCallback))]
//         public static void OnVideoReceived(string videoData)
//         {
//             byte[] bytes = Convert.FromBase64String(videoData);
//             VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();
//             string path = Path.Combine(Application.persistentDataPath, "myVideo.mp4");
//             File.WriteAllBytes(path, bytes);
//             videoPlayer.url = path;
//             videoPlayer.Prepare();
//             videoPlayer.Play();
//         }
// #endif
//         public void RequestVideo()
//         {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         CaptureVideo(OnVideoReceived);
// #else
//             
//             PickVideoDesktop();
// #endif
//         }
//         public void PickVideoDesktop()
//         {
//             string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Video File", "", "*", false);
//             if(paths.Length > 0)
//             {
//                 File.Copy(paths[0], Path.Combine(Application.persistentDataPath, storedVideoName), true);
//                 PlayerPrefs.SetString("capturedVideo", storedVideoName);
//                 PlayerPrefs.Save();
//                 StartCoroutine(DelayPlayVideo("file://" + Path.Combine(Application.persistentDataPath, storedVideoName)));
//             }
//         }
//     }
// }