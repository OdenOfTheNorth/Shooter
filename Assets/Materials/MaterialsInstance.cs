using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsInstance : MonoBehaviour
{
    public Color color;
    private Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.color = color;
    }
}
