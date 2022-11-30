using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricPlayerAbility1 : MonoBehaviour
{
    // func initiate(a):
    //     global_position = a;
    //     src_pos = a;
    //     dest_pos.x = src_pos.x + 50*dir.x;
    //     dest_pos.y = src_pos.y + 50*dir.y;
    //     if dest_pos.x > src_pos.x:
    //         offset_mid = (dest_pos - src_pos)/2;
    //     else:
    //         offset_mid = (src_pos - dest_pos)/2;
    //     offset_mid.x += min(src_pos.x, dest_pos.x);
    //     offset_mid.y = min(src_pos.y, dest_pos.y) - 5*bounces*bounces*bounces;
    //     $erp_timer.start();
    //     set_physics_process(true);

    // func Bezier(t:float):
    //     var p1 = src_pos.linear_interpolate(offset_mid,t);
    //     var p2 = offset_mid.linear_interpolate(dest_pos,t);
    //     var pos =  p1.linear_interpolate(p2,t);
    //     return pos;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
