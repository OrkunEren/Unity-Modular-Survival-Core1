using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerAttacker : MonoBehaviour
{
    public event Action OnAttackPerformed;

    [Header("Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private LayerMask damageableLayers; 
    [SerializeField] private Transform cameraRoot; 

    [Header("Input")]
    [SerializeField] private InputActionReference attackInput;

    private void OnEnable()
    {
        attackInput.action.Enable();
        attackInput.action.performed += TryAttack;
    }

    private void OnDisable()
    {
        attackInput.action.performed -= TryAttack;
        attackInput.action.Disable();
    }

    private void TryAttack(InputAction.CallbackContext context)
    {
        OnAttackPerformed?.Invoke();     
        DealDamage();
    }

    public void DealDamage() 
    {
        RaycastHit hit;


        if (Physics.Raycast(cameraRoot.position, cameraRoot.forward, out hit, attackRange, damageableLayers))
        {

            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);           
            }
        }
    }
  
    private void OnDrawGizmos()
    {
        if (cameraRoot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(cameraRoot.position, cameraRoot.forward * attackRange);
        }
    }
}