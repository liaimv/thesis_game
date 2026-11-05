using System.Collections.Generic;
using UnityEngine;

public class MaterialScramble : MonoBehaviour
{
    [System.Serializable]
    public class MaterialData
    {
        public GameObject obj;
        public Material correctMaterial;
    }

    public List<MaterialData> objects = new List<MaterialData>();

    void Start()
    {
        SaveOriginalMaterials();
        ScrambleMaterials();
    }

    // =========================================
    // SAVE ORIGINAL MATERIALS
    // =========================================

    void SaveOriginalMaterials()
    {
        objects.Clear();

        GameObject[] allObjects =
            FindObjectsByType<GameObject>(
                FindObjectsSortMode.None
            );

        foreach (GameObject obj in allObjects)
        {
            // Only look at Interactable objects
            if (obj.layer != LayerMask.NameToLayer("Interactable"))
            {
                continue;
            }

            // Don't save children separately
            Transform parent = obj.transform.parent;
            bool hasInteractableParent = false;

            while (parent != null)
            {
                if (parent.gameObject.layer ==
                    LayerMask.NameToLayer("Interactable"))
                {
                    hasInteractableParent = true;
                    break;
                }

                parent = parent.parent;
            }

            if (hasInteractableParent)
            {
                continue;
            }

            // Save one material per puzzle object
            Renderer renderer = GetObjectRenderer(obj);

            if (renderer != null)
            {
                Material originalMaterial = renderer.sharedMaterial;

                if (originalMaterial != null)
                {
                    MaterialData data = new MaterialData();

                    data.obj = obj;
                    data.correctMaterial = originalMaterial;

                    objects.Add(data);
                }
            }
        }

        Debug.Log("Saved " + objects.Count + " puzzle objects.");
    }

    // =========================================
    // SCRAMBLE MATERIALS
    // =========================================

    void ScrambleMaterials()
    {
        if (objects.Count < 2)
        {
            Debug.Log("Need at least 2 objects to scramble!");
            return;
        }

        List<MaterialData> remainingObjects =
            new List<MaterialData>(objects);

        List<Material> remainingMaterials =
            new List<Material>();

        foreach (MaterialData data in objects)
        {
            remainingMaterials.Add(data.correctMaterial);
        }

        bool validScramble = false;
        int attempts = 0;

        while (!validScramble && attempts < 1000)
        {
            // Shuffle materials
            for (int i = 0; i < remainingMaterials.Count; i++)
            {
                int randomIndex =
                    Random.Range(i, remainingMaterials.Count);

                Material temp = remainingMaterials[i];

                remainingMaterials[i] =
                    remainingMaterials[randomIndex];

                remainingMaterials[randomIndex] = temp;
            }

            validScramble = true;

            // Check that no object has its own material
            for (int i = 0; i < remainingObjects.Count; i++)
            {
                if (remainingMaterials[i] ==
                    remainingObjects[i].correctMaterial)
                {
                    validScramble = false;
                    break;
                }
            }

            attempts++;
        }

        // Apply scrambled materials
        if (validScramble)
        {
            for (int i = 0; i < remainingObjects.Count; i++)
            {
                SetObjectMaterial(
                    remainingObjects[i].obj,
                    remainingMaterials[i]
                );
            }

            Debug.Log("Materials scrambled successfully!");
        }
        else
        {
            Debug.LogError(
                "Could not create a valid material scramble!"
            );
        }
    }

    // =========================================
    // CHECK CORRECT MATERIAL
    // =========================================

    public bool IsTextureCorrect(GameObject obj)
    {
        GameObject puzzleObject = GetPuzzleObject(obj);

        foreach (MaterialData data in objects)
        {
            if (data.obj == puzzleObject)
            {
                Renderer renderer =
                    GetObjectRenderer(puzzleObject);

                if (renderer != null)
                {
                    return renderer.sharedMaterial ==
                           data.correctMaterial;
                }
            }
        }

        return false;
    }

    // =========================================
    // GET CORRECT MATERIAL
    // =========================================

    public Material GetCorrectMaterial(GameObject obj)
    {
        GameObject puzzleObject = GetPuzzleObject(obj);

        foreach (MaterialData data in objects)
        {
            if (data.obj == puzzleObject)
            {
                return data.correctMaterial;
            }
        }

        return null;
    }

    // =========================================
    // FIND PUZZLE ROOT
    // =========================================

    GameObject GetPuzzleObject(GameObject obj)
    {
        GameObject puzzleObject = obj;
        Transform parent = obj.transform.parent;

        while (parent != null)
        {
            if (parent.gameObject.layer ==
                    LayerMask.NameToLayer("Interactable") ||
                parent.gameObject.layer ==
                    LayerMask.NameToLayer("Selected"))
            {
                puzzleObject = parent.gameObject;
            }

            parent = parent.parent;
        }

        return puzzleObject;
    }

    // =========================================
    // GET ONE RENDERER
    // =========================================

    Renderer GetObjectRenderer(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer;
        }

        return obj.GetComponentInChildren<Renderer>();
    }

    // =========================================
    // SET OBJECT MATERIAL
    // =========================================

    void SetObjectMaterial(GameObject obj, Material material)
    {
        Renderer[] renderers =
            obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            renderer.sharedMaterials = materials;
        }
    }
}
