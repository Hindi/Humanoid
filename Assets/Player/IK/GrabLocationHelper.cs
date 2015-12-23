using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class GrabLocationHelper : MonoBehaviour
{

    BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Use this for initialization
    void Start () {
        createGrabs();

    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void createGrabs()
    {
        Bounds bounds = boxCollider.bounds;
        Vector3 extents = bounds.extents;
        GameObject obj = new GameObject();
        obj.transform.position = bounds.center + extents;
        obj.transform.SetParent(transform);
    }
}
