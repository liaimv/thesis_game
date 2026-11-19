using System.Collections.Generic;
using UnityEngine;

public class MaterialScrambleLogic : MonoBehaviour
{
    [System.Serializable]
    public class MaterialData
    {
        public GameObject obj;
        public Material correctMaterial;
    }

    [Header("Rooms")]
    public GameObject[] rooms;
    public string[] roomAreaTags;

    [Header("Player (not parented under any room)")]
    public Transform player;
    public int CurrentRoomIndex { get; private set; } = 0;

    public List<MaterialData> objects = new List<MaterialData>();

    void Start()
    {
        SaveOriginalMaterials();
        ScrambleAllRooms();
        foreach (MaterialData data in objects)
            SetEffectsActive(data.obj, false);

        CurrentRoomIndex = 0;
    }

    void SaveOriginalMaterials()
    {
        objects.Clear();
        foreach (GameObject room in rooms)
        {
            if (room == null) continue;
            foreach (Transform child in room.GetComponentsInChildren<Transform>(true))
            {
                GameObject obj = child.gameObject;
                if (obj == room || obj.layer != LayerMask.NameToLayer("Interactable")) continue;
                if (IsEffectRenderer(child)) continue;

                Transform parent = child.parent;
                bool hasInteractableParent = false;
                while (parent != null && parent != room.transform)
                {
                    if (parent.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                    {
                        hasInteractableParent = true;
                        break;
                    }
                    parent = parent.parent;
                }
                if (hasInteractableParent) continue;

                Renderer rend = GetObjectRenderer(obj);
                if (rend != null && rend.sharedMaterial != null)
                    objects.Add(new MaterialData { obj = obj, correctMaterial = rend.sharedMaterial });
            }
        }
        Debug.Log("Saved " + objects.Count + " puzzle objects.");
    }

    void ScrambleAllRooms()
    {
        foreach (GameObject room in rooms)
            if (room != null) ScrambleRoom(room);
    }

    void ScrambleRoom(GameObject room)
    {
        List<MaterialData> roomObjects = objects.FindAll(
            data => data.obj != null && data.obj.transform.IsChildOf(room.transform));
        if (roomObjects.Count < 2)
        {
            Debug.LogWarning(room.name + " needs at least two puzzle objects to scramble.");
            return;
        }

        List<Material> materials = new List<Material>();
        foreach (MaterialData data in roomObjects) materials.Add(data.correctMaterial);

        bool valid = false;
        for (int attempt = 0; attempt < 1000 && !valid; attempt++)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                int j = Random.Range(i, materials.Count);
                Material temp = materials[i];
                materials[i] = materials[j];
                materials[j] = temp;
            }
            valid = true;
            for (int i = 0; i < roomObjects.Count; i++)
            {
                if (materials[i] == roomObjects[i].correctMaterial)
                {
                    valid = false;
                    break;
                }
            }
        }

        if (!valid)
        {
            Debug.LogError("Could not scramble " + room.name + ". Too many objects may share one material.");
            return;
        }

        for (int i = 0; i < roomObjects.Count; i++)
            SetObjectMaterial(roomObjects[i].obj, materials[i]);
    }

    public GameObject GetRegisteredPuzzleObject(GameObject obj)
    {
        if (obj == null) return null;
        Transform current = obj.transform;
        while (current != null)
        {
            foreach (MaterialData data in objects)
                if (data.obj == current.gameObject) return data.obj;
            current = current.parent;
        }
        return null;
    }

    public bool IsTextureCorrect(GameObject obj)
    {
        GameObject root = GetRegisteredPuzzleObject(obj);
        if (root == null) return false;
        foreach (MaterialData data in objects)
        {
            if (data.obj != root) continue;
            Renderer rend = GetObjectRenderer(root);
            return rend != null && rend.sharedMaterial == data.correctMaterial;
        }
        return false;
    }

    public Material GetCorrectMaterial(GameObject obj)
    {
        GameObject root = GetRegisteredPuzzleObject(obj);
        foreach (MaterialData data in objects)
            if (data.obj == root) return data.correctMaterial;
        return null;
    }

    // Returns true if this transform is part of an Effect hierarchy.
    public bool IsEffectRenderer(Transform transform)
    {
        Transform current = transform;

        while (current != null)
        {
            if (current.CompareTag("Effect") ||
                current.CompareTag("Collider"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    Renderer GetObjectRenderer(GameObject obj)
    {
        foreach (Renderer rend in obj.GetComponentsInChildren<Renderer>(true))
            if (!IsEffectRenderer(rend.transform)) return rend;
        return null;
    }

    void SetObjectMaterial(GameObject obj, Material material)
    {
        foreach (Renderer rend in obj.GetComponentsInChildren<Renderer>(true))
        {
            if (IsEffectRenderer(rend.transform)) continue;
            Material[] materials = rend.sharedMaterials;
            for (int i = 0; i < materials.Length; i++) materials[i] = material;
            rend.sharedMaterials = materials;
        }
    }

    void SetEffectsActive(GameObject obj, bool active)
    {
        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
            if (child != obj.transform && child.CompareTag("Effect"))
                child.gameObject.SetActive(active);
    }

    public void UpdateEffects(GameObject obj)
    {
        GameObject root = GetRegisteredPuzzleObject(obj);
        if (root != null) SetEffectsActive(root, IsTextureCorrect(root));
    }

    public bool IsObjectInCurrentRoom(GameObject obj)
    {
        if (CurrentRoomIndex < 0 ||
            CurrentRoomIndex >= rooms.Length)
        {
            return false;
        }

        GameObject root = GetRegisteredPuzzleObject(obj);

        if (root == null)
            return false;

        return root.transform.IsChildOf(
            rooms[CurrentRoomIndex].transform
        );
    }

    // Find the room index associated with a doorway area tag
    public int GetRoomIndexFromTag(string areaTag)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (i >= roomAreaTags.Length)
                continue;

            if (rooms[i] == null)
                continue;

            if (roomAreaTags[i] == areaTag)
            {
                return i;
            }
        }

        return -1;
    }

    // Change the current room without teleporting the player
    public void SetCurrentRoom(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= rooms.Length)
            return;

        CurrentRoomIndex = roomIndex;

        Debug.Log("Current Room: " +
            rooms[CurrentRoomIndex].name);
    }
}
