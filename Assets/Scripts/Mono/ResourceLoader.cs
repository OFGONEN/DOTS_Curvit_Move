using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    private void Awake()
    {
        //Load Materials
        Resources.Load("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
        Resources.Load("Materials/Mat_Line_Arrow_Bidirectional_Solid");
        Resources.Load("Materials/Mat_Line_Arrow_Common_Dashed");
        Resources.Load("Materials/Mat_Line_Arrow_Common_Solid");
        Resources.Load("Materials/MAT_Node");
        Resources.Load("Materials/MAT_Lanelet");

        //Load Prefabs
        Resources.Load("Prefabs/Line");
        
        //Load Mesh Assets
        Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        Resources.GetBuiltinResource<Mesh>("Plane.fbx");
    }
}
