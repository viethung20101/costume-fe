using UnityEngine;

public class ModelSkinRendererAsianFemale : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] Mesh mesh;
    [SerializeField] string[] Empty;
    [SerializeField] private float weigth;
    [SerializeField] private float Growbigger;
    [SerializeField] private float[] Limit;
    private void Awake()
    {
        
    }
    void Start()
    {
       skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();    
       mesh = skinnedMeshRenderer.sharedMesh;
    }

  
    void Update()
    {
        if(Growbigger < Limit[0])
        {
           Growbigger += 0.3f;
        }

        
       
    }
    public void GetBlendShape(string ShapeName, float weigth)
    {
        int index = mesh.GetBlendShapeIndex(ShapeName);
        if(index >= 0)
        {
                skinnedMeshRenderer.SetBlendShapeWeight(index, weigth);
        }
        else
        {
            Debug.Log("Không tìm thấy BlendShape mà bạn tìm");
        }
    }
    public void Look()
    {
         GetBlendShape(Empty[1], 100);
         GetBlendShape(Empty[3], 100);
          GetBlendShape(Empty[132], 100);
          GetBlendShape(Empty[4], 100);
    }
    public void CloseLook()
    {
         GetBlendShape(Empty[1], 100);
         GetBlendShape(Empty[3], 100);
          GetBlendShape(Empty[132], 100);
        GetBlendShape(Empty[4], 50);
    }
}
