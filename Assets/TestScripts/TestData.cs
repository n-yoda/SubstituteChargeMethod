using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TestResult))]
public class TestData : MonoBehaviour
{
    public double distance = SubstituteChargeMethod.MinDistance;
    public int count = 4;
    public double radius = 10;
    public double height = 1;
    public int errorDelta = 1000000;
    public MeshFilter graph;
    public Transform chargeRoot;
    public GameObject charge;
    public Transform sampleRoot;
    public GameObject sample;
}
