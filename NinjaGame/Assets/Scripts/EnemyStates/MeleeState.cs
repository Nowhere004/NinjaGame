using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : IEnemyState
{
    private Enemy enemy;
    private float attacktimer;
    private float attackCooldown = 3;
    private bool canAttack = true;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        Attack();
        if (enemy.InThrowRange && !enemy.InMeleeRange)
        {
            enemy.ChangeState(new RangeState());
        }
        else if (enemy.Target==null)
        {
            enemy.ChangeState(new IdleState());
        }
    }

    public void Exit()
    {
      
    }

    public void OnTriggerEnter(Collider2D other)
    {
       
    }
    private void Attack()
    {
        attacktimer += Time.deltaTime;
        if (attacktimer >= attackCooldown)
        {
            canAttack = true;
            attacktimer = 0;
        }
        if (canAttack)
        {
            canAttack = false;
            enemy.MyAnimator.SetTrigger("attack");
        }

    }

}
