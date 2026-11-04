using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RaycastLogic : MonoBehaviour
{
    [Header("Raycast")]
    public Camera playerCamera;
    public float interactionDistance = 5f;
    public LayerMask interactableMask;

    [Header("Interaction UI")]
    public GameObject interactionCanvas;
    public TextMeshProUGUI interactionText;

    [Header("Warning UI")]
    public GameObject warningText;
    public TextMeshProUGUI warningTMP;
    public float warningDelay = 2f;

    [Header("Door Confirmation")]
    public GameObject DoorOpenCheck;

    [Header("End")]
    public GameObject EndCanvas;
    public TextMeshProUGUI endResultText;

    [Header("Key")]
    public GameObject keyObject;

    [Header("Virus")]
    public GameObject virusObject;

    [Header("Death")]
    public GameObject deathZone;

    private bool hasKey = false;
    private bool gameEnded = false;

    public GameObject introObject;

    private bool hasStarted = false;
    private bool hasEnded = false;

    // Stores which door the player is deciding whether to open
    private GameObject pendingDoor;

    private Coroutine warningCoroutine;

    void Start()
    {
        interactionCanvas.SetActive(false);
        warningText.SetActive(false);
        DoorOpenCheck.SetActive(false);
        EndCanvas.SetActive(false);
    }

    void Update()
    {
        if (!hasStarted && Input.GetKeyDown(KeyCode.R))
        {
            introObject.SetActive(false);
            hasStarted = true;
        }
        if (hasEnded && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Prototype 4 - Scan View");
        }

        // Stop interaction after game ends
        if (gameEnded)
            return;

        // If confirmation is open, only check Q / R
        if (DoorOpenCheck.activeSelf)
        {
            CheckDoorConfirmation();
            return;
        }

        CheckInteraction();
    }

    void CheckInteraction()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableMask))
        {
            // Green ray = hitting interactable
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            GameObject hitObject = hit.collider.gameObject;

            // ---------------- KEY ----------------
            if (hitObject.CompareTag("Key"))
            {
                interactionCanvas.SetActive(true);
                interactionText.text = "Pick Up";

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickUpKey();
                }
            }

            // ---------------- DOOR ----------------
            else if (hitObject.CompareTag("Door"))
            {
                interactionCanvas.SetActive(true);
                interactionText.text = "Open Door";

                if (Input.GetKeyDown(KeyCode.F))
                {
                    TryOpenDoor(hitObject);
                }
            }

            // ---------------- HELMET ----------------
            else if (hitObject.CompareTag("Helmet"))
            {
                interactionCanvas.SetActive(true);
                interactionText.text = "Destroy Object";

                if (Input.GetKeyDown(KeyCode.F))
                {
                    DestroyHelmet(hitObject);
                }
            }

            else
            {
                interactionCanvas.SetActive(false);
            }
        }
        else
        {
            // Red ray = not hitting interactable
            Debug.DrawRay(
                ray.origin,
                ray.direction * interactionDistance,
                Color.red
            );

            interactionCanvas.SetActive(false);
        }
    }

    // =========================================================
    // WARNING
    // =========================================================

    void ShowWarning(string message)
    {
        // Stop previous warning timer if one is already running
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }

        warningTMP.text = message;
        warningText.SetActive(true);

        warningCoroutine = StartCoroutine(HideWarningAfterDelay());
    }

    IEnumerator HideWarningAfterDelay()
    {
        yield return new WaitForSeconds(warningDelay);

        warningText.SetActive(false);
        warningCoroutine = null;
    }

    // =========================================================
    // KEY
    // =========================================================

    void PickUpKey()
    {
        hasKey = true;

        if (keyObject != null)
        {
            keyObject.SetActive(false);
        }

        interactionCanvas.SetActive(false);

        ShowWarning(
            "This key is very old. It seems like after one use, it will break."
        );
    }

    // =========================================================
    // DOOR
    // =========================================================

    void TryOpenDoor(GameObject doorObject)
    {
        interactionCanvas.SetActive(false);

        // Player does not have the key
        if (!hasKey)
        {
            ShowWarning("I need a key to open the door.");
            return;
        }

        // Player has the key
        pendingDoor = doorObject;

        // Stop any existing warning coroutine
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        warningText.SetActive(true);
        warningTMP.text =
            "This key can only be used once. Are you sure you want to use it to open this door?";

        DoorOpenCheck.SetActive(true);
    }

    void CheckDoorConfirmation()
    {
        // Q = cancel
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DoorOpenCheck.SetActive(false);
            warningText.SetActive(false);

            pendingDoor = null;
        }

        // R = confirm
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (pendingDoor != null)
            {
                OpenDoor(pendingDoor);

                // Key breaks after one use
                hasKey = false;
            }

            DoorOpenCheck.SetActive(false);
            warningText.SetActive(false);

            pendingDoor = null;
        }
    }

    void OpenDoor(GameObject doorObject)
    {
        Animator doorAnimator = doorObject.GetComponent<Animator>();

        if (doorAnimator == null)
        {
            doorAnimator = doorObject.GetComponentInParent<Animator>();
        }

        // Correct room
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("DoorOpen");

            // Prevent this door from being interacted with again
            doorObject.layer = LayerMask.NameToLayer("Default");
        }

        // Wrong room
        else
        {
            EndGame(
                "You entered the wrong room and wasn't able to find the virus!"
            );
        }
    }

    // =========================================================
    // HELMET
    // =========================================================

    void DestroyHelmet(GameObject helmetObject)
    {
        interactionCanvas.SetActive(false);

        // Correct helmet / virus
        if (helmetObject == virusObject)
        {
            EndGame("You correctly destroyed the virus!");
        }

        // Wrong helmet
        else
        {
            EndGame(
                "You destroyed an existing object of the game. You broke the game."
            );
        }

        Destroy(helmetObject);
    }

    // =========================================================
    // DEATH ZONE
    // =========================================================

    void OnTriggerEnter(Collider other)
    {
        if (deathZone != null && other.gameObject == deathZone)
        {
            EndGame("You fell to your death!");
        }
    }

    // =========================================================
    // END GAME
    // =========================================================

    void EndGame(string result)
    {
        gameEnded = true;

        interactionCanvas.SetActive(false);
        warningText.SetActive(false);
        DoorOpenCheck.SetActive(false);

        endResultText.text = result;
        EndCanvas.SetActive(true);
        hasEnded = true;
    }
}
