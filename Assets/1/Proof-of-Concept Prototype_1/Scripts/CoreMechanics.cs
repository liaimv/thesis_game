using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using SUPERCharacter;

public class CoreMechanics : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 10f;

    private GameObject selectedObject;

    [Header("Material UI")]
    public RawImage textureImage1;
    public RawImage textureImage2;
    public RawImage textureImage3;
    public RawImage largeTextureImage;
    public GameObject largeTextureParent;
    public GameObject endCheck;
    public GameObject finalResults;
    public TextMeshProUGUI fixedPercentage;
    public GameObject guideObject;

    [Header("Evaluation UI")]
    public GameObject evalCooldown;
    public TextMeshProUGUI evalCooldownTime;
    public GameObject evalResult;
    public TextMeshProUGUI evalResultPercentage;
    public int cooldownDelay = 10;
    public float resultDelay = 3f;
    private bool isEvalCooldown = false;
    private bool isRoomFinishChecking = false;
    private bool isPerfectRoom = false;
    private Coroutine evalResultCoroutine;
    private Coroutine evalCooldownCoroutine;
    public GameObject roomFinishCheck;
    public TextMeshProUGUI roomFinishText;
    public GameObject choiceButtons;
    public GameObject nextButton;

    private bool guideEnabled = true;
    private bool hasEnded = false;
    private bool isEndChecking = false;
    private bool largeTextureEnabled = false;

    [Header("Materials")]
    public Material checkerMaterial;
    public Texture checkerTex;

    private Material extractedMaterial1;
    private Material extractedMaterial2;
    private Material extractedMaterial3;
    // Remember which object each carried material was extracted from.
    private GameObject materialOrigin1;
    private GameObject materialOrigin2;
    private GameObject materialOrigin3;

    public MaterialScramble materialScramble;

    public GameObject flashlight;

    [Header("Scan View")]
    public bool hasRepairedDone = true;
    public GameObject[] scanColliders;

    public SUPERCharacterAIO playerController;


    private IEnumerator Start()
    {

        extractedMaterial1 = null;
        extractedMaterial2 = null;
        extractedMaterial3 = null;
        materialOrigin1 = materialOrigin2;
        materialOrigin2 = materialOrigin3;
        materialOrigin3 = null;

        UpdateInventoryUI();

        largeTextureParent.SetActive(false);
        finalResults.SetActive(false);
        endCheck.SetActive(false);
        guideObject.SetActive(true);
        evalResult.SetActive(false);
        evalCooldown.SetActive(false);
        roomFinishCheck.SetActive(false);
        choiceButtons.SetActive(false);
        nextButton.SetActive(false);

        Time.timeScale = 0f;

        // Wait until all Start methods have run
        yield return null;

        if (hasRepairedDone)
        {
            EnableScanPhase();
        }
    }

    void Update()
    {
        // Room completion confirmation takes priority over every other input.
        if (isRoomFinishChecking)
        {
            if (isPerfectRoom)
            {
                if (Keyboard.current.rKey.wasPressedThisFrame)
                    ContinueToNextRoom();
            }
            else
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                    CloseRoomFinishCheck();
                else if (Keyboard.current.qKey.wasPressedThisFrame)
                    ContinueToNextRoom();
            }
            return;
        }

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
        if (!hasRepairedDone && extractedMaterial1 != null &&
            Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            ShowMaterialPreview(extractedMaterial1);
        }

        if (!hasRepairedDone && extractedMaterial2 != null &&
            Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            ShowMaterialPreview(extractedMaterial2);
        }

        if (!hasRepairedDone && extractedMaterial3 != null &&
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
        if (!hasRepairedDone && !guideEnabled && !isEndChecking &&
            !hasEnded && !isEvalCooldown && Keyboard.current.fKey.wasPressedThisFrame)
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

        if(hasRepairedDone) return;

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

        // Extract Material
        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (scrollValue < 0)
        {
            ExtractMaterial();
        }

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

    // Only manipulate puzzle meshes, never Effect renderers.
    Renderer[] GetPuzzleRenderers(GameObject obj)
    {
        GameObject root = materialScramble.GetRegisteredPuzzleObject(obj);
        if (root == null) root = obj;

        System.Collections.Generic.List<Renderer> result =
            new System.Collections.Generic.List<Renderer>();
        foreach (Renderer rend in root.GetComponentsInChildren<Renderer>(true))
        {
            if (!materialScramble.IsEffectRenderer(rend.transform))
                result.Add(rend);
        }
        return result.ToArray();
    }

    // Extract Material
    void ExtractMaterial()
    {
        Renderer[] renderers = GetPuzzleRenderers(selectedObject);

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

        if (StoreMaterial(selectedMaterial, selectedObject))
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

        materialScramble.UpdateEffects(selectedObject);
    }

    // Transfer Material
    void TransferMaterial()
    {
        if (extractedMaterial1 == null)
        {
            Debug.Log("No material to transfer!");
            return;
        }

        Renderer[] renderers = GetPuzzleRenderers(selectedObject);

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

        materialScramble.UpdateEffects(selectedObject);

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
    bool StoreMaterial(Material newMaterial, GameObject sourceObject)
    {
        if (extractedMaterial3 != null)
        {
            Debug.Log("Can't extract anymore materials!");
            return false;
        }

        extractedMaterial3 = extractedMaterial2;
        extractedMaterial2 = extractedMaterial1;
        extractedMaterial1 = newMaterial;
        materialOrigin3 = materialOrigin2;
        materialOrigin2 = materialOrigin1;
        materialOrigin1 = materialScramble.GetRegisteredPuzzleObject(sourceObject);

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

    // Percentage for the active room ONLY.
    float GetCurrentRoomPercentage()
    {
        GameObject room = materialScramble.rooms[materialScramble.CurrentRoomIndex];
        int total = 0;
        int correct = 0;

        foreach (MaterialScramble.MaterialData data in materialScramble.objects)
        {
            if (data.obj == null || !data.obj.transform.IsChildOf(room.transform))
                continue;

            total++;
            if (materialScramble.IsTextureCorrect(data.obj))
                correct++;
        }

        return total == 0 ? 0f : (float)correct / total * 100f;
    }

    void EvaluateMaterials()
    {
        float percentage = GetCurrentRoomPercentage();

        evalResultPercentage.text = Mathf.RoundToInt(percentage) + "%";
        evalResult.SetActive(true);

        if (evalResultCoroutine != null)
        {
            StopCoroutine(evalResultCoroutine);
            evalResultCoroutine = null;
        }

        // ALWAYS start cooldown when evaluating
        isEvalCooldown = true;
        evalCooldown.SetActive(true);

        if (evalCooldownCoroutine != null)
        {
            StopCoroutine(evalCooldownCoroutine);
        }

        evalCooldownCoroutine = StartCoroutine(EvaluationCooldown());

        // Room completion check
        if (percentage >= 50f)
        {
            isRoomFinishChecking = true;
            isPerfectRoom = percentage >= 100f - 0.0001f;

            roomFinishCheck.SetActive(true);
            choiceButtons.SetActive(!isPerfectRoom);
            nextButton.SetActive(isPerfectRoom);

            roomFinishText.text = isPerfectRoom
                ? "You have fully repaired this section!"
                : "This section has reached the minimum reparation level required to continue. However, some corruption remains. Once you proceed, you won't be able to return to this section to repair. Any materials you are holding will be returned to their last assigned objects. Are you sure you want to continue?";

            // Keep evaluation result visible
        }
        else
        {
            // Hide result after delay
            evalResultCoroutine = StartCoroutine(HideEvalResult());
        }
    }

    void CloseRoomFinishCheck()
    {
        isRoomFinishChecking = false;
        roomFinishCheck.SetActive(false);
        choiceButtons.SetActive(false);
        nextButton.SetActive(false);
        evalResult.SetActive(false);
    }

    void ContinueToNextRoom()
    {
        // Return materials before disabling the old room
        ReturnHeldMaterials();

        CloseRoomFinishCheck();

        // Reset evaluation cooldown for the next room
        ResetEvaluationCooldown();

        if (selectedObject != null)
        {
            SetObjectLayer(
                selectedObject,
                LayerMask.NameToLayer("Interactable")
            );

            selectedObject = null;
        }

        if (materialScramble.CurrentRoomIndex + 1 <
            materialScramble.rooms.Length)
        {
            // Activate next room and teleport player
            materialScramble.NextRoom();
        }
        else
        {
            // Last room completed
            CalculateFixedPercentage();

            finalResults.SetActive(true);
            hasEnded = true;
        }
    }

    void ReturnHeldMaterials()
    {
        ReturnMaterialToOrigin(extractedMaterial1, materialOrigin1);
        ReturnMaterialToOrigin(extractedMaterial2, materialOrigin2);
        ReturnMaterialToOrigin(extractedMaterial3, materialOrigin3);

        extractedMaterial1 = null;
        extractedMaterial2 = null;
        extractedMaterial3 = null;
        materialOrigin1 = null;
        materialOrigin2 = null;
        materialOrigin3 = null;
        UpdateInventoryUI();
    }

    void ReturnMaterialToOrigin(Material material, GameObject origin)
    {
        if (material == null || origin == null)
            return;

        Renderer[] renderers = origin.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            if (materialScramble.IsEffectRenderer(rend.transform))
                continue;

            // Only refill empty (checkerboard) material slots.
            Material[] materials = rend.sharedMaterials;
            bool changed = false;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == checkerMaterial)
                {
                    materials[i] = material;
                    changed = true;
                }
            }
            if (changed)
                rend.sharedMaterials = materials;
        }
        materialScramble.UpdateEffects(origin);
    }

    IEnumerator HideEvalResult()
    {
        yield return new WaitForSecondsRealtime(resultDelay);
        if (!isRoomFinishChecking)
            evalResult.SetActive(false);
        evalResultCoroutine = null;
    }

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
        evalCooldownCoroutine = null;
    }

    void ResetEvaluationCooldown()
    {
        if (evalCooldownCoroutine != null)
        {
            StopCoroutine(evalCooldownCoroutine);
            evalCooldownCoroutine = null;
        }

        isEvalCooldown = false;

        evalCooldownTime.text = "0";
        evalCooldown.SetActive(false);
    }

    void EnableScanPhase()
    {
        playerController.ChangePerspective(PerspectiveModes._3rdPerson);

        // Activate all rooms
        foreach (GameObject room in materialScramble.rooms)
        {
            if (room != null)
            {
                room.SetActive(true);
            }
        }

        // Disable material transfer UI
        textureImage1.gameObject.SetActive(false);
        textureImage2.gameObject.SetActive(false);
        textureImage3.gameObject.SetActive(false);

        evalResult.SetActive(false);
        evalCooldown.SetActive(false);
        largeTextureParent.SetActive(false);

        // Remove colliders from Interactable and Selected objects
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        int selectedLayer = LayerMask.NameToLayer("Selected");

        GameObject[] allObjects = FindObjectsByType<GameObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        int removedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer != interactableLayer &&
                obj.layer != selectedLayer)
            {
                continue;
            }

            Collider[] colliders = obj.GetComponents<Collider>();

            foreach (Collider col in colliders)
            {
                col.enabled = false;
                Destroy(col);
                removedCount++;
            }
        }

        // Activate scan colliders and their children
        foreach (GameObject obj in scanColliders)
        {
            if (obj == null)
                continue;

            Transform[] allChildren =
                obj.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                child.gameObject.SetActive(true);
            }
        }

        Debug.Log(
            "Scan Phase Enabled! All rooms activated. Removed " +
            removedCount + " colliders."
        );
    }
}


