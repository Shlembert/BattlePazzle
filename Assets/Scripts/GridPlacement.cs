using System.Collections.Generic;
using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    [SerializeField] private Transform minAnchor, maxAnchor;

    public List<Vector2> PlacePoints(int count)
    {
        List<Vector2> points = new List<Vector2>();

        int c = Mathf.FloorToInt(Mathf.Sqrt(count)) + 1;

        Vector2 fantamas = maxAnchor.position - minAnchor.position;

        Vector2 offset = fantamas / c;

        for (int x = 0; x < c; x++)
        {
            for (int y = 0; y < c; y++)
            {
                points.Add(new Vector2(x * offset.x + offset.x * 0.5f, y * offset.y + offset.y * 0.5f) + (Vector2)minAnchor.position);
            }
        }

        return points;
    }
}
