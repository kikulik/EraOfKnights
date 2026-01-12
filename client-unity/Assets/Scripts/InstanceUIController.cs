using TMPro;
using UnityEngine;

namespace Worldrift.Client
{
    public class InstanceUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text chatLog;

        public void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        public void AppendChat(string message)
        {
            if (chatLog != null)
            {
                chatLog.text += $"\n{message}";
            }
        }
    }
}
