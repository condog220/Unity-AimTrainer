using UnityEngine;

public interface GameMode
{
    // Continuous mode: scored by time on target while fire is held, instead of one shot per click.
    bool AutoFire { get; }
    // Whether the player strafes in this mode.
    bool UsesMovement { get; }

    void StartMode(AimTrainManager manager);
    void EndMode();

    void Update();

    // Called once per hit, or every on-target frame in AutoFire modes.
    // Owns removing the target from ActiveTargets and destroying it.
    void HandleHit(GameObject target);
}
