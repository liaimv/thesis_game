using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Turns the properties of physical material into wet or dry on Player character material (for example Girl _AlbedoM material on Women_mesh in the hierarchy)

public class RainMaterialParameter : MonoBehaviour {

    public Renderer RendMat;

    //WantedParam_M - wanted metalic value, WantedParam_S - wanted smooth value | limit parameters values with EndWantedParam_M and EndWantedParam_S
    public float TimeParam_Metalic, TimeParam_Smooth, WantedParam_M, WantedParam_S, EndWantedParam_M, EndWantedParam_S;

    public bool Rainy; //gets from RainCameraTrigger script

    public int ReadySet = 0; //waiting state until all parameters will correct

    void Update () {

        if (Rainy)
        {
            TimeParam_Metalic += Time.deltaTime/20;
            TimeParam_Smooth += Time.deltaTime/20;

            RendMat.material.SetFloat("_Metallic", TimeParam_Metalic);
            RendMat.material.SetFloat("_Glossiness", TimeParam_Smooth);

            if (TimeParam_Metalic >= WantedParam_M)
            {
                
                ReadySet = 1;
                TimeParam_Metalic = WantedParam_M;
            }

            if (TimeParam_Smooth >= WantedParam_S)
            {
                
                ReadySet = 2;
                TimeParam_Smooth = WantedParam_S;
            }

            if (ReadySet == 2)
            {
                ReadySet = 0;
                //Rainy = false;
            }

        }

        if (!Rainy)
        {
            TimeParam_Metalic -= Time.deltaTime / 20;
            TimeParam_Smooth -= Time.deltaTime / 20;

            RendMat.material.SetFloat("_Metallic", TimeParam_Metalic);
            RendMat.material.SetFloat("_Glossiness", TimeParam_Smooth);

            if (TimeParam_Metalic <= EndWantedParam_M)
            {

                ReadySet = 1;
                TimeParam_Metalic = EndWantedParam_M;
            }

            if (TimeParam_Smooth <= EndWantedParam_S)
            {

                ReadySet = 2;
                TimeParam_Smooth = EndWantedParam_S;
            }

            if (ReadySet == 2)
            {
                ReadySet = 0;
            }

        }

    }
}
