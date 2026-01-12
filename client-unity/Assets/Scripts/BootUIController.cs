using TMPro;
using UnityEngine;

namespace Worldrift.Client
{
    public class BootUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private GameObject blockingPanel;

        public void SetStatus(string status)
        {
            if (statusText != null)
            {
                statusText.text = status;
            }
        }

        public void SetBlocking(bool isBlocking, string message)
        {
            if (blockingPanel != null)
            {
                blockingPanel.SetActive(isBlocking);
            }

            SetStatus(message);
        }
    }
}
