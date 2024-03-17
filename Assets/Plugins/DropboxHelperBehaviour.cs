using UnityEngine;

namespace Plugins.Scripts.Dropbox
{
    public class DropboxHelperBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR
        
        // Эти методы можно вызывать ПКМ по скрипту в инспекторе. Можно без плеймода
        
        [ContextMenu("GetAuthCode")]
        public void GetAuthCode()
        {
            DropboxHelper.GetAuthCode();
        }
        [ContextMenu("GetRefreshToken")]
        public void GetRefreshToken()
        {
            DropboxHelper.GetRefreshToken();
        }
#endif
    }
}