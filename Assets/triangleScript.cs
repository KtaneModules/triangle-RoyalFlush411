using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class triangleScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] triangleButton;

    private string[] letterOptions = new string[4] {"T", "R", "N", "G"};
    public TextMesh triangleText;
    public Renderer backgroundRend;
    public Material[] backgroundOptions;
    public Animator triangleRotation;

    public Renderer[] triangleColours;
    public Material[] triangleColourOptions;
    public TriangleColour[] triangleNames;
    private string[] triangleColourNames = new string[4] {"blue","green","red","yellow"};
    private List<int> pickedColours = new List<int>();
    private string[] rotationLog = new string[2] {"clockwise","counterclockwise"};
    private string[] backgroundLog = new string[3] {"Picasso","Cool", "Concentric"};

    private int rotation = 0;
    private int background = 0;
    private int letter = 0;

    private string correctColour = "";
    private string pressedColour = "";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable triangle in triangleButton)
        {
            KMSelectable pressedTriangle = triangle;
            triangle.OnInteract += delegate () { TrianglePress(pressedTriangle); return false; };
        }
    }


    void Start()
    {
        SetRules();
        SetColours();
        CalculateColour();
    }

    void SetRules()
    {
        rotation = UnityEngine.Random.Range(0,2);
        if(rotation == 0)
        {
            triangleRotation.ResetTrigger("antiClock");
            triangleRotation.SetTrigger("clock");
        }
        else
        {
            triangleRotation.ResetTrigger("clock");
            triangleRotation.SetTrigger("antiClock");
        }

        background = UnityEngine.Random.Range(0,3);
        backgroundRend.material = backgroundOptions[background];

        letter = UnityEngine.Random.Range(0,4);
        triangleText.text = letterOptions[letter];
        Debug.LogFormat("[The Triangle #{0}] Rotation: {1}. Background: {2}. Letter: {3}.", moduleId, rotationLog[rotation], backgroundLog[background], letterOptions[letter]);
    }

    void SetColours()
    {
        for(int i = 0; i <= 3; i++)
        {
            int colourIndex = UnityEngine.Random.Range(0,4);
            while(pickedColours.Contains(colourIndex))
            {
                colourIndex = UnityEngine.Random.Range(0,4);
            }
            pickedColours.Add(colourIndex);
            triangleColours[i].material = triangleColourOptions[colourIndex];
            triangleNames[i].triangleColour = triangleColourNames[colourIndex];
        }
        pickedColours.Clear();
        Debug.LogFormat("[The Triangle #{0}] Large triangle: {1}. Top left triangle: {2}. Bottom left triangle: {3}. Bottom right triangle: {4}.", moduleId, triangleNames[0].triangleColour, triangleNames[1].triangleColour, triangleNames[2].triangleColour, triangleNames[3].triangleColour);
    }

    void CalculateColour()
    {
        if(rotation == 0)
        {
            if(background == 0)
            {
                if(letter == 0)
                {
                    correctColour = "green";
                }
                else if(letter == 1)
                {
                    correctColour = "red";
                }
                else if(letter == 2)
                {
                    correctColour = "blue";
                }
                else
                {
                    correctColour = "yellow";
                }
            }
            else if(background == 1)
            {
                if(letter == 0)
                {
                    correctColour = "red";
                }
                else if(letter == 1)
                {
                    correctColour = "yellow";
                }
                else if(letter == 2)
                {
                    correctColour = "blue";
                }
                else
                {
                    correctColour = "green";
                }
            }
            else if (background == 2)
            {
                if(letter == 0)
                {
                    correctColour = "blue";
                }
                else if(letter == 1)
                {
                    correctColour = "green";
                }
                else if(letter == 2)
                {
                    correctColour = "red";
                }
                else
                {
                    correctColour = "yellow";
                }
            }
        }
        else if(rotation == 1)
        {
            if(background == 0)
            {
                if(letter == 0)
                {
                    correctColour = "yellow";
                }
                else if(letter == 1)
                {
                    correctColour = "blue";
                }
                else if(letter == 2)
                {
                    correctColour = "green";
                }
                else
                {
                    correctColour = "red";
                }
            }
            else if(background == 1)
            {
                if(letter == 0)
                {
                    correctColour = "green";
                }
                else if(letter == 1)
                {
                    correctColour = "red";
                }
                else if(letter == 2)
                {
                    correctColour = "yellow";
                }
                else
                {
                    correctColour = "blue";
                }
            }
            else if (background == 2)
            {
                if(letter == 0)
                {
                    correctColour = "red";
                }
                else if(letter == 1)
                {
                    correctColour = "blue";
                }
                else if(letter == 2)
                {
                    correctColour = "yellow";
                }
                else
                {
                    correctColour = "green";
                }
            }
        }
        Debug.LogFormat("[The Triangle #{0}] Correct colour: {1}.", moduleId, correctColour);
    }

    void TrianglePress(KMSelectable triangle)
    {
        if(moduleSolved)
        {
            return;
        }
        triangle.AddInteractionPunch();
        pressedColour = triangle.GetComponentInChildren<TriangleColour>().triangleColour;
        if(pressedColour == correctColour)
        {
            Audio.PlaySoundAtTransform("beep", transform);
            triangle.GetComponentInChildren<TriangleColour>().pressed = true;
            Debug.LogFormat("[The Triangle #{0}] You pressed {1}. That is correct.", moduleId, pressedColour);
            if(triangleNames[0].pressed && triangleNames[1].pressed && triangleNames[2].pressed && triangleNames[3].pressed)
            {
                moduleSolved = true;
            }
            if(moduleSolved)
            {
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[The Triangle #{0}] Module disarmed.", moduleId);
                triangleRotation.SetTrigger("solved");
                triangleText.text = "";
                return;
            }
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[The Triangle #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedColour);
        }
        Start();
    }
}
