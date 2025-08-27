using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class VBDPoint : MonoBehaviour
{
    public float mass;
    public Vector3 velocity = new Vector3(0, 0, 0);
    public Vector3 prevPosition = new Vector3(0, 0, 0);
    // Vector3 position = new Vector3(0, 0, 0);

    public GameObject futureGhost;
    public GameObject pastGhost;
    public Material ghostMaterial;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VBDSolver.AddPoint(this);
        prevPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DrawGhost();
    }

    public void UpdateState(float deltaTime)
    {
        prevPosition = transform.position;
        transform.position = CalculatePosition(prevPosition, deltaTime);
        velocity += VBDSolver.gravity * deltaTime; // Apply gravity to the velocity
    }

    public Vector3 CalculatePosition(Vector3 position, float deltaTime)
    {
        return position + velocity * deltaTime + VBDSolver.gravity * deltaTime * deltaTime * 0.5f;
    }

    void DrawGhost()
    {
        if (futureGhost == null)
        {
            futureGhost = Instantiate(gameObject);
            futureGhost.transform.SetParent(transform); // Set the parent to match the original object
            futureGhost.transform.localScale = Vector3.one; // Match the scale of the original object
            futureGhost.GetComponent<VBDPoint>().enabled = false; // Disable the script on the ghost object
            futureGhost.GetComponent<Renderer>().material = ghostMaterial;
            Color selfColor = GetComponent<Renderer>().material.color;
            Color ghostColor = new Color(selfColor.r, selfColor.g, selfColor.b, 0.1f); // Semi-transparent ghost color
            futureGhost.GetComponent<Renderer>().material.color = ghostColor;
        }

        if (pastGhost == null)
        {
            pastGhost = Instantiate(gameObject);
            pastGhost.transform.SetParent(transform); // Set the parent to match the original object
            pastGhost.transform.localScale = Vector3.one; // Match the scale of the original object
            pastGhost.GetComponent<VBDPoint>().enabled = false; // Disable the script on the ghost object
            pastGhost.GetComponent<Renderer>().material = ghostMaterial;
            Color selfColor = GetComponent<Renderer>().material.color;
            Color ghostColor = new Color(selfColor.r, selfColor.g, selfColor.b, 0.1f); // Semi-transparent ghost color
            pastGhost.GetComponent<Renderer>().material.color = ghostColor;
        }

        pastGhost.transform.position = prevPosition;
        Vector3 projectedPos = CalculatePosition(transform.position, VBDSolver.deltaTime);
        futureGhost.transform.position = projectedPos;
    }

    public float MomentumEnergy(Vector3 point)
    {
        return 0.5f * mass * (point - transform.position).sqrMagnitude / VBDSolver.deltaTime / VBDSolver.deltaTime;
    }
}