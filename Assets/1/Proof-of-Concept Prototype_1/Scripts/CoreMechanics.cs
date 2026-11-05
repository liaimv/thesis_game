using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CoreMechanics : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 10f;

    private GameObject selectedObject;

    //UI
    public RawImage textureImage1;
    public RawImage textureImage2;
    public RawImage textureImage3;
    public RawImage largeTextureImage;
    public GameObject largeTextureParent;
    public GameObject endCheck;
    public GameObject finalResults;
    public TextMeshProUGUI fixedPercentage;
    public GameObject guideObject;

    //Evaluation UI
    public GameObject evalCooldown;
    public TextMeshProUGUI evalCooldownTime;
    public GameObject evalResult;
    public TextMeshProUGUI evalResultPercentage;
    public int cooldownDelay = 10;
    public float resultDelay = 3f;
    private bool isEvalCooldown = false;

    private bool guideEnabled = true;
    private bool hasEnded = false;
    private bool isEndChecking = false;
    private bool largeTextureEnabled = false;

    // Materials
    public Material checkerMaterial;
    public Texture checkerTex;

    private Material extractedMaterial1;
    private Material extractedMaterial2;
    private Material extractedMaterial3;

    public MaterialScramble materialScramble;

    public GameObject flashlight;

    void Start()
    {
        extractedMaterial1 = null;
        extractedMaterial2 = null;
        extractedMaterial3 = null;

        UpdateInventoryUI();

        largeTextureParent.SetActive(false);
        finalResults.SetActive(false);
        endCheck.SetActive(false);
        guideObject.SetActive(true);
        evalResult.SetActive(false);
        evalCooldown.SetActive(false);

        Time.timeScale = 0f;
    }

    void Update()
    {
        // Guide
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (guideEnabled)
            {
                guideObject.SetActive(false);
                guideEnabled = false;
                Time.timeScale = 1f;
            }
            else
            {
                guideObject.SetActive(true);
                guideEnabled = true;
                Time.timeScale = 0f;
            }
        }

        // Large Material Preview
        if (extractedMaterial1 != null &&
            Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            ShowMaterialPreview(extractedMaterial1);
        }

        if (extractedMaterial2 != null &&
            Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            ShowMaterialPreview(extractedMaterial2);
        }

        if (extractedMaterial3 != null &&
            Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            ShowMaterialPreview(extractedMaterial3);
        }

        if (largeTextureEnabled)
        {
            Time.timeScale = 0f;

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                largeTextureParent.SetActive(false);
                largeTextureEnabled = false;

                Time.timeScale = guideEnabled ? 0f : 1f;
            }

            return;
        }

        // Evaluate Materials
        if (!guideEnabled && !isEndChecking &&
            !isEvalCooldown && Keyboard.current.fKey.wasPressedThisFrame)
        {
            EvaluateMaterials();
        }

        // End Checking
        if (isEndChecking)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                endCheck.SetActive(false);

                CalculateFixedPercentage();

                finalResults.SetActive(true);
                hasEnded = true;
                isEndChecking = false;
            }
            else if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                isEndChecking = false;
                endCheck.SetActive(false);
            }
        }

        if (hasEnded)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex
                );
            }

            return;
        }

        // Raycast Logic and Outline
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );

        Debug.DrawRay(
            ray.origin,
            ray.direction * interactionDistance,
            Color.red
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactionDistance,
            LayerMask.GetMask("Interactable", "Selected")))
        {
            if (selectedObject != hit.collider.gameObject)
            {
                if (selectedObject != null)
                {
                    SetObjectLayer(
                        selectedObject,
                        LayerMask.NameToLayer("Interactable")
                    );
                }

                selectedObject = hit.collider.gameObject;

                SetObjectLayer(
                    selectedObject,
                    LayerMask.NameToLayer("Selected")
                );
            }
        }
        else
        {
            if (selectedObject != null)
            {
                SetObjectLayer(
                    selectedObject,
                    LayerMask.NameToLayer("Interactable")
                );

                selectedObject = null;
            }
        }

        if (selectedObject == null)
            return;

        float scrollValue = Mouse.current.scroll.ReadValue().y;

        // Extract Material
        if (scrollValue < 0)
        {
            ExtractMaterial();
        }

        // Transfer Material
        if (scrollValue > 0)
        {
            TransferMaterial();
        }
    }

    // Change Object and Children Layers
    void SetObjectLayer(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetObjectLayer(child.gameObject, layer);
        }
    }

    // Extract Material
    void ExtractMaterial()
    {
        Renderer[] renderers =
            selectedObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.Log("This object has no renderer!");
            return;
        }

        // Extract material from first renderer
        Material selectedMaterial =
            renderers[0].sharedMaterial;

        if (selectedMaterial == null ||
            selectedMaterial == checkerMaterial)
        {
            Debug.Log("This object doesn't have a material!");
            return;
        }

        if (StoreMaterial(selectedMaterial))
        {
            // Replace all materials with checkerMaterial
            foreach (Renderer rend in renderers)
            {
                Material[] materials = rend.sharedMaterials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = checkerMaterial;
                }

                rend.sharedMaterials = materials;
            }

            Debug.Log(
                "Extracted Material: " + selectedMaterial.name
            );
        }
    }

    // Transfer Material
    void TransferMaterial()
    {
        if (extractedMaterial1 == null)
        {
            Debug.Log("No material to transfer!");
            return;
        }

        Renderer[] renderers =
            selectedObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.Log("This object has no renderer!");
            return;
        }

        Material currentMaterial =
            renderers[0].sharedMaterial;

        // Only transfer onto checkerboard material
        if (currentMaterial != checkerMaterial)
        {
            Debug.Log(
                "You can only transfer onto an empty object!"
            );

            return;
        }

        // Apply material to all renderers
        foreach (Renderer rend in renderers)
        {
            Material[] materials = rend.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = extractedMaterial1;
            }

            rend.sharedMaterials = materials;
        }

        Debug.Log(
            "Transferred Material: " + extractedMaterial1.name
        );

        UseMaterial();

        // Check correctness
        // Requires updating TextureScramble to check materials
        // instead of textures.
        if (materialScramble.IsTextureCorrect(selectedObject))
        {
            Debug.Log("CORRECT MATERIAL!");
        }
        else
        {
            Debug.Log("WRONG MATERIAL!");
        }
    }

    // Store Material
    bool StoreMaterial(Material newMaterial)
    {
        if (extractedMaterial3 != null)
        {
            Debug.Log("Can't extract anymore materials!");
            return false;
        }

        extractedMaterial3 = extractedMaterial2;
        extractedMaterial2 = extractedMaterial1;
        extractedMaterial1 = newMaterial;

        UpdateInventoryUI();

        return true;
    }

    // Use Material
    void UseMaterial()
    {
        extractedMaterial1 = extractedMaterial2;
        extractedMaterial2 = extractedMaterial3;
        extractedMaterial3 = null;

        UpdateInventoryUI();
    }

    // Get Preview Texture From Material
    Texture GetMaterialPreview(Material mat)
    {
        if (mat == null)
            return checkerTex;

        if (mat.HasProperty("_BaseMap"))
        {
            Texture tex = mat.GetTexture("_BaseMap");

            if (tex != null)
                return tex;
        }

        return checkerTex;
    }

    // Update Inventory UI
    void UpdateInventoryUI()
    {
        textureImage1.texture =
            GetMaterialPreview(extractedMaterial1);

        textureImage2.texture =
            GetMaterialPreview(extractedMaterial2);

        textureImage3.texture =
            GetMaterialPreview(extractedMaterial3);
    }

    // Large Material Preview
    void ShowMaterialPreview(Material mat)
    {
        largeTextureParent.SetActive(true);

        largeTextureImage.texture =
            GetMaterialPreview(mat);

        largeTextureEnabled = true;
    }

    // Calculate Fixed Percentage
    void CalculateFixedPercentage()
    {
        int totalObjects = materialScramble.objects.Count;
        int correctObjects = 0;

        foreach (MaterialScramble.MaterialData data
                 in materialScramble.objects)
        {
            if (materialScramble.IsTextureCorrect(data.obj))
            {
                correctObjects++;
            }
        }

        float percentage = 0f;

        if (totalObjects > 0)
        {
            percentage =
                ((float)correctObjects / totalObjects) * 100f;
        }

        fixedPercentage.text =
            Mathf.RoundToInt(percentage) + "%";

        Debug.Log(
            correctObjects + "/" +
            totalObjects + " materials correct = " +
            percentage + "%"
        );
    }

    void EvaluateMaterials()
    {
        int totalObjects = materialScramble.objects.Count;
        int correctObjects = 0;

        foreach (MaterialScramble.MaterialData data in materialScramble.objects)
        {
            if (materialScramble.IsTextureCorrect(data.obj))
            {
                correctObjects++;
            }
        }

        float percentage = 0f;

        if (totalObjects > 0)
        {
            percentage = ((float)correctObjects / totalObjects) * 100f;
        }

        evalResultPercentage.text = Mathf.RoundToInt(percentage) + "%";

        // Show evaluation result
        evalResult.SetActive(true);

        StartCoroutine(HideEvalResult());

        // Start cooldown
        isEvalCooldown = true;
        evalCooldown.SetActive(true);

        StartCoroutine(EvaluationCooldown());
    }

    // Hide Evaluation Result
    IEnumerator HideEvalResult()
    {
        yield return new WaitForSecondsRealtime(resultDelay);

        evalResult.SetActive(false);
    }

    // Evaluation Cooldown
    IEnumerator EvaluationCooldown()
    {
        float remainingTime = Mathf.Max(0, cooldownDelay);

        while (remainingTime > 0f)
        {
            evalCooldownTime.text = Mathf.CeilToInt(remainingTime).ToString();

            yield return null;

            remainingTime -= Time.unscaledDeltaTime;
        }

        evalCooldownTime.text = "0";

        evalCooldown.SetActive(false);
        isEvalCooldown = false;
    }
}
