using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : Singleton<StarGenerator>
{
    private Vector2 minPosition;
    private Vector2 maxPosition;
    private float centerX;
    private float centerY;
    private float centralZoneRadius;
    private List<Vector2> generatedStarsPositions = new List<Vector2>();

    public  void Init(float minX, float maxX, float minY, float maxY, float centralZoneRadius)
    {
        this.minPosition = new Vector2(minX, minY);
        this.maxPosition = new Vector2(maxX, maxY);
        this.centerX = (minX + maxX) / 2f;
        this.centerY = (minY + maxY) / 2f;
        this.centralZoneRadius = centralZoneRadius;
    }

    public Vector2 GenerateStars()
    {
        Vector2 starPosition;
        do
        {
            starPosition = new Vector2(
                Random.Range(Mathf.RoundToInt(minPosition.x), Mathf.RoundToInt(maxPosition.x)),
                Random.Range(Mathf.RoundToInt(minPosition.y), Mathf.RoundToInt(maxPosition.y))
            );
        } while (IsInCentralZone(starPosition) || IsDuplicate(starPosition));

        generatedStarsPositions.Add(starPosition);
        return starPosition;
    }

    private bool IsInCentralZone(Vector2 position)
    {
        float distanceFromCenter = Vector2.Distance(position, new Vector2(centerX, centerY));
        return distanceFromCenter < centralZoneRadius;
    }

    private bool IsDuplicate(Vector2 position)
    {
        return generatedStarsPositions.Contains(position);
    }
}
