using UnityEngine;
public class RandomHelper
{
    public float maxSpawnX = 3.1f;
    public Vector2 RandomSpawnPos(int spawnCount, float yPos)
    {
        int selectedSpawn = Random.Range(0, spawnCount);
        float xPos = -maxSpawnX + (maxSpawnX * 2 / (spawnCount - 1) * selectedSpawn);
        return new Vector2(xPos, yPos);
    }
}
