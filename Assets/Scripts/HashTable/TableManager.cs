using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{
    public GameObject indexUiPrefab;
    public ScrollRect scrollRect;

    public TMP_Dropdown hashTablesDropdown;
    public TMP_Dropdown probingStrategyDropdown;

    public TMP_InputField keyInputField;
    public TMP_InputField valueInputField;

    public Button addButton;
    public Button removeButton;
    public Button clearButton;

    public TextMeshProUGUI infoText;

    public Color occupiedColor = new Color(0.3f, 0.7f, 0.3f);
    public Color emptyColor = new Color(0.7f, 0.7f, 0.7f);
    public Color deletedColor = new Color(0.9f, 0.5f, 0.3f);
    public Color collisionColor = new Color(0.9f, 0.3f, 0.3f);

    private enum HashTableType
    {
        Simple,
        Chaining,
        OpenAddressing
    }

    private HashTableType currentTableType = HashTableType.Simple;
    private ProbingStrategy currentProbingStrategy = ProbingStrategy.Linear;

    private SimpleHashTable<string, string> simpleTable;
    private ChainingHashTable<string, string> chainingTable;
    private OpenAddressingHashTable<string, string> openAddressingTable;

    private List<GameObject> indexUiObjects = new List<GameObject>();

    private Transform ContentTransform => scrollRect.content;

    void Start()
    {
        simpleTable = new SimpleHashTable<string, string>();
        chainingTable = new ChainingHashTable<string, string>();
        openAddressingTable = new OpenAddressingHashTable<string, string>(currentProbingStrategy);

        hashTablesDropdown.ClearOptions();
        hashTablesDropdown.AddOptions(new List<string> { "Simple Hash Table", "Chaining Hash Table", "Open Addressing" });
        hashTablesDropdown.onValueChanged.AddListener(OnHashTableTypeChanged);

        probingStrategyDropdown.ClearOptions();
        probingStrategyDropdown.AddOptions(new List<string> { "Linear Probing", "Quadratic Probing", "Double Hashing" });
        probingStrategyDropdown.onValueChanged.AddListener(OnProbingStrategyChanged);
        probingStrategyDropdown.gameObject.SetActive(false);

        addButton.onClick.AddListener(OnAddButtonClicked);
        removeButton.onClick.AddListener(OnRemoveButtonClicked);
        clearButton.onClick.AddListener(OnClearButtonClicked);

        UpdateVisualization();
    }

    void OnHashTableTypeChanged(int index)
    {
        currentTableType = (HashTableType)index;

        probingStrategyDropdown.gameObject.SetActive(currentTableType == HashTableType.OpenAddressing);

        UpdateVisualization();
    }

    void OnProbingStrategyChanged(int index)
    {
        currentProbingStrategy = (ProbingStrategy)index;
        openAddressingTable = new OpenAddressingHashTable<string, string>(currentProbingStrategy);

        UpdateVisualization();
    }

    void OnAddButtonClicked()
    {
        string key = keyInputField.text.Trim();
        string value = valueInputField.text.Trim();

        if (string.IsNullOrEmpty(key))
        {
            ShowInfo("키 중복!", Color.red);
            return;
        }

        try
        {
            switch (currentTableType)
            {
                case HashTableType.Simple:
                    simpleTable.Add(key, value);
                    break;
                case HashTableType.Chaining:
                    chainingTable.Add(key, value);
                    break;
                case HashTableType.OpenAddressing:
                    openAddressingTable.Add(key, value);
                    break;
            }

            ShowInfo($"Added: {key} = {value}", Color.green);
            keyInputField.text = "";
            valueInputField.text = "";
            UpdateVisualization();
        }
        catch (System.Exception e)
        {
            ShowInfo($"Error: {e.Message}", Color.red);
        }
    }

    void OnRemoveButtonClicked()
    {
        string key = keyInputField.text.Trim();

        if (string.IsNullOrEmpty(key))
        {
            ShowInfo("키 중복!", Color.red);
            return;
        }

        bool removed = false;
        switch (currentTableType)
        {
            case HashTableType.Simple:
                removed = simpleTable.Remove(key);
                break;
            case HashTableType.Chaining:
                removed = chainingTable.Remove(key);
                break;
            case HashTableType.OpenAddressing:
                removed = openAddressingTable.Remove(key);
                break;
        }

        if (removed)
        {
            ShowInfo($"Removed: {key}", Color.yellow);
            keyInputField.text = "";
            UpdateVisualization();
        }
        else
        {
            ShowInfo($"Key not found: {key}", Color.red);
        }
    }

    void OnClearButtonClicked()
    {
        switch (currentTableType)
        {
            case HashTableType.Simple:
                simpleTable.Clear();
                break;
            case HashTableType.Chaining:
                chainingTable.Clear();
                break;
            case HashTableType.OpenAddressing:
                openAddressingTable.Clear();
                break;
        }

        ShowInfo("Table cleared!", Color.cyan);
        UpdateVisualization();
    }

    void UpdateVisualization()
    {
        foreach (var obj in indexUiObjects)
        {
            Destroy(obj);
        }
        indexUiObjects.Clear();

        switch (currentTableType)
        {
            case HashTableType.Simple:
                VisualizeSimpleTable();
                break;
            case HashTableType.Chaining:
                VisualizeChainingTable();
                break;
            case HashTableType.OpenAddressing:
                VisualizeOpenAddressingTable();
                break;
        }
    }

    void VisualizeSimpleTable()
    {
        var tableField = typeof(SimpleHashTable<string, string>).GetField("table", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var occupiedField = typeof(SimpleHashTable<string, string>).GetField("occuiped", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sizeField = typeof(SimpleHashTable<string, string>).GetField("size", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var table = (KeyValuePair<string, string>[])tableField.GetValue(simpleTable);
        var occupied = (bool[])occupiedField.GetValue(simpleTable);
        int size = (int)sizeField.GetValue(simpleTable);

        for (int i = 0; i < size; i++)
        {
            GameObject indexObj = Instantiate(indexUiPrefab, ContentTransform);
            IndexUi indexUi = indexObj.GetComponent<IndexUi>();

            if (occupied[i])
            {
                indexUi.keyValueText.text = $"I:{i} K:{table[i].Key} V:{table[i].Value}";
                //indexUi.keyValueText.color = occupiedColor;
                indexUi.background.color = occupiedColor;
            }
            else
            {
                indexUi.keyValueText.text = $"I:{i}";
                //indexUi.keyValueText.color = emptyColor;
                indexUi.background.color = Color.white;
            }

            indexUiObjects.Add(indexObj);
        }

        ShowInfo($"Count: {simpleTable.Count} / Size: {size}", Color.white);
    }

    void VisualizeChainingTable()
    {
        var tableField = typeof(ChainingHashTable<string, string>).GetField("table",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sizeField = typeof(ChainingHashTable<string, string>).GetField("size",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var table = (LinkedList<KeyValuePair<string, string>>[])tableField.GetValue(chainingTable);
        int size = (int)sizeField.GetValue(chainingTable);

        for (int i = 0; i < size; i++)
        {
            GameObject indexObj = Instantiate(indexUiPrefab, ContentTransform);
            IndexUi indexUi = indexObj.GetComponent<IndexUi>();

            if (table[i] != null && table[i].Count > 0)
            {
                string chainText = $"I:{i} ";
                foreach (var kvp in table[i])
                {
                    chainText += $"K:{kvp.Key} V:{kvp.Value} → ";
                }
                chainText = chainText.TrimEnd(' ', '→', ' ');

                indexUi.keyValueText.text = chainText;
                indexUi.background.color = table[i].Count > 1 ? collisionColor : occupiedColor;
            }
            else
            {
                indexUi.keyValueText.text = $"I:{i}";
                //indexUi.keyValueText.color = emptyColor;
                indexUi.background.color = Color.white;
            }

            indexUiObjects.Add(indexObj);
        }

        ShowInfo($"Count: {chainingTable.Count} / Size: {size}", Color.white);
    }

    void VisualizeOpenAddressingTable()
    {
        var tableField = typeof(OpenAddressingHashTable<string, string>).GetField("table", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var occupiedField = typeof(OpenAddressingHashTable<string, string>).GetField("occupied", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var deletedField = typeof(OpenAddressingHashTable<string, string>).GetField("deleted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var sizeField = typeof(OpenAddressingHashTable<string, string>).GetField("size", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var table = (KeyValuePair<string, string>[])tableField.GetValue(openAddressingTable);
        var occupied = (bool[])occupiedField.GetValue(openAddressingTable);
        var deleted = (bool[])deletedField.GetValue(openAddressingTable);
        int size = (int)sizeField.GetValue(openAddressingTable);

        for (int i = 0; i < size; i++)
        {
            GameObject indexObj = Instantiate(indexUiPrefab, ContentTransform);
            IndexUi indexUi = indexObj.GetComponent<IndexUi>();

            if (occupied[i] && !deleted[i])
            {
                indexUi.keyValueText.text = $"I:{i} K:{table[i].Key} V:{table[i].Value}";
                //indexUi.keyValueText.color = occupiedColor;
                indexUi.background.color = occupiedColor;
            }
            else if (deleted[i])
            {
                indexUi.keyValueText.text = $"[{i}] Deleted";
                indexUi.keyValueText.color = deletedColor;
            }
            else
            {
                indexUi.keyValueText.text = $"I:{i}";
                //indexUi.keyValueText.color = emptyColor;
                indexUi.background.color = Color.white;
            }

            indexUiObjects.Add(indexObj);
        }

        ShowInfo($"Count: {openAddressingTable.Count} / Size: {size}", Color.white);
    }

    void ShowInfo(string message, Color color)
    {
        if (infoText != null)
        {
            infoText.text = message;
            infoText.color = color;
        }
    }
}

