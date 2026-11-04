using UnityEngine;

public class CreateBoundObject : MonoBehaviour
{
    void Awake()
    {
        GameObject[] boxObjects = GameObject.FindGameObjectsWithTag("Box");
        foreach (GameObject obj in boxObjects)
        {
            CreateBox(obj);
        }

        GameObject[] capsuleObjects = GameObject.FindGameObjectsWithTag("Capsule");
        foreach (GameObject obj in capsuleObjects)
        {
            CreateBox(obj);
        }

        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject obj in doorObjects)
        {
            CreateBox(obj);
        }

        GameObject[] helmetObjects = GameObject.FindGameObjectsWithTag("Helmet");
        foreach (GameObject obj in helmetObjects)
        {
            CreateBox(obj);
        }
    }

    void CreateBox(GameObject target)
    {
        Renderer rend = target.GetComponentInChildren<Renderer>();

        if (rend == null)
            return;

        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);

        box.name = "ColliderDisplay";
        box.transform.SetParent(rend.transform, false);

        box.transform.localPosition = rend.localBounds.center;
        box.transform.localRotation = Quaternion.identity;

        // 5% bigger on width, height, and depth
        box.transform.localScale = rend.localBounds.size * 1.05f;

        box.tag = "Collider";
    }

    void CreateCapsule(GameObject target)
    {
        Renderer rend = target.GetComponentInChildren<Renderer>();

        if (rend == null)
            return;

        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

        capsule.name = "ColliderDisplay";
        capsule.transform.SetParent(target.transform);

        capsule.transform.position = rend.bounds.center;
        capsule.transform.rotation = target.transform.rotation;

        capsule.transform.localScale = new Vector3(
            rend.bounds.size.x / target.transform.lossyScale.x,
            rend.bounds.size.y / (2f * target.transform.lossyScale.y),
            rend.bounds.size.z / target.transform.lossyScale.z
        );

        capsule.tag = "Collider";
    }
}
