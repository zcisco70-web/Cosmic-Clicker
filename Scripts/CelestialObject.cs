using UnityEngine;

public class CelestialObject : MonoBehaviour
{
    [Header("Tamaño y Escala")]
    
    public float currentSize = 1f; // Tamaño real del objeto
    public ulong massToEvolve = 10; // Tamaño necesario para evolucionar
    public float scaleFactor = 0.01f; // Factor para la escala visual

    [Header("Partículas")]
    public ParticleSystem clickParticles; // Sistema de partículas
    public float minParticleSpeed = 1f; // Velocidad inicial mínima de las partículas
    public float maxParticleSpeed = 10f; // Velocidad máxima de las partículas
    public float particleSpeedIncrement = 0.5f; // Incremento de velocidad por clic

    private float currentParticleSpeed; // Velocidad actual de las partículas
    private float timeSinceLastClick = 0f; // Tiempo transcurrido desde el último clic


    [Header("Rotación")]
    public float rotationSpeed = 0f; // Velocidad inicial de rotación
    public float maxRotationSpeed = 100f; // Velocidad máxima de rotación

    public float StartAcceleration = 5f; // Cuánto aumenta la velocidad con cada clic
    public float StartADeceleration = 10f; // Cuánto se desacelera cuando no se hace clic
    public float acceleration = 5f; // Cuánto aumenta la velocidad con cada clic
    public float deceleration = 10f; // Cuánto se desacelera cuando no se hace clic

    private bool isClicking = false; // Controla si el clic está activo
    [Header("Evolución")]
    public ulong evolveClickMass;
    public float evolvePassiveMass;

    void Start()
    {
        // Inicializar las partículas con la velocidad mínima
        currentParticleSpeed = minParticleSpeed;
        //SetParticleSpeed(currentParticleSpeed);
    }

    void Update()
    {
        // Gradualmente ralentizar las partículas si no hay clics recientes
        if (timeSinceLastClick > 0f)
        {
            timeSinceLastClick -= Time.deltaTime;
            if (timeSinceLastClick <= 0f)
            {
                StopParticles();
            }
        }

        if (isClicking)
        {
            rotationSpeed = Mathf.Min(rotationSpeed + acceleration * Time.deltaTime, maxRotationSpeed);
            acceleration += Time.deltaTime;
        }
        else
        {
            if (timeSinceLastClick <= 0f)
                if (rotationSpeed > 0f)
                {
                    rotationSpeed = Mathf.Max(rotationSpeed - deceleration * Time.deltaTime, 0f);
                    if (acceleration > StartAcceleration)
                        acceleration -= Time.deltaTime;
                    else 
                        acceleration = StartAcceleration;
                }
        }


        // Rotar el objeto basado en la velocidad
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        Vector2 clickPosition = Input.mousePosition;
        GameManager.Instance.AddSize(clickPosition);
        timeSinceLastClick = 0.5f;
        StartParticles();
        isClicking = true;
    }

    void OnMouseUp()
    {
        //StopParticles();
        isClicking = false;
    }

    void StartParticles()
    {
        //clickParticles.transform.position = gameObject.transform.position;
        if (!clickParticles.isPlaying)
        {
            clickParticles.Play();
        }
    }

    void StopParticles()
    {
        clickParticles.Stop();
        currentParticleSpeed = minParticleSpeed; // Reiniciar la velocidad de las partículas
        //SetParticleSpeed(currentParticleSpeed);
    }

    void SetParticleSpeed(float speed)
    {
        var mainModule = clickParticles.main;
        mainModule.simulationSpeed = speed;
    }

    public void Disappear()
    {
        if (GetComponent<DissolvingController>())
            GetComponent<DissolvingController>().Dissolve();
    }

    public void Appear()
    {
        if (GetComponent<DissolvingController>())
            GetComponent<DissolvingController>().Create();
    }
}