using UnityEngine;

public class MultipleMeshRendererController : MonoBehaviour {

    MeshRenderer[] MR;
    public bool EnableMeshes = true;
    private void Awake()
    {
        MR = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshrend in MR)
            meshrend.enabled = EnableMeshes;
    }
}
