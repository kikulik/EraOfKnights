using System;
using TMPro;
using UnityEngine;

namespace Worldrift.Client
{
    public class ConnectUIController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_Dropdown factionDropdown;

        public event Action<string, string> ConnectRequested;

        private void Start()
        {
            if (nameInput != null)
            {
                nameInput.text = $"Rift-{UnityEngine.Random.Range(1000, 9999)}";
            }

            if (factionDropdown != null)
            {
                factionDropdown.ClearOptions();
                factionDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Meridian Covenant",
                    "Umbral Relay"
                });
            }
        }

        public void OnConnectPressed()
        {
            var name = nameInput != null ? nameInput.text : "Rift-Player";
            var faction = factionDropdown != null ? factionDropdown.options[factionDropdown.value].text : "Meridian Covenant";
            ConnectRequested?.Invoke(name, faction);
        }
    }
}
