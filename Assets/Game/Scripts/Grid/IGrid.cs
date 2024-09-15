using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grids
{
    public interface IGrid
    {
        int Width { get; }
        int Height { get; }
        Vector3 SpawnedPosition { get; }
        Vector3 GetGridMidPoint(Vector3 worldPos);
        void SetValue(Vector3 worldPosition, int value);
        float GetValueWithPos(Vector3 worldPosition);
        int GetValue(int x, int y);
        void DebugGridValues();
        void GetXYValuesOutside(Vector3 worldPosition, out int x, out int y);
        void GetXYValuesBuilding(Vector3 worldPosition, out int x, out int y);
    }

    public interface IGridObject
    {
        void OnPlace(Vector3 position, IGrid grid, GameObject obj);
        void OnHover(Vector3 position, IGrid grid);
    }

    public interface IBuilding : IGridObject
    {
        void TakeDamage(int damage);
        void ExecuteObjective();
    }
}