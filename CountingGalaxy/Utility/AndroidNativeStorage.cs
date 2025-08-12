#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// A helper class to interact with Android's MediaStore API for saving files.
    /// </summary>
    public static class AndroidNativeStorage
    {
        /// <summary>
        /// Saves a file to the public "Downloads" collection using MediaStore.
        /// </summary>
        /// <param name="_data">The raw byte data of the file.</param>
        /// <param name="_fileName">The desired name of the file, e.g., "my_document.pdf".</param>
        /// <param name="_mimeType">The MIME type of the file, e.g., "application/pdf".</param>
        public static bool SaveFileToDownloads(byte[] _data, string _fileName, string _mimeType)
        {
            try
            {
                using (AndroidJavaClass _unityPlayer = new("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject _context = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (AndroidJavaClass _mediaStore = new("android.provider.MediaStore$Downloads"))
                        {
                            using (AndroidJavaObject _contentResolver = _context.Call<AndroidJavaObject>("getContentResolver"))
                            {
                                using (AndroidJavaObject _values = new("android.content.ContentValues"))
                                {
                                    _values.Call("put", "_display_name", _fileName);
                                    _values.Call("put", "mime_type", _mimeType);
                                    using (AndroidJavaObject _externalUri = _mediaStore.CallStatic<AndroidJavaObject>("getContentUri", "external_primary"))
                                    {
                                        using (AndroidJavaObject _newFileUri = _contentResolver.Call<AndroidJavaObject>("insert", _externalUri, _values))
                                        {
                                            if (_newFileUri == null)
                                            {
                                                Debug.LogError("MediaStore failed to create new file entry.");
                                                return false;
                                            }

                                            using (AndroidJavaObject _outputStream = _contentResolver.Call<AndroidJavaObject>("openOutputStream", _newFileUri))
                                            {
                                                _outputStream.Call("write", _data);
                                                _outputStream.Call("flush");
                                                _outputStream.Call("close");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Debug.Log("File saved successfully to Downloads");
                return true;
            }
            catch (Exception _ex)
            {
                Debug.LogError($"Failed to save file using MediaStore: {_ex.Message}");
                return false;
            }
        }
    }
}
#endif