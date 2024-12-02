
using UnityEngine;

public class WindController : MonoBehaviour
{
    public float minInterval = 5f;
    public float maxInterval = 15f;
    public float windForceMin = 2f;
    public float windForceMax = 10f;

    private float timer;
    private bool isWindActive = false;
    private Vector2 windDirection;
    private float windForce;

    void Start()
    {
        SetNextWind();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && !isWindActive)
        {
            ActivateWind();
        }
        else if (isWindActive)
        {
            float windDuration = 3f;
            windDuration -= Time.deltaTime;
            if (windDuration <= 0)
            {
                DeactivateWind();
                SetNextWind();
            }
            else
            {
                ApplyWind();
            }
        }
    }

    void SetNextWind()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    void ActivateWind()
    {
        isWindActive = true;
        float angle = Random.Range(0, 360);
        windDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        windForce = Random.Range(windForceMin, windForceMax);
    }

    void DeactivateWind()
    {
        isWindActive = false;
    }

    void ApplyWind()
    {
        CatController.Instance.ApplyWind(windDirection, windForce);
    }
}
