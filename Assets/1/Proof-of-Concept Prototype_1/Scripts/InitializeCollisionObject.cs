using UnityEngine;

public class InitializeCollisionObject : MonoBehaviour
{
    void Awake()
    {
        // Include inactive objects in other rooms
        Transform[] allObjects =
            FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Transform child in allObjects)
        {
            GameObject obj = child.gameObject;

            if (obj.CompareTag("Box"))
            {
                CreateCollider(obj, PrimitiveType.Cube);
            }
            else if (obj.CompareTag("Capsule"))
            {
                CreateCollider(obj, PrimitiveType.Capsule);
            }
            else if (obj.CompareTag("Sphere"))
            {
                CreateCollider(obj, PrimitiveType.Sphere);
            }
        }
    }

    // =========================================
    // CREATE COLLIDER
    // =========================================

    void CreateCollider(GameObject target, PrimitiveType type)
    {
        // Get all renderers, including inactive children
        Renderer[] renderers =
            target.GetComponentsInChildren<Renderer>(true);

        bool hasBounds = false;
        Bounds combinedBounds = new Bounds();

        foreach (Renderer rend in renderers)
        {
            // Ignore Effect objects and their children
            if (IsEffectObject(rend.transform))
                continue;

            if (!hasBounds)
            {
                combinedBounds = rend.bounds;
                hasBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(rend.bounds);
            }
        }

        if (!hasBounds)
        {
            Debug.LogWarning(
                "No renderer found for " + target.name
            );
            return;
        }

        // Create one overarching collider display
        GameObject colliderObject =
            GameObject.CreatePrimitive(type);

        colliderObject.name = "ColliderDisplay";

        // Parent under target
        colliderObject.transform.SetParent(
            target.transform,
            false
        );

        // Convert world bounds center to local position
        colliderObject.transform.localPosition =
            target.transform.InverseTransformPoint(
                combinedBounds.center
            );

        colliderObject.transform.localRotation =
            Quaternion.identity;

        // Convert world bounds size to local scale
        Vector3 parentScale =
            target.transform.lossyScale;

        Vector3 size = new Vector3(
            combinedBounds.size.x /
                Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),

            combinedBounds.size.y /
                Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),

            combinedBounds.size.z /
                Mathf.Max(Mathf.Abs(parentScale.z), 0.0001f)
        );

        // Make slightly bigger
        size *= 1f;

        // Capsule primitive has a height of 2
        if (type == PrimitiveType.Capsule)
        {
            colliderObject.transform.localScale =
                new Vector3(
                    size.x,
                    size.y / 2f,
                    size.z
                );
        }
        else
        {
            colliderObject.transform.localScale = size;
        }

        colliderObject.tag = "Collider";
        colliderObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Debug.Log(
            "Created " + type +
            " collider for " + target.name +
            " using " + renderers.Length + " renderers."
        );
    }

    // =========================================
    // CHECK EFFECT OBJECTS
    // =========================================

    bool IsEffectObject(Transform obj)
    {
        Transform current = obj;

        while (current != null)
        {
            if (current.CompareTag("Effect"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
