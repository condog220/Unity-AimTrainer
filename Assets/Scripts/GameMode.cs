using UnityEngine;

public interface GameMode
{
    // Hold-to-fire at the manager's fire rate instead of one shot per click.
    bool AutoFire { get; }
    // Whether the player strafes in this mode.
    bool UsesMovement { get; }

    void StartMode(AimTrainManager manager);
    void EndMode();

    void Update();

    // Owns removing the target from ActiveTargets and destroying it.
    void HandleHit(GameObject target);
}
