using Unity.Mathematics;
using UnityEngine;

public class IntVectorOperationExample : MonoBehaviour
{
    void Start()
    {
        int2 a = new int2(1, 2);
        int2 b = new int2(3, 4);
        
        int2 c = a + b;               // new int2(a.x + b.x, a.y + b.y)
        c = -a;                       // new int2(-a.x, -a.y)
        
        bool myBool = a.Equals(b);    // a.x == b.x && a.y == b.y
        bool2 myBool2 = a == b;       // new int2(a.x == b.x, a.y == b.y)
        
        myBool2 = a > b;              // new bool2(a.x > b.x, a.y > b.y)
    }
}
