using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class VBDPoint : MonoBehaviour
{
    public Vector3 velocity = new Vector3(0, 0, 0);
    public Vector3 prevPosition = new Vector3(0, 0, 0);
    // Vector3 position = new Vector3(0, 0, 0);

    public GameObject ghost;
    public Material ghostMaterial;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VBDSolver.AddPoint(this);
    }

    // Update is called once per frame
    void Update()
    {
        DrawGhost();
    }

    public void UpdateState(float deltaTime)
    {
        prevPosition = transform.position;
        transform.position = prevPosition + velocity * VBDSolver.deltaTime;
    }

    void DrawGhost()
    {
        if (ghost == null)
        {
            ghost = Instantiate(gameObject);
            ghost.transform.SetParent(transform); // Set the parent to match the original object
            ghost.transform.localScale = Vector3.one; // Match the scale of the original object
            ghost.GetComponent<VBDPoint>().enabled = false; // Disable the script on the ghost object
            ghost.GetComponent<Renderer>().material = ghostMaterial;
            Color selfColor = GetComponent<Renderer>().material.color;
            Color ghostColor = new Color(selfColor.r, selfColor.g, selfColor.b, 0.1f); // Semi-transparent ghost color
            ghost.GetComponent<Renderer>().material.color = ghostColor;
        }
        Vector3 projectedPos = transform.position + (velocity * VBDSolver.deltaTime);
        ghost.transform.position = projectedPos;
    }
}