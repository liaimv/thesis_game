using UnityEngine;
using System.Collections;

//Animate UVW's of any material for example using for Waterfall mesh

public class UVW_Animated : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";

    Vector2 uvOffset = Vector2.zero;

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (GetComponent<Renderer>().enabled)
        {
            GetComponent<Renderer>().materials[materialIndex].EnableKeyword("_EMISSION");
            GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
    }
}
