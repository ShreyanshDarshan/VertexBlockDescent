using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VBDSolver : MonoBehaviour
{
    // A static reference to the instance of the class.
    public static VBDSolver Instance { get; private set; }
    public static float deltaTime = 0.02f; // Default delta time for physics calculations
    public static Vector3 gravity = Vector3.down * 9.81f; // Default gravity vector
    public Vector3 _gravity = Vector3.zero; // Override gravity if needed
    public float _deltaTime = 0.02f; // Override delta time if needed
    public HashSet<VBDPoint> points = new HashSet<VBDPoint>();
    public bool canSimulate = false;
    public bool hasFinishedIteration = true;
    public IEnumerator physicsCoroutine;

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set this instance as the Singleton.
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gravity = _gravity;
        deltaTime = _deltaTime; // Use the overridden delta time if set
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canSimulate = true;
        }
        if (hasFinishedIteration) {
            StartCoroutine(UpdatePhysics());
        }
    }
    
    // void FixedUpdate()
    // {
    //     deltaTime = Time.fixedDeltaTime; // Update deltaTime to match the fixed update interval        
    //     foreach (var point in points)
    //     {
    //         point.UpdateState(deltaTime);
    //     }
    // }

    IEnumerator UpdatePhysics()
    {
        hasFinishedIteration = false;
        
        foreach (var point in points)
        {
            while (canSimulate == false) {
                yield return null; // Wait until canSimulate is true
            }
            canSimulate = false;
            point.UpdateState(deltaTime);
        }
        
        hasFinishedIteration = true;
    }

    public static void AddPoint(VBDPoint point)
    {
        if (Instance.points.Contains(point) == false)
        {
            Instance.points.Add(point);
        }
    }
}
