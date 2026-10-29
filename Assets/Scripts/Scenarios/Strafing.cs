using UnityEngine;

public class Strafing : GameMode
{

    private AimTrainManager manager;
    private int maxSpheres = 1;

    private float minX = -4f;
    private float maxX = 4f;
    private float minY = 2f;
    private float maxY = 6f;

    private Vector3 lastSpawnedPosition = Vector3.positiveInfinity;

    public bool AutoFire => false;
    public bool UsesMovement => true;

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
        manager.ActiveTargets.Remove(target);
        Object.Destroy(target);

        if (manager.ActiveTargets.Count < maxSpheres)
        {
            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
        Vector3 spawnPosition;

        if (manager.ActiveTargets.Count >= maxSpheres )
        {
            return;
        }

        do
        {
            spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 5f);
        }
        while(spawnPosition == lastSpawnedPosition);
        lastSpawnedPosition = spawnPosition;

        GameObject gameObject = Object.Instantiate(manager.spherePrefab, spawnPosition, Quaternion.identity);
        if (SettingsManager.randomSize)
        {
            float randScale = Random.Range(0.5f, 1.5f);
            gameObject.transform.localScale = new Vector3(randScale, randScale, randScale);
        }
        manager.ActiveTargets.Add(gameObject);
    }

    public void EndMode()
    {
        if (manager == null)
        {
            return;
        }

        foreach (var target in manager.ActiveTargets)
        {
            Object.Destroy(target);
        }
        manager.ActiveTargets.Clear();
    }
}
