param (
    [switch]
    $Interactive = $false,
    [switch]
    $Compress = $false,
    [string]
    $Version = ""
)

#Functions
#From https://stackoverflow.com/a/69964408
function Copy-Folder {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [String]$FromPath,

        [Parameter(Mandatory)]
        [String]$ToPath,

        [string[]] $Exclude
    )

    if (Test-Path $FromPath -PathType Container) {
        New-Item $ToPath -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
        Get-ChildItem $FromPath -Force | ForEach-Object {
            # avoid the nested pipeline variable
            $item = $_
            $target_path = Join-Path $ToPath $item.Name
            if (($Exclude | ForEach-Object { $item.Name -like $_ }) -notcontains $true) {
                if (Test-Path $target_path) { Remove-Item $target_path -Recurse -Force }
                Copy-Item $item.FullName $target_path
                Copy-Folder -FromPath $item.FullName $target_path $Exclude
            }
        }
    }
}

function New-AssemblyDefintion {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [String]$Path,
        [Parameter(Mandatory)]
        [String]$Name,
        [String]$RootNamespace,
        [String[]]$References = @(),
        [String[]]$IncludePlatforms = @(),
        [String[]]$ExcludePlatforms = @(),
        [Switch]$AllowUnsafeCode = $false,
        [Switch]$OverrideReferences = $false,
        [String[]]$PrecompiledReferences = @(),
        [Switch]$NoAutoReferenced = $false,
        [String[]]$DefineConstraints = @(),
        [String[]]$VersionDefines = @(),
        [Switch]$NoEngineReferences = $false
    )

    $Json = ConvertTo-Json @{
        name = $Name;
        rootNamespace = $RootNamespace;
        references = $References;
        includePlatforms = $IncludePlatforms;
        excludePlatforms = $ExcludePlatforms;
        allowUnsafeCode = $AllowUnsafeCode.IsPresent;
        overrideReferences = $OverrideReferences.IsPresent;
        precompiledReferences = $PrecompiledReferences;
        autoReferenced = -not $NoAutoReferenced.IsPresent;
        defineConstraints = $DefineConstraints;
        versionDefines = $VersionDefines;
        noEngineReferences = $NoEngineReferences.IsPresent;
    } -Compress

    [IO.File]::WriteAllLines($Path, $Json)
}

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
    if ("$(git -C $PSScriptRoot describe --tags --always)" -match $FindSemVer) {
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

if (Test-Path $packageOutput) {
    Remove-Item $packageOutput -Recurse
}

#Create Folders
$packageLocation = "$packageOutput\Behaviour Tree"
New-Item -Path "$packageLocation\Editor" -ItemType "Directory" | Out-Null
New-Item -Path "$packageLocation\Runtime" -ItemType "Directory" | Out-Null

#Copy code
$exlusions = @( "obj", "bin", "*.csproj" )
Copy-Folder -FromPath "$PSScriptRoot\BehaviourTrees.Core" -ToPath "$packageLocation\Runtime\BehaviourTrees.Core" -Exclude $exlusions
Copy-Folder -FromPath "$PSScriptRoot\BehaviourTrees.Model" -ToPath "$packageLocation\Runtime\BehaviourTrees.Model" -Exclude $exlusions
Copy-Folder -FromPath "$PSScriptRoot\BehaviourTrees.UnityEditor" -ToPath "$packageLocation\Runtime\BehaviourTrees.UnityEditor" -Exclude $exlusions

#Create assembly definitions
New-AssemblyDefintion -Path "$packageLocation\Runtime\BehaviourTrees.Core\BehaviourTrees.Core.asmdef" -Name "technologicalMayhem.BehaviourTrees.Core" -NoEngineReferences
New-AssemblyDefintion -Path "$packageLocation\Runtime\BehaviourTrees.Model\BehaviourTrees.Model.asmdef" -Name "technologicalMayhem.BehaviourTrees.Model" -NoEngineReferences -References @( "technologicalMayhem.BehaviourTrees.Core" )
New-AssemblyDefintion -Path "$packageLocation\Runtime\BehaviourTrees.UnityEditor\BehaviourTrees.UnityEditor.asmdef" -Name "technologicalMayhem.BehaviourTrees.UnityEditor" -References @( "technologicalMayhem.BehaviourTrees.Core", "technologicalMayhem.BehaviourTrees.Model" )

#Copy info files
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