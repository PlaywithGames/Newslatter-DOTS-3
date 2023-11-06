using Unity.Mathematics;
using UnityEngine;

public class IntVectorExample : MonoBehaviour
{
    void Start()
    {
        // 벡터 생성 (x, y, z, w)
        int4 i4 = new int4(1, 2, 3, 4);
        
        // new int2(0, 0);
        int2 i2 = int2.zero;                    
        
        // 배열과 동일하게 사용할 수 있습니다.
        int i = i4[2];
        i4[0] = 9;
        
        // 벡터 셔플링(swizzing)
        i4 = i4.xwyy; // new int4(i4.x, i4.w, i4.y, i4.y);
        i2 = i4.yz;   // new int2(i4.y, i4.z);
        i4 = i2.yxyx; // new int4(i2.y, i2.x, i2.y, i2.x);    
        
        i4 = new int4(1, i2, 3); // new int4(1, i2.x, i2.y, 3);           
        i4 = new int4(i2, i2); // new int4(i2.x, i2.y, i2.x, i2.y);
        i4 = new int4(7); // new int4(7, 7, 7, 7);
        i2 = new int2(7.5f); // new int2((int) 7.5f, (int) 7.5f);                 
        
        // 형 변환
        i4 = (int4) 7; // new int4(7, 7, 7, 7);                         
        i2 = (int2) 7.5f; // new int2((int) 7.5f, (int) 7.5f);
    }
}

