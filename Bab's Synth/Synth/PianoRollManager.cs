using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PianoRollManager : MonoBehaviour
{
    public GameObject keyButtonPrefab;
    public int numberOfKeys = 88;
    public int numberOfSteps = 16;
    public Synthesizer synthesizer;
    public Color playingColor = Color.red;
    public Color activeColor = Color.green;
    public Color inactivePlayingColor = Color.yellow;
    public Color defaultColor = Color.white;
    public Toggle loopToggle;
    public TMP_InputField bpmInputField;

    private List<List<bool>> pianoRoll;
    private Button[,] keyButtons;
    private GridLayoutGroup gridLayout;
    private bool isPlaying = false;
    private static List<PianoRollManager> allPianoRollManagers = new List<PianoRollManager>();
    private float bpm = 120f;

    void Awake()
    {
        allPianoRollManagers.Add(this);
    }

    void OnDestroy()
    {
        allPianoRollManagers.Remove(this);
    }

    void Start()
    {
        InitializePianoRoll();
        PopulatePianoRollUI();
        InitializeBPMInputField();
    }

    void InitializePianoRoll()
    {
        pianoRoll = new List<List<bool>>();
        for (int i = 0; i < numberOfKeys; i++)
        {
            List<bool> keyTrack = new List<bool>();
            for (int j = 0; j < numberOfSteps; j++)
            {
                keyTrack.Add(false);
            }
            pianoRoll.Add(keyTrack);
        }
    }

    void PopulatePianoRollUI()
    {
        keyButtons = new Button[numberOfKeys, numberOfSteps];
        gridLayout = GetComponentInChildren<GridLayoutGroup>();

        RectTransform contentTransform = gridLayout.transform.parent.GetComponent<RectTransform>();

        float cellWidth = contentTransform.rect.width / numberOfSteps;
        float cellHeight = 30f;

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = numberOfSteps;

        for (int i = numberOfKeys - 1; i >= 0; i--)
        {
            for (int j = 0; j < numberOfSteps; j++)
            {
                GameObject keyButton = Instantiate(keyButtonPrefab, gridLayout.transform);
                keyButton.name = $"Key_{i}_Step_{j}";
                int keyIndex = i;
                int stepIndex = j;
                keyButton.GetComponent<Button>().onClick.AddListener(() => ToggleNoteState(keyIndex, stepIndex));
                keyButtons[keyIndex, stepIndex] = keyButton.GetComponent<Button>();
            }
        }
    }

    void ToggleNoteState(int key, int step)
    {
        pianoRoll[key][step] = !pianoRoll[key][step];
        UpdateKeyButtonColor(key, step);
    }

    void UpdateKeyButtonColor(int key, int step)
    {
        Color newColor = pianoRoll[key][step] ? activeColor : defaultColor;
        keyButtons[key, step].GetComponent<Image>().color = newColor;
    }

    public void PlayPianoRoll()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            StartCoroutine(PlayPianoRollCoroutine());
        }
    }

    public static void PlayAllPianoRolls()
    {
        foreach (PianoRollManager manager in allPianoRollManagers)
        {
            manager.PlayPianoRoll();
        }
    }

    public void StopPianoRoll()
    {
        isPlaying = false;
        StopAllCoroutines();
        ResetKeyButtonColors();
    }

    public static void StopAllPianoRolls()
    {
        foreach (PianoRollManager manager in allPianoRollManagers)
        {
            manager.StopPianoRoll();
        }
    }

    private IEnumerator PlayPianoRollCoroutine()
    {
        float stepDuration = 60f / bpm / 4f; // Step duration based on BPM (16th notes)

        while (isPlaying)
        {
            for (int step = 0; step < numberOfSteps; step++)
            {
                for (int key = 0; key < numberOfKeys; key++)
                {
                    HighlightPlayingButton(key, step, true);

                    if (pianoRoll[key][step])
                    {
                        float noteFrequency = 440.0f * Mathf.Pow(2, (key - 49) / 12.0f);
                        synthesizer.PlayNote(noteFrequency);
                    }
                }
                yield return new WaitForSeconds(stepDuration);

                for (int key = 0; key < numberOfKeys; key++)
                {
                    HighlightPlayingButton(key, step, false);
                    synthesizer.StopNote();
                }
            }

            if (!loopToggle.isOn)
            {
                isPlaying = false;
            }
        }
    }

    void HighlightPlayingButton(int key, int step, bool isPlaying)
    {
        Color newColor;

        if (isPlaying)
        {
            newColor = pianoRoll[key][step] ? playingColor : inactivePlayingColor;
        }
        else
        {
            newColor = pianoRoll[key][step] ? activeColor : defaultColor;
        }

        keyButtons[key, step].GetComponent<Image>().color = newColor;
    }

    void ResetKeyButtonColors()
    {
        for (int key = 0; key < numberOfKeys; key++)
        {
            for (int step = 0; step < numberOfSteps; step++)
            {
                UpdateKeyButtonColor(key, step);
            }
        }
    }

    void InitializeBPMInputField()
    {
        if (bpmInputField != null)
        {
            bpmInputField.onEndEdit.AddListener(OnBPMInputChanged);
            bpmInputField.text = bpm.ToString();
        }
        else
        {
            Debug.LogError("BPM Input Field is not assigned!");
        }
    }

    void OnBPMInputChanged(string newValue)
    {
        if (float.TryParse(newValue, out float newBPM))
        {
            bpm = newBPM;
            Debug.Log($"BPM changed to: {bpm}");
        }
        else
        {
            Debug.LogError("Invalid BPM value entered!");
        }
    }
}
