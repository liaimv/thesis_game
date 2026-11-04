using UnityEngine;
using UnityEngine.InputSystem;
using SUPERCharacter;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerTransfer : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 50f;

    public float exitDistance = 2f;

    private bool isTransferred = false;

    private GameObject transferredObject;
    private Transform originalParent;

    public GameObject mushroomSpotlight;

    public GameObject interactionUI;
    public GameObject transferUI;
    public GameObject finalResultsUI;
    public GameObject introUI;
    public GameObject transferOutUI;

    private SUPERCharacterAIO playerController;

    public GameObject[] plants;
    private GameObject virusPlant;
    public TextMeshProUGUI resultText;

    private bool hasEnded = false;

    void Start()
    {
        interactionUI.SetActive(false);
        transferUI.SetActive(false);
        transferOutUI.SetActive(false);
        finalResultsUI.SetActive(false);

        introUI.SetActive(true);

        playerController = GetComponent<SUPERCharacterAIO>();

        // Randomly choose one of the plants as the virus
        if (plants.Length > 0)
        {
            virusPlant = plants[Random.Range(0, plants.Length)];

            Debug.Log("Virus plant is: " + virusPlant.name);

            PlantInteraction virusPlantInteraction = virusPlant.GetComponent<PlantInteraction>();

            if (virusPlantInteraction != null)
            {
                virusPlantInteraction.emissionColor2 = virusPlantInteraction.emissionColor1;
            }
        }
    }

    void Update()
    {
        // Press R to close intro UI
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!hasEnded)
            {
                introUI.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene("Prototype 3 - Investigate a Virus");
            }
        }

        if (isTransferred)
        {
            transferUI.SetActive(false);
            interactionUI.SetActive(false);

            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                TransferOut();
            }

            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Debug.DrawRay(
            ray.origin,
            ray.direction * interactDistance,
            Color.red
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactDistance,
            LayerMask.GetMask("Interactable")))
        {
            // LOOKING AT A PLANT
            if (hit.collider.CompareTag("Plant"))
            {
                interactionUI.SetActive(true);
                transferUI.SetActive(false);

                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    CheckPlantResult(hit.collider.gameObject);
                }
            }

            // LOOKING AT AN NPC
            else if (hit.collider.CompareTag("NPC"))
            {
                transferUI.SetActive(true);
                interactionUI.SetActive(false);

                if (Keyboard.current.tKey.wasPressedThisFrame)
                {
                    TransferToObject(hit.collider.gameObject);
                }
            }

            else
            {
                interactionUI.SetActive(false);
                transferUI.SetActive(false);
            }
        }
        else
        {
            interactionUI.SetActive(false);
            transferUI.SetActive(false);
        }
    }

    void CheckPlantResult(GameObject selectedPlant)
    {
        finalResultsUI.SetActive(true);

        if (selectedPlant == virusPlant)
        {
            resultText.text = "You successfully destroyed the virus!";
        }
        else
        {
            resultText.text = "You destroyed the wrong game object. You broke the game!";
        }

        hasEnded = true;
    }

    void TransferToObject(GameObject targetObject)
    {
        transferredObject = targetObject;

        originalParent = targetObject.transform.parent;

        transform.position = targetObject.transform.position;

        targetObject.transform.SetParent(transform);

        isTransferred = true;

        transferUI.SetActive(false);
        transferOutUI.SetActive(true);

        playerController.ChangePerspective(PerspectiveModes._3rdPerson);
    }

    void TransferOut()
    {
        if (transferredObject == null)
            return;

        transferredObject.transform.SetParent(originalParent);

        transform.position =
            transferredObject.transform.position
            - transferredObject.transform.forward * exitDistance;

        transferredObject = null;
        originalParent = null;

        isTransferred = false;

        transferOutUI.SetActive(false);

        playerController.ChangePerspective(PerspectiveModes._1stPerson);
    }
}