using System.Collections.Generic;
using UnityEngine;

public class TextureScrambler : MonoBehaviour
{
    [System.Serializable]
    public class TextureData
    {
        public GameObject obj;
        public Texture correctTexture;
    }

    public List<TextureData> objects =
        new List<TextureData>();

    void Start()
    {
        SaveOriginalTextures();
        ScrambleTextures();
    }


    // =========================================
    // SAVE ORIGINAL TEXTURES
    // =========================================

    void SaveOriginalTextures()
    {
        objects.Clear();

        GameObject[] allObjects =
            FindObjectsByType<GameObject>(
                FindObjectsSortMode.None
            );

        foreach (GameObject obj in allObjects)
        {
            // Only look at Interactable objects
            if (obj.layer !=
                LayerMask.NameToLayer("Interactable"))
            {
                continue;
            }


            // ---------------------------------
            // DON'T SAVE CHILDREN SEPARATELY
            // ---------------------------------

            // Check if this object is underneath
            // another Interactable object.
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


            // If it belongs to another
            // Interactable object, skip it.
            if (hasInteractableParent)
            {
                continue;
            }


            // ---------------------------------
            // SAVE ONE TEXTURE FOR THIS OBJECT
            // ---------------------------------

            Renderer renderer =
                GetObjectRenderer(obj);

            if (renderer != null)
            {
                Texture originalTexture =
                    GetObjectTexture(
                        obj,
                        renderer
                    );

                if (originalTexture != null)
                {
                    TextureData data =
                        new TextureData();

                    data.obj = obj;
                    data.correctTexture =
                        originalTexture;

                    objects.Add(data);
                }
            }
        }


        Debug.Log(
            "Saved " +
            objects.Count +
            " puzzle objects."
        );
    }


    // =========================================
    // SCRAMBLE
    // =========================================

    void ScrambleTextures()
    {
        if (objects.Count < 2)
        {
            Debug.Log("Need at least 2 objects to scramble!");
            return;
        }


        TextureData bottleData = null;
        TextureData doorData = null;


        // =========================================
        // FIND BOTTLE AND DOOR
        // =========================================

        foreach (TextureData data in objects)
        {
            if (data.obj.CompareTag("Bottle"))
            {
                bottleData = data;
            }

            if (data.obj.CompareTag("Door"))
            {
                doorData = data;
            }
        }


        if (bottleData == null || doorData == null)
        {
            Debug.LogError(
                "Could not find Bottle or Door!"
            );

            return;
        }


        // =========================================
        // BOTTLE ALWAYS GETS DOOR TEXTURE
        // =========================================

        SetObjectTexture(
            bottleData.obj,
            doorData.correctTexture
        );


        // =========================================
        // CREATE LIST OF REMAINING OBJECTS
        // AND REMAINING TEXTURES
        // =========================================

        List<TextureData> remainingObjects =
            new List<TextureData>();

        List<Texture> remainingTextures =
            new List<Texture>();


        foreach (TextureData data in objects)
        {
            // Bottle already received its texture,
            // so don't scramble Bottle again.
            if (data != bottleData)
            {
                remainingObjects.Add(data);
            }


            // Door texture has already been used
            // on Bottle, so don't use it again.
            if (data != doorData)
            {
                remainingTextures.Add(
                    data.correctTexture
                );
            }
        }


        // =========================================
        // FIND A VALID SCRAMBLE
        // =========================================

        bool validScramble = false;

        int attempts = 0;


        while (!validScramble && attempts < 1000)
        {
            // Shuffle remaining textures
            for (int i = 0;
                 i < remainingTextures.Count;
                 i++)
            {
                Texture temp =
                    remainingTextures[i];

                int randomIndex =
                    Random.Range(
                        i,
                        remainingTextures.Count
                    );

                remainingTextures[i] =
                    remainingTextures[randomIndex];

                remainingTextures[randomIndex] =
                    temp;
            }


            // Assume scramble is valid
            validScramble = true;


            // Check that nobody got
            // their own texture
            for (int i = 0;
                 i < remainingObjects.Count;
                 i++)
            {
                if (remainingTextures[i] ==
                    remainingObjects[i].correctTexture)
                {
                    validScramble = false;
                    break;
                }
            }


            attempts++;
        }


        // =========================================
        // APPLY SCRAMBLED TEXTURES
        // =========================================

        if (validScramble)
        {
            for (int i = 0;
                 i < remainingObjects.Count;
                 i++)
            {
                SetObjectTexture(
                    remainingObjects[i].obj,
                    remainingTextures[i]
                );
            }


            Debug.Log(
                "Textures scrambled! Bottle has Door texture."
            );
        }
        else
        {
            Debug.LogError(
                "Could not create a valid texture scramble!"
            );
        }
    }


    // =========================================
    // CHECK CORRECT TEXTURE
    // =========================================

    public bool IsTextureCorrect(
        GameObject obj
    )
    {
        // In case a child was passed,
        // find its puzzle root.
        GameObject puzzleObject =
            GetPuzzleObject(obj);


        foreach (TextureData data in objects)
        {
            if (data.obj == puzzleObject)
            {
                Renderer renderer =
                    GetObjectRenderer(
                        puzzleObject
                    );

                if (renderer != null)
                {
                    Texture currentTexture =
                        GetObjectTexture(
                            puzzleObject,
                            renderer
                        );


                    return currentTexture ==
                           data.correctTexture;
                }
            }
        }


        return false;
    }


    // =========================================
    // GET CORRECT TEXTURE
    // =========================================

    public Texture GetCorrectTexture(
        GameObject obj
    )
    {
        GameObject puzzleObject =
            GetPuzzleObject(obj);


        foreach (TextureData data in objects)
        {
            if (data.obj == puzzleObject)
            {
                return data.correctTexture;
            }
        }


        return null;
    }


    // =========================================
    // FIND PUZZLE ROOT
    // =========================================

    GameObject GetPuzzleObject(
        GameObject obj
    )
    {
        GameObject puzzleObject = obj;

        Transform parent =
            obj.transform.parent;


        while (parent != null)
        {
            if (parent.gameObject.layer ==
                    LayerMask.NameToLayer(
                        "Interactable"
                    ) ||
                parent.gameObject.layer ==
                    LayerMask.NameToLayer(
                        "Selected"
                    ))
            {
                puzzleObject =
                    parent.gameObject;
            }

            parent = parent.parent;
        }


        return puzzleObject;
    }


    // =========================================
    // GET ONE RENDERER
    // =========================================

    Renderer GetObjectRenderer(
        GameObject obj
    )
    {
        Renderer renderer =
            obj.GetComponent<Renderer>();


        if (renderer != null)
        {
            return renderer;
        }


        return obj.GetComponentInChildren
            <Renderer>();
    }


    // =========================================
    // GET OBJECT TEXTURE
    // =========================================

    Texture GetObjectTexture(
        GameObject obj,
        Renderer renderer
    )
    {
        // ---------------------------------
        // HOUSE
        // ---------------------------------

        if (obj.CompareTag("House"))
        {
            Material[] materials =
                renderer.materials;


            if (materials.Length > 0)
            {
                Material lastMaterial =
                    materials[
                        materials.Length - 1
                    ];


                return lastMaterial.GetTexture(
                    "_BaseMap"
                );
            }
        }


        // ---------------------------------
        // NORMAL OBJECT
        // ---------------------------------

        return renderer.material.GetTexture(
            "_BaseMap"
        );
    }


    // =========================================
    // SET OBJECT TEXTURE
    // =========================================

    void SetObjectTexture(
        GameObject obj,
        Texture texture
    )
    {
        // ---------------------------------
        // HOUSE
        // Only change last material
        // ---------------------------------

        if (obj.CompareTag("House"))
        {
            Renderer renderer =
                GetObjectRenderer(obj);


            if (renderer != null)
            {
                Material[] materials =
                    renderer.materials;


                if (materials.Length > 0)
                {
                    Material lastMaterial =
                        materials[
                            materials.Length - 1
                        ];


                    lastMaterial.SetTexture(
                        "_BaseMap",
                        texture
                    );
                }
            }


            return;
        }


        // ---------------------------------
        // NORMAL OBJECT
        //
        // Give ALL renderers belonging
        // to this object the SAME texture.
        // ---------------------------------

        Renderer[] renderers =
            obj.GetComponentsInChildren
                <Renderer>();


        foreach (Renderer renderer
                 in renderers)
        {
            renderer.material.SetTexture(
                "_BaseMap",
                texture
            );
        }
    }
}
