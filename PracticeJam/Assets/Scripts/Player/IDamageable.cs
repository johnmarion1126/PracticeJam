using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    IEnumerator takeDamage(int amount);
}
