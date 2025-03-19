using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UISystem
{
    public class PlayerHUD : UIViewController
    {
        public override void OnLoad()
        {
            Addressables.LoadAssetAsync<PlayerStateBar>("PlayerStateBar").Completed += (handle) =>
            {
                if (handle.IsDone) AddView(Instantiate(handle.Result, transform));
                handle.Release();
            };

            Addressables.LoadAssetAsync<InteractionInfo>("InteractionInfo").Completed += (handle) =>
            {
                if (handle.IsDone) AddView(Instantiate(handle.Result, transform));
                handle.Release();
            };
        }
    }
}

