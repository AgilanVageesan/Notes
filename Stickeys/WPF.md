### WPF Save data in Local
1. Save Key-Value Pairs to JSON File:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public void SaveKeyValuePairsToJson(Dictionary<string, object> keyValuePairs, string filePath)
{
    // Serialize the dictionary to JSON and write it to the file
    File.WriteAllText(filePath, JsonSerializer.Serialize(keyValuePairs));
}
```

2. Fetch Key-Value Pairs from JSON File:

```csharp
public Dictionary<string, object> LoadKeyValuePairsFromJson(string filePath)
{
    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

    if (File.Exists(filePath))
    {
        // Read the JSON data from the file and deserialize it into the dictionary
        string jsonData = File.ReadAllText(filePath);
        keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
    }

    return keyValuePairs;
}
```

3. Usage remains the same as previously shown:

```csharp
string filePath = "keyValueData.json";
Dictionary<string, object> dataToSave = new Dictionary<string, object>
{
    { "Name", "John" },
    { "Age", 30 },
    { "IsStudent", false }
};

// Save key-value pairs to JSON file
SaveKeyValuePairsToJson(dataToSave, filePath);

// Load key-value pairs from JSON file
Dictionary<string, object> loadedData = LoadKeyValuePairsFromJson(filePath);

// Access specific values by key
if (loadedData.ContainsKey("Name"))
{
    string name = (string)loadedData["Name"];
    Console.WriteLine($"Name: {name}");
}
```

Using `System.Text.Json` is a more optimized approach because it's part of the .NET Core and .NET 5+ frameworks, and it provides good performance for JSON serialization and deserialization without the need for external libraries.
