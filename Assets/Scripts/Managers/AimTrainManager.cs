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

    void Start()
    {
        cam = Camera.main;
        hitSound.volume = SettingsManager.volume;
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

        bool fire = currentGameMode.AutoFire
            ? Input.GetMouseButton(0) && Time.time > nextFire
            : Input.GetMouseButtonDown(0);

        if (fire)
        {
            nextFire = Time.time + fireRate;
            DetectHit();
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


    private void DetectHit()
    {
        if (PauseScript.isPaused)
        {
            return;
        }

        bool rayHit = Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)), out RaycastHit hit, Mathf.Infinity, layer);

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
