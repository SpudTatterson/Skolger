using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Skolger.Tutorial
{
    public class UIPointerVisualAid : VisualAid, IRaycastCellHelperUser
    {
        [SerializeField] GameObject pointer;
        [SerializeField] List<Transform> transforms;
        [SerializeField, InlineButton("StartSelecting")] List<Vector3> positions;

        [SerializeField] Vector3 onScreenOffset = new Vector3(50, 50, 0);
        [SerializeField] float lerpSpeed = 5f;

        [SerializeField] float paddingX = 50f;
        [SerializeField] float paddingY = 50f;
        [SerializeField] float lockDistance = 10f;
        Vector3 offScreenPosition;
        Camera mainCamera;

        public override void Initialize()
        {
            pointer?.SetActive(true);
            mainCamera = Camera.main;
            offScreenPosition = pointer.transform.position;

            foreach (Transform t in transforms)
            {
                positions.Add(t.position);
            }
        }

        public override void Reset()
        {
            pointer.transform.position = offScreenPosition;
            pointer?.SetActive(false);
        }

        void StartSelecting()
        {
            RaycastCellHelper.StartEditModeRaycast(this);
        }

        public void SetCells(Cell[] cells)
        {
            cells.ForEach(c => positions.Add(c.position));
        }

        public override void Update()
        {
            if (pointer == null || mainCamera == null)
                return;

            Vector3 center = VectorUtility.CalculateCenter(positions);
            Vector3 screenCenter = mainCamera.WorldToScreenPoint(center);

            // Check if the object is behind the camera (negative z value)
            if (screenCenter.z < 0)
            {
                // Flip the screen center horizontally and vertically
                screenCenter.x = Screen.width - screenCenter.x;
                screenCenter.y = Screen.height - screenCenter.y;
            }

            bool isOnScreen = screenCenter.x >= paddingX && screenCenter.x <= Screen.width - paddingX &&
                  screenCenter.y >= paddingY && screenCenter.y <= Screen.height - paddingY &&
                  screenCenter.z >= 0;

            Vector3 targetPosition = isOnScreen ? screenCenter + onScreenOffset : offScreenPosition;

            if (Vector3.Distance(targetPosition, pointer.transform.position) >= lockDistance)
                pointer.transform.position = Vector3.Lerp(pointer.transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            else
            {
                if (targetPosition != offScreenPosition)
                    pointer.transform.position = targetPosition;
            }

            Vector3 pointerScreenPos = pointer.transform.position;
            Vector3 direction = screenCenter - pointerScreenPos;

            // Calculate the angle based on the direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            pointer.transform.rotation = targetRotation;
        }
    }
}
