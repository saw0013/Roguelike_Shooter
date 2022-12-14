using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator playerAnimator;
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void anim_Walk(bool isWalking)
    {
        playerAnimator.SetBool("walk", isWalking);
    }

    public void anim_Attack(bool canAttack, int AttackIndex)
    {
        playerAnimator.SetBool($"attack{AttackIndex}", canAttack);
    }

    public void anim_Dead(bool isDead)
    {
        playerAnimator.SetBool("dead", isDead);
    }

    public void anim_WalkSpeed(float speed)
    {
        playerAnimator.SetFloat("SpeedWalk", speed);
    }
}
