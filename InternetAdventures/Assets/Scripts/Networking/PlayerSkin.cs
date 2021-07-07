using Mirror;
using UnityEngine;

namespace Networking
{
    public class PlayerSkin : NetworkBehaviour
    {
        [SerializeField, SyncVar(hook = nameof(ChangeSkinIndexClient))] private int skinIndex = 0;

        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        
        [SerializeField] private Material playerOneMaterialHead;
        [SerializeField] private Material playerOneMaterialFace;
        [SerializeField] private Material playerOneMaterialBody;
        
        [SerializeField] private Material playerTwoMaterialHead;
        [SerializeField] private Material playerTwoMaterialFace;
        [SerializeField] private Material playerTwoMaterialBody;

        [ServerCallback]
        public void ChangeSkinIndex(int pValue)
        {
            skinIndex = pValue;
            ChangeSkin(pValue);
        }

        [ClientCallback]
        private void ChangeSkinIndexClient(int oldValue, int newValue)
        {
            ChangeSkin(newValue);
        }

        private void ChangeSkin(int pValue)
        {
            switch (pValue)
            {
                case 1:
                {
                    Material[] mats = new Material[] {playerOneMaterialHead, playerOneMaterialFace, playerOneMaterialBody};
                    meshRenderer.materials = mats;
                    break;
                }
                case 2:
                {
                    Material[] mats = new Material[] {playerTwoMaterialHead, playerTwoMaterialFace, playerTwoMaterialBody};
                    meshRenderer.materials = mats;
                    break;
                }
            }
        }
    }
}