
using UnityEngine;

namespace Tensori.LowPolyFarm
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class FruitCrateRigidbodyHit : MonoBehaviour
    {
        private Rigidbody[] allRigidbodies = null;

        private void Start()
        {
            allRigidbodies = GetComponentsInChildren<Rigidbody>();
            RigidbodyState(false);
        }

        private void RigidbodyState(bool state)
        {
            if (allRigidbodies == null || allRigidbodies.Length == 0)
                return;

            foreach (var rb in allRigidbodies)
            {
                rb.isKinematic = !state;
                rb.useGravity = state;
            }
        }

        private void OnCollisionEnter(Collision other)
            => RigidbodyState(true);
    }
}
