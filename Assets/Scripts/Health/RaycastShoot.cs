using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// This class triggers a Damaged event.
namespace Examples.Observer
{
    public class RaycastShoot : MonoBehaviour
    {
        [Header("Stats")]
        public int damage = 5;
        public float reloadTime = 0.8f;
        public float range = 5;
        public float beamDuration = 0.25f;

        private RaycastHit2D hit;
        private LineController lineController = null;
        private Vector3[] positions;
        private float timeToShoot = 0;
        private int layerMask;
        
        void Awake()
        {
            // Riferimento al LineController figlio
            if (!lineController)
                lineController = transform.GetComponentInChildren<LineController>();

            positions = new Vector3[2];
            layerMask = LayerMask.GetMask("ArrowHittable");
        }

        void Update()
        {
            timeToShoot -= Time.deltaTime;
        }

        public void TryShoot(Vector2 direction)
        {
            if (timeToShoot > 0) return; // Non pronto a sparare di nuovo
            Shoot(direction);
        }

        private void Shoot(Vector3 direction)
        {
            timeToShoot = reloadTime;   // intervallo tra i colpi
            positions[0] = transform.position;
            
            // Raycast 2D
            Vector3 normalizedDirection = direction.normalized;
            hit = Physics2D.Raycast(transform.position, normalizedDirection, range, layerMask);

            if (hit)
            {
                positions[1] = hit.transform.position;

                Health health = hit.transform.GetComponent<Health>();
                if (health)
                {
                    health.TakeDamage(damage);
                    SFXManager.Instance.PlayPlayerHit();
                }
                    
                else
                    Debug.LogError("Target has no Health component!");
            }
            else
            {
                positions[1] = transform.position + direction * range;
            }

            // Disegna il raggio
            lineController.DrawLine(positions[0], positions[1], beamDuration);
        }

        public void SetLayerMask(LayerMask mask)
        {
            layerMask = mask;
        }
    }
}
