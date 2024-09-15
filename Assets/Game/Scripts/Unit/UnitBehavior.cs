using System.Collections;
using System.Collections.Generic;
using CanDurmazLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace Unit
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class UnitBehavior : MonoBehaviour
    {
        public HealthBar healthBar;
        public SpriteRenderer areaIndicator;

        //Unit values.
        private float maxHealth;
        private float health;
        private float moveSpeed;
        private float damage;
        private float range;

        //Used values.
        private Vector3 targetPosition;
        private List<Vector3> path;
        private int currentPathIndex;
        private bool isSelected;
        private bool isMoving = false;
        private string returnName;
        Vector3 lastPos;

        private void Start()
        {
            isMoving = false;
            path = new List<Vector3>();
            targetPosition = transform.position;

        }
        public void OnEnable()
        {
            isMoving = false;
            if (areaIndicator != null)
            {
                areaIndicator.enabled = false;
            }
        }
        public void Initialize(UnitData unitData)
        {
            //Set unit values.
            this.health = unitData.health;
            this.maxHealth = unitData.health;
            this.moveSpeed = unitData.moveSpeed;
            this.damage = unitData.damage;
            this.range = unitData.range;
            this.returnName = unitData.unitType.ToString();

            lastPos = transform.localPosition;
            GameManager.Instance?.ClearOldAndSetNewPositionSoldier(Vector3.zero, lastPos);
            healthBar?.Initialize(maxHealth);
            UpdateHealthBar();
            ResizeAreaIndicator();
        }
        private void UpdateHealthBar()
        {
            healthBar?.UpdateHealth(health);
        }
        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            if (health <= 0)
            {
                health = 0;
                DestroyUnit();
            }
            UpdateHealthBar();
        }
        private void DestroyUnit()
        {
            healthBar?.StopCoroutines();
            GameManager.Instance.SoldierDied(lastPos);
            GameManager.Instance.RemoveUnit(this);
            ObjectPooler.Instance.ReturnObjectToPool(this.gameObject, returnName);
        }
        public void SelectUnit()
        {
            isSelected = true;
            GetComponent<SpriteRenderer>().color = Color.green;  //Highlight unit.

            if (areaIndicator != null)
            {
                areaIndicator.enabled = true;
            }
        }
        public void DeselectUnit()
        {
            isSelected = false;
            GetComponent<SpriteRenderer>().color = Color.white;  //Remove highlight.

            if (areaIndicator != null)
            {
                areaIndicator.enabled = false;
            }
        }
        private void Update()
        {
            if (isSelected && Input.GetMouseButtonDown(1))  // Right-click to move
            {
                Vector3 clickedPosition = GetMouseWorldPosition();
                RequestPath(clickedPosition);
                if (!isMoving)
                {
                    //Attack logic.
                    AttackTarget();
                }
            }

            if (path != null && currentPathIndex < path.Count)
            {
                MoveAlongPath();
            }
        }

        private void RequestPath(Vector3 targetWorldPosition)
        {
            var newPath = GameManager.Instance.RequestPath(transform.position, targetWorldPosition);
            if (newPath != null)
            {
                path = newPath;
                isMoving = true;
                Vector3 newPositionWorld = path[path.Count - 1];
                Vector3 newPositionLocal = transform.parent.InverseTransformPoint(newPositionWorld);
                GameManager.Instance?.ClearOldAndSetNewPositionSoldier(lastPos, newPositionLocal);
                lastPos = newPositionLocal;
                currentPathIndex = 0;
                foreach (var position in path)
                {
                    //Debug.Log($"Path position: {position}");
                }
            }
            else
            {
                Debug.LogWarning("No path found.");
            }
        }

        private void MoveAlongPath()
        {
            if (currentPathIndex >= path.Count)
                return;
            Vector3 currentTargetPosition = path[currentPathIndex];

            float distanceToTarget = Vector3.Distance(transform.position, currentTargetPosition);

            if (distanceToTarget > 0.05f) //Check if really close.
            {
                Vector3 moveDirection = (currentTargetPosition - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {
                transform.position = currentTargetPosition;
                currentPathIndex++; //Move to next path.

                if (currentPathIndex >= path.Count)
                {
                    isMoving = false;
                }
            }
        }
        private void AttackTarget()
        {
            if (isMoving)
                return;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                UnitBehavior clickedUnit = hit.collider.GetComponent<UnitBehavior>();
                if (clickedUnit != null && clickedUnit != this)
                {
                    float distanceToClickedUnit = Vector3.Distance(transform.position, hit.point);
                    if (distanceToClickedUnit <= range)
                    {
                        clickedUnit.TakeDamage(damage);
                        //Debug.Log($"Dealt {damage} damage to {clickedUnit.name}");
                    }
                }
                Build.BuildingOnGrid buildingOnGrid = hit.collider.GetComponent<Build.BuildingOnGrid>();
                if (buildingOnGrid != null && buildingOnGrid != this)
                {
                    float distanceToClickedUnit = Vector3.Distance(transform.position, hit.point);
                    if (distanceToClickedUnit <= range)
                    {
                        buildingOnGrid.BuildingBehavior.TakeDamage(((int)damage));
                        //Debug.Log($"Dealt {damage} damage to {buildingOnGrid.name}");
                    }
                }
            }
        }
        private void ResizeAreaIndicator()
        {
            if (areaIndicator != null)
            {
                float scale = range * 1.27f; //1.27 comes from perfect fit of sprite inside prefab.
                areaIndicator.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }
        private void OnDrawGizmos()
        {
            if (path != null && path.Count > 0)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(path[i], path[i + 1], Color.red);  //Visualize path in sceneview.
                }
            }
        }

    }
}
