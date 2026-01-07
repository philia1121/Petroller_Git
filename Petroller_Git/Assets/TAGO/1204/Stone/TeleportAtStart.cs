using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAtStart : MonoBehaviour
{
    // 在 Inspector 面板中設定你想要的座標
    public Vector3 targetPosition = new Vector3(0f, 0f, 0f);



    void Start()
    {
        // 檢查是否有母物件
        if (transform.parent != null)
        {
            // 將相對座標設為 (0, 0, 0)，物件會瞬間移動到母物件的座標原點
            transform.localPosition = Vector3.zero+targetPosition;;
            
            // 如果你也希望旋轉角度跟母物件同步，可以加上這一行
            // transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("此物件沒有母物件，無法移動到母物件座標！");
        }
    }
}