using UnityEngine;
using UnityEngine.EventSystems;

using RPG.Character;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {

        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int WALKABLE_LAYER = 8;
        float maxRaycastDepth = 100f; // Hard coded value

        Rect screenSize = new Rect(); 

        public delegate void OnMouseOverTerrain(Vector3 destination);
        public event OnMouseOverTerrain onMouseOverWalkableLayer;

        public delegate void OnMouseOverEnemy(EnemyAI enemy);
        public event OnMouseOverEnemy onMouseOverEnemyLayer;


        void Update()
        {
            screenSize = new Rect(0, 0, Screen.width, Screen.height);
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {

            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            if (screenSize.Contains(Input.mousePosition)) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (RaycastForEnemy(ray)) { return; }
                if (RaycastForWalkable(ray)) { return; }
            }

        }

        private bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask walkableLayer = 1 << WALKABLE_LAYER;
            bool walkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, walkableLayer);
            if(walkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverWalkableLayer(hitInfo.point);
                return true;
            }
            return false;
        }

        private bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<EnemyAI>();
            if(enemyHit)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemyLayer(enemyHit);
                return true;
            }
            return false;
        }

    }
}