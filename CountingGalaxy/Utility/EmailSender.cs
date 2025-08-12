using System;
using UnityEngine;

namespace Utility
{
    public static class EmailSender
    {
        // Opens the default email client with the specified recipient and message.
        public static void Send(string _recipientEmail, string _subject, string _body)
        {
            if (string.IsNullOrEmpty(_recipientEmail))
            {
                Debug.LogError("EmailSender: Recipient email cannot be null or empty.");
                return;
            }

            // Every part of the mailto URL must be properly escaped to handle special characters.
            string _subjectEscaped = Uri.EscapeDataString(_subject ?? "");
            string _bodyEscaped = Uri.EscapeDataString(_body ?? "");

            // This is the standard format for mailto links.
            string _mailToUrl = $"mailto:{_recipientEmail}?subject={_subjectEscaped}&body={_bodyEscaped}";
            Debug.Log($"EmailSender: Opening mail client with URL: {_mailToUrl}");
            Application.OpenURL(_mailToUrl);
        }
    }
}