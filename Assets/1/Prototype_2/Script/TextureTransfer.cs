using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextureTransfer : MonoBehaviour
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

    private bool guideEnabled = true;
    private bool hasEnded = false;

    private bool isEndChecking = false;
    private bool hasEntered = false;

    private bool largeTextureEnabled = false;

    //Textures
    public Texture checkerTex;
    private Texture extractedTexture1;
    private Texture extractedTexture2;
    private Texture extractedTexture3;

    public TextureScrambler textureScrambler;

    //Animation
    public Animator cupboardAnimator;
    public Animator bottleAnimator;
    public Animator doorAnimator;

    public GameObject chandelierLights;
    public GameObject flashlight;
    public GameObject endArea;

    void Start()
    {
        extractedTexture1 = null;
        extractedTexture2 = null;
        extractedTexture3 = null;

        textureImage1.texture = checkerTex;
        textureImage2.texture = checkerTex;
        textureImage3.texture = checkerTex;

        largeTextureParent.SetActive(false);
        chandelierLights.SetActive(false);
        endArea.SetActive(false);
        finalResults.SetActive(false);
        endCheck.SetActive(false);
        guideObject.SetActive(true);

        Time.timeScale = 0f;
    }

    void Update()
    {
        //Guide
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

        //Large Texture Image
        if (extractedTexture1 != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            largeTextureParent.SetActive(true);
            largeTextureImage.texture = extractedTexture1;
            largeTextureEnabled = true;
        }
        if (extractedTexture2 != null && Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            largeTextureParent.SetActive(true);
            largeTextureImage.texture = extractedTexture2;
            largeTextureEnabled = true;
        }
        if (extractedTexture3 != null && Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            largeTextureParent.SetActive(true);
            largeTextureImage.texture = extractedTexture3;
            largeTextureEnabled = true;
        }

        if (largeTextureEnabled)
        {
            Time.timeScale = 0f;

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                largeTextureParent.SetActive(false);
                largeTextureEnabled = false;

                Time.timeScale = 1f;
            }

            return;
        }

        //End Checking
        if (isEndChecking)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                endCheck.SetActive(false);

                CalculateFixedPercentage();

                finalResults.SetActive(true);
                hasEnded = true;
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        //Raycast Logic and Outline
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, interactionDistance, LayerMask.GetMask("Interactable", "Selected")))
        {
            if (selectedObject != hit.collider.gameObject)
            {
                if (selectedObject != null)
                {
                    selectedObject.layer =
                        LayerMask.NameToLayer("Interactable");

                    if(selectedObject.transform.childCount > 0)
                    {
                        for (int i = 0; i < selectedObject.transform.childCount; i++)
                        {
                            selectedObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Interactable");
                        }
                    }
                }

                selectedObject = hit.collider.gameObject;
                selectedObject.layer = LayerMask.NameToLayer("Selected");

                if (selectedObject.transform.childCount > 0)
                {
                    for (int i = 0; i < selectedObject.transform.childCount; i++)
                    {
                        selectedObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Selected");
                    }
                }
            }
        }

        //Extract Texture
        if (selectedObject != null && Mouse.current.scroll.ReadValue().y < 0)
        {
            if (selectedObject.transform.childCount == 0)
            {
                Debug.Log("NO CHILDREN: " + selectedObject.name);
                Renderer objectRenderer = selectedObject.GetComponent<Renderer>();

                if (objectRenderer != null)
                {
                    Texture selectedTexture;

                    if (selectedObject.CompareTag("House"))
                    {
                        Material[] materials = objectRenderer.materials;
                        Material lastMaterial = materials[materials.Length - 1];

                        selectedTexture = lastMaterial.GetTexture("_BaseMap");
                    }
                    else
                    {
                        selectedTexture = objectRenderer.material.GetTexture("_BaseMap");
                    }

                    if (selectedTexture != null && selectedTexture != checkerTex)
                    {
                        if (StoreTexture(selectedTexture))
                        {
                            if (selectedObject.CompareTag("House"))
                            {
                                Material[] materials = objectRenderer.materials;
                                Material lastMaterial = materials[materials.Length - 1];

                                lastMaterial.SetTexture("_BaseMap", checkerTex);
                            }
                            else
                            {
                                objectRenderer.material.SetTexture("_BaseMap", checkerTex);
                            }

                            Debug.Log("Extracted Texture!");
                        }
                    }
                    else
                    {
                        Debug.Log("This object doesn't have a texture!");
                    }
                }
            }
            else
            {
                Renderer childRenderer = selectedObject.transform.GetChild(0).GetComponent<Renderer>();
                Debug.Log(
        "HAS CHILDREN: " +
        selectedObject.name +
        " | Child Count: " +
        selectedObject.transform.childCount
    );

                if (childRenderer != null)
                {
                    Texture selectedTexture = childRenderer.material.GetTexture("_BaseMap");

                    if (selectedTexture != null && selectedTexture != checkerTex)
                    {
                        if (StoreTexture(selectedTexture))
                        {
                            for (int i = 0; i < selectedObject.transform.childCount; i++)
                            {
                                Renderer childrenRenderer = selectedObject.transform.GetChild(i).GetComponent<Renderer>();

                                Debug.Log(
        selectedObject.transform.GetChild(i).name +
        " Renderer: " +
        childrenRenderer);

                                if (childrenRenderer != null)
                                {
                                    childrenRenderer.material.SetTexture("_BaseMap", checkerTex);
                                }
                            }

                            Debug.Log("Extracted Texture!");
                        }
                    }
                    else
                    {
                        Debug.Log("This object doesn't have a texture!");
                    }
                }
            }
        }

        // Transfer Texture
        if (selectedObject != null && Mouse.current.scroll.ReadValue().y > 0)
        {
            // Make sure there is a texture in Slot 1
            if (extractedTexture1 != null)
            {
                // =====================================
                // OBJECT HAS NO CHILDREN
                // =====================================
                if (selectedObject.transform.childCount == 0)
                {
                    Renderer objectRenderer =
                        selectedObject.GetComponent<Renderer>();

                    if (objectRenderer != null)
                    {
                        Texture currentTexture;


                        // HOUSE:
                        // Use the LAST material
                        if (selectedObject.CompareTag("House"))
                        {
                            Material[] materials =
                                objectRenderer.materials;

                            Material lastMaterial =
                                materials[materials.Length - 1];

                            currentTexture =
                                lastMaterial.GetTexture("_BaseMap");
                        }
                        else
                        {
                            currentTexture =
                                objectRenderer.material.GetTexture("_BaseMap");
                        }


                        // Only transfer onto checkerboard
                        if (currentTexture == checkerTex)
                        {
                            // HOUSE
                            if (selectedObject.CompareTag("House"))
                            {
                                Material[] materials =
                                    objectRenderer.materials;

                                Material lastMaterial =
                                    materials[materials.Length - 1];

                                lastMaterial.SetTexture(
                                    "_BaseMap",
                                    extractedTexture1
                                );
                            }

                            // NORMAL OBJECT
                            else
                            {
                                objectRenderer.material.SetTexture(
                                    "_BaseMap",
                                    extractedTexture1
                                );
                            }


                            UseTexture();

                            Debug.Log("Transferred Texture!");


                            if (textureScrambler.IsTextureCorrect(selectedObject))
                            {
                                Debug.Log("CORRECT TEXTURE!");

                                CorrectTextureEvent(selectedObject);
                            }
                            else
                            {
                                Debug.Log("WRONG TEXTURE!");
                            }
                        }
                        else
                        {
                            Debug.Log(
                                "You can only transfer onto an empty object!"
                            );
                        }
                    }
                }


                // =====================================
                // OBJECT HAS CHILDREN
                // =====================================
                else
                {
                    Renderer childRenderer =
                        selectedObject.transform
                            .GetChild(0)
                            .GetComponent<Renderer>();


                    if (childRenderer != null)
                    {
                        Texture currentTexture =
                            childRenderer.material.GetTexture("_BaseMap");


                        // Only transfer onto checkerboard
                        if (currentTexture == checkerTex)
                        {
                            // Give ALL children
                            // the same texture
                            for (int i = 0;
                                 i < selectedObject.transform.childCount;
                                 i++)
                            {
                                Renderer childrenRenderer =
                                    selectedObject.transform
                                        .GetChild(i)
                                        .GetComponent<Renderer>();


                                if (childrenRenderer != null)
                                {
                                    childrenRenderer.material.SetTexture(
                                        "_BaseMap",
                                        extractedTexture1
                                    );
                                }
                            }


                            UseTexture();

                            Debug.Log("Transferred Texture!");


                            if (textureScrambler.IsTextureCorrect(selectedObject))
                            {
                                Debug.Log("CORRECT TEXTURE!");

                                CorrectTextureEvent(selectedObject);
                            }
                            else
                            {
                                Debug.Log("WRONG TEXTURE!");
                            }
                        }
                        else
                        {
                            Debug.Log(
                                "You can only transfer onto an empty object!"
                            );
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No texture to transfer!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndArea"))
        {
            if (!hasEntered)
            {
                endCheck.SetActive(true);
                isEndChecking = true;

                hasEntered = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EndArea"))
        {
            hasEntered = false;
        }
    }

    bool StoreTexture(Texture newTexture)
    {
        if (extractedTexture3 != null)
        {
            Debug.Log("Can't extract anymore textures!");
            return false;
        }

        extractedTexture3 = extractedTexture2;
        extractedTexture2 = extractedTexture1;
        extractedTexture1 = newTexture;

        textureImage1.texture = extractedTexture1;

        textureImage2.texture = extractedTexture2 != null ? extractedTexture2 : checkerTex;

        textureImage3.texture = extractedTexture3 != null ? extractedTexture3 : checkerTex;

        return true;
    }

    void UseTexture()
    {
        extractedTexture1 = extractedTexture2;

        extractedTexture2 = extractedTexture3;

        extractedTexture3 = null;

        textureImage1.texture = extractedTexture1 != null ? extractedTexture1 : checkerTex;

        textureImage2.texture = extractedTexture2 != null ? extractedTexture2 : checkerTex;

        textureImage3.texture = checkerTex;
    }

    void CorrectTextureEvent(GameObject obj)
    {
        if (obj.CompareTag("Cupboard"))
        {
            cupboardAnimator.SetTrigger("CupboardOpen");
            bottleAnimator.SetTrigger("CupboardOpen");

            Debug.Log("Cupboard Open!");
        }


        if (obj.CompareTag("Door"))
        {
            doorAnimator.SetTrigger("DoorOpen");

            Debug.Log("Door Open!");

            endArea.SetActive(true);
        }


        if (obj.CompareTag("Chandelier"))
        {
            chandelierLights.SetActive(true);
            flashlight.SetActive(false);

            Debug.Log("Chandelier Activated!");
        }
    }

    void CalculateFixedPercentage()
    {
        int totalObjects = textureScrambler.objects.Count;
        int correctObjects = 0;

        foreach (TextureScrambler.TextureData data in textureScrambler.objects)
        {
            if (textureScrambler.IsTextureCorrect(data.obj))
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
            totalObjects + " textures correct = " +
            percentage + "%"
        );
    }
}
