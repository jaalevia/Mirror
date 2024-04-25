using UnityEngine;
using Mirror;

namespace Mirror.Examples.CharacterSelection
{
    public class PlayerEmpty : NetworkBehaviour
    {
        private SceneReferencer sceneReferencer;

        public override void OnStartAuthority()
        {
            sceneReferencer = GameObject.FindObjectOfType<SceneReferencer>();
            sceneReferencer.GetComponent<Canvas>().enabled = true;
        }
    }
}