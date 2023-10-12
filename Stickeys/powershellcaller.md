Creating a PowerShell script that takes three inputs and prints them is a straightforward task. Here's how you can do it, step by step, and then how to call this script from a C# application.

**Step 1: Create the PowerShell Script**

1. Open a text editor, such as Notepad or Visual Studio Code, and create a new file with a `.ps1` extension. Let's name it `PrintInputs.ps1`.

2. Add the following code to your PowerShell script:

```powershell
param(
    [string]$input1,
    [string]$input2,
    [string]$input3
)

Write-Host "Input 1: $input1"
Write-Host "Input 2: $input2"
Write-Host "Input 3: $input3"
```

This script defines three input parameters and then prints them.

**Step 2: Call the PowerShell Script from C#**

Now, let's create a C# program to call this PowerShell script and provide the inputs. You'll need to use the `System.Management.Automation` namespace for this. If you don't already have the `System.Management.Automation` assembly referenced in your project, you can do so in Visual Studio.

Here's a C# program to call the PowerShell script:

```csharp
using System;
using System.Management.Automation;

class Program
{
    static void Main(string[] args)
    {
        // Create an instance of PowerShell
        using (PowerShell ps = PowerShell.Create())
        {
            // Add a script to be executed
            ps.AddScript(".\\PrintInputs.ps1");

            // Provide the input values
            ps.AddParameter("input1", "Value1");
            ps.AddParameter("input2", "Value2");
            ps.AddParameter("input3", "Value3");

            // Execute the script
            var results = ps.Invoke();

            // Check for errors
            if (ps.HadErrors)
            {
                foreach (var error in ps.Streams.Error)
                {
                    Console.WriteLine("PowerShell Error: " + error.ToString());
                }
            }

            // Display script output
            foreach (var result in results)
            {
                Console.WriteLine(result.ToString());
            }
        }
    }
}
```

Make sure you adjust the script file path (`".\\PrintInputs.ps1"`) and the provided input values accordingly. 

**Step 3: Build and Run the C# Program**

Build and run your C# program, and it will call the PowerShell script and display the inputs you provided.

Remember to compile and run the C# program within an environment where PowerShell scripting is allowed (e.g., Windows). This example demonstrates how to call a PowerShell script from C# and pass parameters, which can be useful in various automation and integration scenarios.
