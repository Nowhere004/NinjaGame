using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState {

    private float patrolTimer;
    private float patrolDuration;
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        patrolDuration = Random.Range(1,10);
        this.enemy = enemy;
    }

    public void Execute()
    {
        Debug.Log("Patrolling");
        Patrol();
        enemy.Move();
        if (enemy.Target!=null && enemy.InThrowRange)
        {
            enemy.ChangeState(new RangeState());
        }
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {

        if (other.tag == "Knife")
        {
            enemy.Target = Player.Instance.gameObject;
        }
    }

    private void Patrol()
    {
                
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            patrolTimer = 0f;
            enemy.ChangeState(new IdleState());
            
        }

    }
}
