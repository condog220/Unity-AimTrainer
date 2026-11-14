using System.Collections.Generic;
using UnityEngine;

public class AimTrainManager : MonoBehaviour
{
    public static AimTrainManager instance;

    public List<Transform> spawnList = new List<Transform>();
    public LayerMask layer;
    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioClip hitClip;
    [SerializeField] public GameObject spherePrefab;


    [SerializeField] private float fireRate = 0.2f;
    private float nextFire = 0f;

    private Camera cam;
    private GameMode currentGameMode;
    public List<GameObject> ActiveTargets { get; private set; } = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        SettingsManager.Changed += ApplySettings;
    }

    // Static event: unsubscribe, or it would keep calling this manager after the scene is gone.
    void OnDisable()
    {
        SettingsManager.Changed -= ApplySettings;
    }

    void Start()
    {
        cam = Camera.main;
        ApplySettings();
        if(GameModeSelect.currentMode != null)
        {
            SetScenario(GameModeSelect.currentMode);
        } else
        {
            SetScenario(new GridShot());
        }

    }

    // Update is called once per frame
    void Update()
    {
        currentGameMode.Update();

        // Countdown, pause and results: nothing fires or scores.
        if (GameManager.InputLocked)
        {
            return;
        }

        if (currentGameMode.AutoFire)
        {
            Track();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            DetectHit();
        }
    }

    // Also runs when a setting is changed from the pause menu.
    private void ApplySettings()
    {
        hitSound.volume = SettingsManager.volume;
        foreach (GameObject target in ActiveTargets)
        {
            if (target != null)
            {
                SettingsManager.ApplyTargetColor(target);
            }
        }
    }

    public void SetScenario(GameMode scenario)
    {
        if (currentGameMode != null)
        {
            currentGameMode.EndMode();
        }

        currentGameMode = scenario;
        currentGameMode.StartMode(this);
    }

    // Starts a fresh round of the current mode (EndMode clears targets, StartMode respawns them).
    public void RestartMode()
    {
        nextFire = 0f;
        SetScenario(currentGameMode);
    }


    private bool RaycastTarget(out RaycastHit hit)
    {
        return Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out hit, Mathf.Infinity, layer);
    }

    // Continuous modes: every frame the crosshair is on the target while fire is held counts as time on target.
    private void Track()
    {
        RaycastHit hit = default;
        bool onTarget = Input.GetMouseButton(0) && RaycastTarget(out hit);
        ScoreManager.instance.AddTrackingTime(Time.deltaTime, onTarget);
        if (!onTarget)
        {
            return;
        }

        currentGameMode.HandleHit(hit.collider.gameObject);

        // Sound and hit marker tick at the fire rate, not every frame.
        if (Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            hitSound.PlayOneShot(hitClip);
            if (Crosshair.instance != null)
            {
                Crosshair.instance.Flash();
            }
        }
    }

    private void DetectHit()
    {
        bool rayHit = RaycastTarget(out RaycastHit hit);

        if (rayHit)
        {
            hitSound.PlayOneShot(hitClip);
            currentGameMode.HandleHit(hit.collider.gameObject);
            ScoreManager.instance.updateScore();
            ScoreManager.instance.registerShot(true);
            if (Crosshair.instance != null)
            {
                Crosshair.instance.Flash();
            }
        }
        else
        {
            ScoreManager.instance.registerShot(false);
        }

    }
}
