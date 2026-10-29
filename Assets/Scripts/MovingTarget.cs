using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    private const float Bound = 4f;

    private float direction = 1f; // 1 for right, -1 for left
    private float timer = 0f;
    private float changeInterval;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changeInterval = Random.Range(0.5f, 4f);

    }

    // Update is called once per frame
    void Update()
    {
        float xValue = transform.position.x + direction * Time.deltaTime * 3f;

        timer += Time.deltaTime;

        if(timer >= changeInterval)
        {
            direction *= -1f;

            timer = 0f;
            changeInterval = Random.Range(0.5f, 4f);

        }

        // Turn back at the arena edge so the target can't leave the playable area.
        if (xValue <= -Bound || xValue >= Bound)
        {
            xValue = Mathf.Clamp(xValue, -Bound, Bound);
            direction = -Mathf.Sign(xValue);
        }

        transform.position = new Vector3(xValue, transform.position.y, transform.position.z);

    }

    public void Initialize(float inpDirection)
    {
        direction = inpDirection;
    }
}
