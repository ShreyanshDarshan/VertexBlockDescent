using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

public class VBDDistanceConstraint : VBDConstraint
{
    [SerializeField] float springConstant;
    [SerializeField] float restLength;
    [SerializeField] GameObject springPrefab;
    [SerializeField] GameObject planePrefab;
    GameObject springPrefabInstance;
    GameObject planePrefabInstance;
    Material planeMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        springPrefabInstance = Instantiate(springPrefab, transform);
        planePrefabInstance = Instantiate(planePrefab, transform);
        planeMaterial = planePrefabInstance.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        SetSpringMeshProperties();
        SetPlaneMeshProperties(points[0], points[1]);
    }

    void SetSpringMeshProperties()
    {
        springPrefabInstance.transform.position = points[0].transform.position;
        springPrefabInstance.transform.LookAt(points[1].transform.position);
        springPrefabInstance.transform.localScale = new Vector3(3, 3, Vector3.Distance(points[0].transform.position, points[1].transform.position));
    }

    void SetPlaneMeshProperties(VBDPoint p1, VBDPoint p2)
    {
        planePrefabInstance.transform.position = (p1.transform.position + p2.transform.position) / 2;
        planePrefabInstance.transform.LookAt(p2.transform.position);
        float distance = Vector3.Distance(p1.transform.position, p2.transform.position);
        planePrefabInstance.transform.localScale = new Vector3(distance, 1, distance);
        planeMaterial.SetVector("_Position", p1.transform.position);
        planeMaterial.SetFloat("_Mass", p1.mass);
        planeMaterial.SetFloat("_SpringConstant", springConstant);
        planeMaterial.SetFloat("_DeltaTime", VBDSolver.deltaTime);
        planeMaterial.SetVector("_SpringPosition", p2.transform.position);
        planeMaterial.SetFloat("_SpringLength", restLength);

        Mesh mesh = planePrefabInstance.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        planePrefabInstance.transform.TransformPoints(vertices);
        float[] energies = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            energies[i] = CalculateEnergy(vertices[i], points[0], points[1]);
        }

        float maxEnergy = Mathf.Max(energies);
        planeMaterial.SetFloat("_ScaleConstant", maxEnergy);
    }

    float CalculateEnergy(Vector3 point, VBDPoint p1, VBDPoint p2)
    {
        float distance = Vector3.Distance(p1.transform.position, p2.transform.position);
        float springForce = springConstant * (distance - restLength);
        float spring_energy = 0.5f * springForce * (distance - restLength);
        float momentum_energy = 0.5f * p1.mass * (point - p1.transform.position).sqrMagnitude / VBDSolver.deltaTime / VBDSolver.deltaTime;
        return spring_energy + momentum_energy;
    }
}
