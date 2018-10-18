using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendMethods{

    /// <summary>
    /// OnDrawGizmos内でしか呼び出せない
    /// </summary>
    /// <param name="from">始点</param>
    /// <param name="direction">方向と長さ</param>
    public static void DrawAllow(Vector3 from, Vector3 direction)
    {
        const float AllowTopLength = 0.3f;
        const float AllowRadius = 20f;

        var toPosition = from + direction;

        var tmp = Quaternion.Euler(0, 0, AllowRadius) * -direction;
        tmp = tmp.normalized * AllowTopLength;
        Gizmos.DrawRay(toPosition, tmp);

        tmp = Quaternion.Euler(0, 0, -AllowRadius) * -direction;
        tmp = tmp.normalized * AllowTopLength;
        Gizmos.DrawRay(toPosition, tmp);

        Gizmos.DrawRay(from, direction);
    }
}
