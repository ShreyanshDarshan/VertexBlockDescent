using UnityEngine;

public class VBDDistanceConstraint : VBDConstraint
{
    [SerializeField] GameObject springPrefab;
    [SerializeField] GameObject springPrefabInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        springPrefabInstance = Instantiate(springPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        springPrefabInstance.transform.position = points[0].transform.position;
        springPrefabInstance.transform.LookAt(points[1].transform.position);
        springPrefabInstance.transform.localScale = new Vector3(3, 3, Vector3.Distance(points[0].transform.position, points[1].transform.position));
    }
}
