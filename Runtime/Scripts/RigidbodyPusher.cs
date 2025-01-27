using UnityEngine;

namespace SPDev 
{
    public class RigidbodyPusher : MonoBehaviour {
        public LayerMask pushLayers;
        public bool canPush;
        public float strength = 1.1f;

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (canPush) PushRigidBodies(hit);
        }

        private void PushRigidBodies(ControllerColliderHit hit) {

            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            // make sure we only push desired layer(s)
            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayers.value) == 0) return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

            body.AddForce(pushDir * strength, ForceMode.Impulse);
        }
    }
}
