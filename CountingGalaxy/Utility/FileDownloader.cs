using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices; // <-- Required for DllImport

namespace Utility
{
    /// <summary>
    /// This static class acts as the bridge to native iOS code.
    /// Better to keep it in the same file as the FileDownloader for clarity.
    /// </summary>
    public static class NativeShare
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _ShareFile(string filePath);
#endif

        /// <summary>
        /// Attempts to share a file using the native iOS share sheet.
        /// </summary>
        public static void ShareFile(string _filePath)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (File.Exists(_filePath))
            {
                _ShareFile(_filePath);
            }
            else
            {
                Debug.LogError($"File not found at path: {_filePath}");
            }
#else
            Debug.Log("Native sharing is only available on iOS devices.");
#endif
        }
    }
    
    public class FileDownloader : MonoSingleton<FileDownloader>
    {
        private event Action<bool, string> OnFileDownloadCompleted;
        private string currentUrl;
        private Coroutine downloadCoroutine;

        private void OnDisable()
        {
            if (downloadCoroutine != null)
            {
                Debug.Log("Stopping download in progress: " + currentUrl);
                StopCoroutine(downloadCoroutine);
            }
        }

        public static void StartFileDownload(string _url, Action<bool, string> _downloadFinishedCallback)
        {
            Instance.TryDownloadingFile(_url, _downloadFinishedCallback);
        }

        private void TryDownloadingFile(string _url, Action<bool, string> _downloadFinishedCallback)
        {
            if (!string.IsNullOrEmpty(currentUrl))
            {
                Debug.LogWarning("A download is already in progress. Please wait until it finishes.");
                return;
            }

            currentUrl = _url;
            OnFileDownloadCompleted = _downloadFinishedCallback;
            downloadCoroutine = StartCoroutine(DownloadFileCoroutine());
        }

        private IEnumerator DownloadFileCoroutine()
        {
            Debug.Log("Starting download...");
            UnityWebRequest _request = UnityWebRequest.Get(currentUrl);
            yield return _request.SendWebRequest();

            bool _success = false;
            string _message;
            if (_request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                _message = $"Error downloading file: {_request.error}";
            }
            else
            {
                byte[] _data = _request.downloadHandler.data;
                string _fileName = Path.GetFileName(new Uri(currentUrl).AbsolutePath);
                _success = TrySaveFile(_fileName, _data, GetMimeType(_fileName), out _message);
            }

            OnFileDownloadCompleted?.Invoke(_success, _message);
            currentUrl = null;
            downloadCoroutine = null;
        }
        
        private bool TrySaveFile(string _fileName, byte[] _data, string _mimeType, out string _message)
        {
            bool _success = false;
            
#if UNITY_ANDROID && !UNITY_EDITOR
            _success = AndroidNativeStorage.SaveFileToDownloads(_data, _fileName, _mimeType);
            _message = _success ? $"File '{_fileName}' was saved to your Downloads folder." : "Failed to save file. Check logs for details.";

#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            // For Windows and the Editor, use the standard path method.
            string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", _fileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                File.WriteAllBytes(_path, _data);
                _success = true;
                _message = $"File '{_fileName}' was saved to your Downloads folder.";
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save file: {e.Message}");
                _message = "An error occurred while saving the file.";
            }
            
#elif UNITY_IOS
            // cache the file
            string _path = Path.Combine(Application.temporaryCachePath, _fileName);
            try
            {
                File.WriteAllBytes(_path, _data);
                NativeShare.ShareFile(_path);
                _message = $"File '{_fileName}' shared successfully.";
                _success = true;
            }
            catch (Exception _ex)
            {
                Debug.LogError($"Failed to save file: {_ex.Message}");
                _message = "An error occurred while saving the file.";
            }
#endif
            
            return _success;
        }

        private string GetMimeType(string _fileName)
        {
            string _extension = Path.GetExtension(_fileName).ToLowerInvariant();
            return _extension switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }
    }
}