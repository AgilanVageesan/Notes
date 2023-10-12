## Step 1: Create the PowerShell Script

1. Create a new PowerShell script file (e.g., `script.ps1`) using a text editor or PowerShell ISE.

```powershell
# Ask for the user's name
$name = Read-Host "What is your name?"

# Ask for the user's age
$age = Read-Host "How old are you?"

# Ask for the user's location
$location = Read-Host "Where are you located?"

# Print the inputs
Write-Host "Your name is: $name"
Write-Host "Your age is: $age"
Write-Host "You are located in: $location"
```

## Step 2: Create the C# Application

1. Open Visual Studio or your preferred C# development environment.

2. Create a new C# project or use an existing one.

3. In your C# project, create a class for calling the PowerShell script. For example, create a class named `PowerShellCaller`.

```csharp
using System;
using System.Diagnostics;

public class PowerShellCaller
{
    public void RunPowerShellScript(string name, age, location)
    {
        string scriptPath = "C:\path\to\your\script.ps1"; // Replace with the actual path to your script
        string arguments = $"-ExecutionPolicy Bypass -NoProfile -File "{scriptPath}" "{name}" "{age}" "{location}"";

        ProcessStartInfo psi = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = psi;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output); // Display the output of the PowerShell script
        }
    }

    public static void Main(string[] args)
    {
        PowerShellCaller caller = new PowerShellCaller();
        caller.RunPowerShellScript("John Doe", "30", "New York");
    }
}
```

In the `RunPowerShellScript` method, replace "C:\path\to\your\script.ps1" with the actual path to your PowerShell script. This method runs the PowerShell script, passing input parameters, and captures the script's output.

## Step 3: Run the C# Application

1. Build and run your C# application. The `Main` method in your `PowerShellCaller` class demonstrates how to call the `RunPowerShellScript` method with example input values.

2. Observe the C# application's output, which will display the output of the PowerShell script after running it.

That's it! You have successfully integrated a PowerShell script with a C# application, allowing you to pass input parameters and capture the script's output.

## Conclusion

This tutorial has shown you how to combine PowerShell and C# to create a powerful automation and scripting solution. You can customize the PowerShell script and the C# application to suit your specific needs and workflow, making it a versatile tool for various tasks.

Happy coding!
