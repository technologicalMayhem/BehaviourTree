try {
    Write-Host "Dotnet version $(dotnet --version)";
}
catch {
    Write-Host "Dotnet not intalled.";
    Exit;
}

#Get version
$version = Read-Host -Prompt "Package version (SemVer)";

#Clear package directory
$packageLocation = "$PSScriptRoot\packageOutput\Behaviour Tree"

if (Test-Path $packageLocation) {
    Remove-Item $packageLocation -Recurse
}

#Build project
dotnet build "$PSScriptRoot\BehaviourTrees.UnityEditor" -c Release -o "$packageLocation\Editor"

#Copy files over
Copy-Item -Path @( "$PSScriptRoot\README.md" ) -Destination $packageLocation

#Create package.json
$json = ConvertTo-Json @{
    name = "com.github.technologicalmayhem.behaviourtree";
    version = $version;
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
};

$json | Out-File -FilePath "$packageLocation\package.json" -Encoding utf8

Write-Host "Package created.";