using UnityEngine;

public class ParentSkinMeshRenderer : MonoBehaviour
{
    [SerializeField] ModelSkinRendererAsianFemale  modelSkinRendererAsianFemale;
    public Animator animator;
    void Start()
    {
        
    }
   void Update()
   {
     
   }
   public void Look()
   {
    modelSkinRendererAsianFemale.Look();
   }
   public void CloseLook()
   {
    modelSkinRendererAsianFemale.CloseLook();
   }
}
