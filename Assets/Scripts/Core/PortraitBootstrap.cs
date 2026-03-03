using UnityEngine;

namespace IdleSpace.Core
{
    public class PortraitBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Application.targetFrameRate = 60;
        }
    }
}
