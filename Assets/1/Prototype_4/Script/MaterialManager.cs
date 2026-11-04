using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MaterialManager : MonoBehaviour
{
    public Material wireframeMaterial;
    public Material colliderMaterial;

    private Renderer[] allRenderers;
    private Dictionary<Renderer, Material[]> OGMaterials = new Dictionary<Renderer, Material[]>();

    private bool scanViewOn = false;

    private MeshRenderer[] colliderRenderers;

    public MeshRenderer bugRenderer;

    void Start()
    {
        // Find Collider-tagged objects
        GameObject[] colliderObjects = GameObject.FindGameObjectsWithTag("Collider");

        colliderRenderers = new MeshRenderer[colliderObjects.Length];

        for (int i = 0; i < colliderObjects.Length; i++)
        {
            colliderRenderers[i] = colliderObjects[i].GetComponent<MeshRenderer>();

            if (colliderRenderers[i] != null)
            {
                colliderRenderers[i].enabled = false;
            }
        }
        bugRenderer.enabled = false;


        Debug.Log("Collider displays found: " + colliderObjects.Length);

        // Find all renderers
        allRenderers = FindObjectsByType<Renderer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Renderer rend in allRenderers)
        {
            OGMaterials[rend] = rend.materials;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            scanViewOn = !scanViewOn;

            if (scanViewOn)
            {
                EnableScanView();
                EnableColliderRenderers();
            }
            else
            {
                DisableScanView();
                DisableColliderRenderers();
            }
        }
    }

    void EnableScanView()
    {
        foreach (Renderer rend in allRenderers)
        {
            Material materialToUse;

            if (rend.CompareTag("Collider"))
            {
                materialToUse = colliderMaterial;
            }
            else
            {
                materialToUse = wireframeMaterial;
            }

            Material[] newMaterials = new Material[rend.materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = materialToUse;
            }

            rend.materials = newMaterials;

            bugRenderer.enabled = true;
        }
    }

    void DisableScanView()
    {
        foreach (Renderer rend in allRenderers)
        {
            rend.materials = OGMaterials[rend];
        }

        bugRenderer.enabled = false;
    }

    void EnableColliderRenderers()
    {
        foreach (MeshRenderer rend in colliderRenderers)
        {
            if (rend != null)
            {
                rend.enabled = true;
            }
        }
    }

    void DisableColliderRenderers()
    {
        foreach (MeshRenderer rend in colliderRenderers)
        {
            if (rend != null)
            {
                rend.enabled = false;
            }
        }
    }
}
