using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Ultilities.ClientAuthority {

    //Used for syncing a transform with client side changes. This include host. 
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform {

        //Used to determine who can write to this transform. Owner client only
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }    
}