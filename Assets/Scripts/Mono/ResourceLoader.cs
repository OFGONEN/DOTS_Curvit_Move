using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    private void Awake()
    {
        Resources.Load("Prefabs/Line");
        Resources.Load("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
        Resources.Load("Materials/Mat_Line_Arrow_Bidirectional_Solid");
        Resources.Load("Materials/Mat_Line_Arrow_Common_Dashed");
        Resources.Load("Materials/Mat_Line_Arrow_Common_Solid");
    }
}
