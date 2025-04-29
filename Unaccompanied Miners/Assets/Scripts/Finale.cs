using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Script for the ending screen
public class Finale : MonoBehaviour
{
    // Reference to text and main menu button
    public TMP_Text textMesh;
    public Button mainMenu;

    //Data fro the text
    Mesh mesh;
    Vector3[] vertices;

    //Length of words in the text
    List<int> wordIndexes;
    List<int> wordLengths;

    //Gradient in order to make it rainbow
    public Gradient rainbow;

    void Start()
    {
        wordIndexes = new List<int> { 0 };
        wordLengths = new List<int>();

        string s = textMesh.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
            wordIndexes.Add(index + 1);
        }
        wordLengths.Add(s.Length - wordIndexes[wordIndexes.Count - 1]);
        mainMenu.onClick.AddListener(MainMenu);
    }

    void Update()
    {
        //Grabs the mesh data from the text and places the color within it
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        Color[] colors = mesh.colors;

        //For every word garbs each letter and applys a rainbow affect on all 4 vertices (saw it online)
        for (int w = 0; w < wordIndexes.Count; w++)
        {
            int wordIndex = wordIndexes[w];

            for (int i = 0; i < wordLengths[w]; i++)
            {
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[wordIndex + i];

                int index = c.vertexIndex;

                colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x * 0.001f, 1f));
                colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x * 0.001f, 1f));
                colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x * 0.001f, 1f));
                colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x * 0.001f, 1f));
            }
        }
        //Sets the mesh with rainbow coloring
        mesh.vertices = vertices;
        mesh.colors = colors;
        textMesh.canvasRenderer.SetMesh(mesh);
    }
    //Method to take the player back to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
