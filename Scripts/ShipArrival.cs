using UnityEngine;

public class ShipArrival : MonoBehaviour
{
    public Transform startPosition; // Punto inicial (lejos)
    public Transform endPosition;   // Punto final (objetivo)
    public Transform controlPoint;  // Punto intermedio para la curva
    public float arrivalTime = 3f;  // Tiempo que tarda en llegar

    private float elapsedTime = 0f;
    private bool isArriving = false;

    void Start()
    {
        transform.position = startPosition.position; // Inicia lejos
        isArriving = true;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (isArriving)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / arrivalTime); // Normalizar el tiempo

            // Interpolación de Bézier cuadrática
            Vector3 p0 = startPosition.position;
            Vector3 p1 = controlPoint.position;
            Vector3 p2 = endPosition.position;
            transform.position = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;

            // Rotación opcional hacia la dirección de movimiento
            Vector3 direction = (p2 - transform.position).normalized;
            if (direction != Vector3.zero)
                transform.forward = direction;

            // Llegada
            if (t >= 1f)
            {
                isArriving = false;
            }
        }
    }
}
