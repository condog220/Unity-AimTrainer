using UnityEngine;

public class Tracking : GameMode
{

    private AimTrainManager manager;
    private int maxSpheres = 1;
    private const float SecondsToKill = 3f;
    // Seconds of tracking the current target has left.
    private float health;
    private float minX = -4f;
    private float maxX = 4f;
    private float minY = 2f;
    private float maxY = 6f;

    public bool AutoFire => true;
    public bool UsesMovement => false;

    public void StartMode(AimTrainManager manager)
    {
        this.manager = manager;
        health = SecondsToKill;

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

    // Called every frame the crosshair is on the target while firing, so health drains in real time.
    public void HandleHit(GameObject target)
    {
        health -= Time.deltaTime;

        if(health <= 0f)
        {
            manager.ActiveTargets.Remove(target);
            Object.Destroy(target);
            health = SecondsToKill;
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
        SettingsManager.ApplyTargetColor(gameObject);
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
