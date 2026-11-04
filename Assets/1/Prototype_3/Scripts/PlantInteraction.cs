using UnityEngine;

public class PlantInteraction : MonoBehaviour
{
    public Color emissionColor1;
    public Color emissionColor2;
    public Vector3 scaleSmall = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 scaleLarge = new Vector3(1f, 1f, 1f);

    private Material[] plantMats;

    private Color currentColor;

    private bool inMushroomArea = false;
    private bool npcOnPlant = false;

    void Start()
    {
        Renderer plantRend = GetComponent<Renderer>();
        plantMats = plantRend.materials;

        currentColor = emissionColor1;

        UpdatePlantState();
    }

    void Update()
    {
        // Scale
        if (inMushroomArea)
        {
            transform.localScale = scaleLarge;
        }
        else
        {
            transform.localScale = scaleSmall;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Mushroom spotlight enters
        if (other.CompareTag("MushroomSpotlight"))
        {
            inMushroomArea = true;

            // Recheck color because mushroom area changed
            UpdatePlantState();
        }

        // NPC enters
        if (other.CompareTag("NPC"))
        {
            npcOnPlant = true;

            UpdatePlantState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Mushroom spotlight leaves
        if (other.CompareTag("MushroomSpotlight"))
        {
            inMushroomArea = false;

            // Recheck color because mushroom area changed
            UpdatePlantState();
        }

        // NPC leaves
        if (other.CompareTag("NPC"))
        {
            npcOnPlant = false;

            UpdatePlantState();
        }
    }

    void UpdatePlantState()
    {
        // NPC changes color ONLY when plant is outside mushroom area
        if (npcOnPlant && !inMushroomArea)
        {
            currentColor = emissionColor2;
        }
        else
        {
            currentColor = emissionColor1;
        }

        UpdateEmission();
    }

    void UpdateEmission()
    {
        foreach (Material mat in plantMats)
        {
            mat.SetColor("_EmissionColor", currentColor);
        }
    }
}
