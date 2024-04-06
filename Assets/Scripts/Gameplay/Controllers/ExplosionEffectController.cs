using UnityEngine;

public class ExplosionEffectController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayExplosion()
    {
        animator.SetTrigger("Explode");
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
