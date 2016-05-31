using UnityEngine;
using System.Collections;

public partial class Actor
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Position, 1);
    }
}
