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

    public void anim_WalkSpider(bool isWalking)
    {
        playerAnimator.SetBool("walk", isWalking);
    }

    public void anim_WalkSolider(Vector3 desiredDirection)
    {
        if(desiredDirection.magnitude > 0.05)
        {
            Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
            float forw = Vector3.Dot(movement, transform.forward);
            float stra = Vector3.Dot(movement, transform.right);

            playerAnimator.SetFloat("Forward", forw);
            playerAnimator.SetFloat("Strafe", stra);
        }
        else
        {
            playerAnimator.SetFloat("Forward", 0);
            playerAnimator.SetFloat("Strafe", 0);
        }
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
        if(playerAnimator)
            playerAnimator.SetFloat("SpeedWalk", speed);
    }
}
