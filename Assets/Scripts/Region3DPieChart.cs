using ChartAndGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Region3DPieChart : MonoBehaviour
{
    // Reference to the 3D Pie Chart component
    public WorldSpacePieChart pieChart3D;
    public PieAnimation pieAnimation;

    public Material[] materials;

    private void Awake()
    {
        //pieAnimation = GetComponentInChildren<PieAnimation>();
        //pieAnimation.AnimationTime = 1f;
    }

    // Call this method when a region is selected
    public void Update3DPieChart(Dictionary<string, int> populationData)
    {
        Debug.Log($"[Update3DPieChart] Updating 3DPiechart...");
        // Clear existing data
        ClearChart();

        // Optional: Adjust 3D-specific settings such as rotation or scale here if needed
        pieChart3D.Radius = 1;
        pieChart3D.InnerDepth = 0.2f;
        pieChart3D.OuterDepth = 0.2f;

        // Get the total population from the populationData dictionary
        int totalPopulation = populationData.Values.Sum();
        int matIndex = 0;

        // Add new data to the 3D pie chart
        foreach (var kvp in populationData)
        {
            if (matIndex >= materials.Length - 1)
                matIndex = 0;

            string species = kvp.Key; // The key is the species name
            int count = kvp.Value; // The value is the population count for that species

            // Check to avoid division by zero
            if (totalPopulation > 0)
            {
                float percentage = (float)count / totalPopulation * 100f;

                pieChart3D.DataSource.AddCategory(species, materials[matIndex]);
                pieChart3D.DataSource.SlideValue(species, percentage, 1);
            }
            matIndex++;
        }

        pieAnimation.Animate();
    }

    public void ClearChart()
    {
        // Clear existing data
        pieChart3D.DataSource.Clear();
    }
}