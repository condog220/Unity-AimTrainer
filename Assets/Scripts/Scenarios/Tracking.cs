using UnityEngine;

public class Tracking : GameMode
{

    private AimTrainManager manager;
    private int maxSpheres = 1;
    public int health = 100;
    private float minX = -4f;
    private float maxX = 4f;
    private float minY = 2f;
    private float maxY = 6f;

    public void StartMode(AimTrainManager manager)
    {
        this.manager = manager;

        for (int i = 0; i < maxSpheres; i++)
        {
            SpawnTarget();
        }
    }

    public void Update()
    {
        if (manager.ActiveTargets.Count < maxSpheres)
        {
            SpawnTarget();
        }
    }

    public void HandleHit(GameObject target)
    {
        health -= 20;

        if (manager.ActiveTargets.Count < maxSpheres)
        {
            SpawnTarget();
        }

        if(health <= 0)
        {
            manager.ActiveTargets.Remove(target);
            Object.Destroy(target);
            health = 100;
        }
    }

    private void SpawnTarget()
    {
        if (manager.ActiveTargets.Count >= maxSpheres)
        {
            return;
        }
        float temp = Random.Range(minX, maxX);

        Vector3 spawnPosition = new Vector3(temp, Random.Range(minY, maxY), 5f);
        GameObject gameObject = Object.Instantiate(manager.spherePrefab, spawnPosition, Quaternion.identity);
        if (SettingsManager.randomSize)
        {
            float randScale = Random.Range(0.5f, 1.5f);
            gameObject.transform.localScale = new Vector3(randScale, randScale, randScale);
        }
        manager.ActiveTargets.Add(gameObject);

        MovingTarget movingTarget = gameObject.AddComponent<MovingTarget>();
        movingTarget.Initialize(Random.Range(0, 2) == 0 ? -1f : 1f);

    }

    public void EndMode()
    {
        manager.ActiveTargets.Clear();
    }

}
