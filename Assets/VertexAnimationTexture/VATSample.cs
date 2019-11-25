#pragma warning disable CS0649 //hide never assigned SerializeField warnings
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StoryProgramming
{
    public class VATSample : MonoBehaviour
    {
        [SerializeField]
        float _radius = 2;
        [SerializeField]
        float _force = 2;
        [SerializeField]
        UnityEvent _callWhenApplyForce;

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
            if (_callWhenApplyForce != null)
            {
                _callWhenApplyForce.Invoke();
            }
        }
        public void ApplyForceAndCallAction()
        {
            ApplyForce();
            if (_callWhenApplyForce != null)
            {
                _callWhenApplyForce.Invoke();
            }
        }



        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}