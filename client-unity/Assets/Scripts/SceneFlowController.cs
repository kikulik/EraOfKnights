using UnityEngine;
using UnityEngine.SceneManagement;

namespace Worldrift.Client
{
    public class SceneFlowController : MonoBehaviour
    {
        public const string BootScene = "Boot";
        public const string ConnectScene = "Connect";
        public const string WorldScene = "World";
        public const string InstanceScene = "Instance";

        public void GoToConnect()
        {
            SceneManager.LoadScene(ConnectScene);
        }

        public void GoToWorld()
        {
            SceneManager.LoadScene(WorldScene);
        }

        public void GoToInstance()
        {
            SceneManager.LoadScene(InstanceScene);
        }
    }
}
