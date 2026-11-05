using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScanViewLogic : MonoBehaviour
{
    public Material wireframeMaterial;
    public Material colliderMaterial;

    private Renderer[] allRenderers;
    private Dictionary<Renderer, Material[]> OGMaterials = new Dictionary<Renderer, Material[]>();

    private bool scanViewOn = false;
    public bool IsScanViewOn => scanViewOn;

    private MeshRenderer[] colliderRenderers;

    public MeshRenderer bugRenderer;

    public CoreMechanics coreMechanics;

    void Start()
    {
        RefreshRenderers();

        // Find all Collider-tagged objects, including inactive ones
        List<MeshRenderer> foundColliders = new List<MeshRenderer>();

        Transform[] allTransforms = FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Transform trans in allTransforms)
        {
            if (trans.CompareTag("Collider"))
            {
                MeshRenderer[] renderers =
                    trans.GetComponentsInChildren<MeshRenderer>(true);

                foreach (MeshRenderer rend in renderers)
                {
                    if (!foundColliders.Contains(rend))
                    {
                        foundColliders.Add(rend);
                        rend.enabled = false;
                    }
                }
            }
        }

        colliderRenderers = foundColliders.ToArray();

        if (bugRenderer != null)
            bugRenderer.enabled = false;

        Debug.Log("Collider displays found: " + colliderRenderers.Length);

        // Find all renderers
        allRenderers = FindObjectsByType<Renderer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Renderer rend in allRenderers)
        {
            OGMaterials[rend] = rend.sharedMaterials;
        }
    }

    void Update()
    {
        if (coreMechanics == null || !coreMechanics.hasRepairedDone)
            return;

        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            scanViewOn = !scanViewOn;

            coreMechanics.UpdateVirusMovement();

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

            Material[] newMaterials = new Material[rend.sharedMaterials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = materialToUse;
            }

            rend.sharedMaterials = newMaterials;

            bugRenderer.enabled = true;
        }
    }

    void DisableScanView()
    {
        foreach (Renderer rend in allRenderers)
        {
            rend.sharedMaterials = OGMaterials[rend];
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

    public void RefreshRenderers()
    {
        // Find all renderers, including inactive ones
        allRenderers = FindObjectsByType<Renderer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        OGMaterials.Clear();

        List<MeshRenderer> foundColliders = new List<MeshRenderer>();

        foreach (Renderer rend in allRenderers)
        {
            if (rend == null)
                continue;

            // Save original materials
            OGMaterials[rend] = rend.sharedMaterials;

            // Check whether this renderer belongs to a Collider object
            Transform current = rend.transform;

            while (current != null)
            {
                if (current.CompareTag("Collider"))
                {
                    if (rend is MeshRenderer meshRend)
                    {
                        foundColliders.Add(meshRend);
                        meshRend.enabled = false;
                    }

                    break;
                }

                current = current.parent;
            }
        }

        colliderRenderers = foundColliders.ToArray();

        Debug.Log("Scan View refreshed: " +
                  colliderRenderers.Length + " Collider Displays found.");
    }
}
