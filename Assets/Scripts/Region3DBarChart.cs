using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Region3DBarChart : MonoBehaviour
{
    // Reference to the 3D Pie Chart component
    public WorldSpaceBarChart barChart3D;
    public BarAnimation barAnimation;

    public Material[] materials;

    private void Awake()
    {
        barChart3D = GetComponentInChildren<WorldSpaceBarChart>();
        barAnimation = GetComponentInChildren<BarAnimation>();
        barAnimation.AnimationTime = 1f;
        barChart3D.enabled = false;
    }

    // Call this method when a region is selected
    public void Update3DPieChart(Dictionary<string, int> populationData)
    {
        //Debug.Log($"[Update3DPieChart] Updating 3DPiechart...");
        // Clear existing data
        ClearChart();
        barChart3D.enabled = true;

        // Get the total population from the populationData dictionary
        int totalPopulation = populationData.Values.Sum();
        int matIndex = 0;
        //Debug.Log($"[Update3DPieChart] totalPopulation: {totalPopulation}");

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
                double percentage = (double)count / totalPopulation * 100f;
                //Debug.Log($"[Update3DPieChart] percentage: {percentage}");
                barChart3D.DataSource.AddCategory(species, materials[matIndex]);
                barChart3D.DataSource.SetValue(species, "Population" , percentage);
            }
            matIndex++;
        }

        barAnimation.Animate();
    }

    public void ClearChart()
    {
        // Clear existing data
        barChart3D.DataSource.ClearCategories();
    }
}
