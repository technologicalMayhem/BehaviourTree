param (
    [switch]
    $Interactive = $false,
    [switch]
    $Compress = $false,
    [string]
    $Version = ""
)

try {
    Write-Host "Dotnet version $(dotnet --version)";
}
catch {
    Write-Host "Dotnet not intalled.";
    Exit 1;
}

#Get version
$FindSemVer = "v(\d+\.\d+\.\d+)";
if ($Version -eq "") {
    if ("$(git describe --tags --always)" -match $FindSemVer) {
        $Version = $Matches[1]
    }
    else {
        if ($Interactive) {
            $Version = Read-Host -Prompt "Package version (SemVer)"
        }
        else {
            Write-Host "Could not find a fitting version number."
            Exit 1
        }
    }
}

#Clear package directory
$packageOutput = "$PSScriptRoot\packageOutput"
$packageLocation = "$packageOutput\Behaviour Tree"

if (Test-Path $packageOutput) {
    Remove-Item $packageOutput -Recurse
}

#Build project
dotnet build "$PSScriptRoot\BehaviourTrees.UnityEditor" -c Release -o "$packageLocation\Editor"

#Copy files over
Copy-Item -Path @( "$PSScriptRoot\README.md" ) -Destination $packageLocation
Copy-Item -Path @( "$PSScriptRoot\LICENSE" ) -Destination "$packageLocation\LICENSE.md"

#Create package.json
$json = ConvertTo-Json @{
    name = "com.github.technologicalmayhem.behaviourtree";
    version = $Version;
    description = "Tools to create, edit and run behaviour trees.";
    displayName = "Behaviour Tree";
    unity = "2021.2";
    author = @{
        name = "Tobias Freese";
        email = "wolfshund98@gmail.com";
        url = "https://github.com/technologicalMayhem/"
    };
    dependencies = @{
        "com.unity.nuget.newtonsoft-json" = "3.0.1";
    };
} -Compress

[IO.File]::WriteAllLines("$packageLocation\package.json", $json)

if ($Compress) {
    tar -czf "$packageOutput\Behaviour Tree.tar.gz" -C "$packageLocation" *
}

Write-Host "Package creation sucessful."