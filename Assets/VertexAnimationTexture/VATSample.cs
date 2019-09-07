using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public class VATSample : MonoBehaviour
    {
        [SerializeField]
        float _radius = 2;
        [SerializeField]
        float _force = 2;


        public void ApplyForce()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody rigidbody = colliders[i].GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(_force, transform.position, _radius);
                }
            }
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}