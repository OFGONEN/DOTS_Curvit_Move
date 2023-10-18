using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    private void Awake()
    {
        Resources.LoadAsync("Prefabs/Line");
        Resources.LoadAsync("Materials/MAT_Lanelet");
        Resources.LoadAsync("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
        Resources.LoadAsync("Materials/Mat_Line_Arrow_Bidirectional_Solid");
        Resources.LoadAsync("Materials/Mat_Line_Arrow_Common_Dashed");
        Resources.LoadAsync("Materials/Mat_Line_Arrow_Common_Solid");
    }
}
