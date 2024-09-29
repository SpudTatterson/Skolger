using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Skolger.Tutorial
{
    public class GhostVisualAid : VisualAid, IRaycastCellHelperUser
    {
        [SerializeField] GameObject model;
        
        [SerializeField, InlineButton("StartSelecting")] Vector3 cellPos;
        GameObject tempObject;
        public override void Initialize()
        {
            Cell cell = GridManager.Instance.GetCellFromPosition(cellPos);
            tempObject = MonoBehaviour.Instantiate(model, cell.position, Quaternion.identity);
        }

        public override void Reset()
        {
            MonoBehaviour.Destroy(tempObject);
        }

        public override void Update()
        {

        }

        void StartSelecting()
        {
            RaycastCellHelper.StartEditModeRaycast(this);
        }

        public void SetCellPosition(Vector3 point)
        {
            cellPos = point;
        }
    }

}
