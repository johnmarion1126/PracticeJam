using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : EnemyMovement
{
    public override void Update() {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        movement = direction;

        if (isDead) {
            dialog.addDialog("Mac: I don't get paid enough for this...");
        }
        if (damaged < damagedDuration) damaged += Time.deltaTime;
        if (inRange && !inAction && damaged >= damagedDuration) StartCoroutine(delayCall());
        checkInRange();
    }

}
