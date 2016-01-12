[T4Scaffolding.Scaffolder(Description = "Enter a description of Metadata here")][CmdletBinding()]
param(  
    [parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ModelType,      
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$outputPath = "Models\EntityMetadata\$ModelType"  # The filename extension will be added based on the template's <#@ Output Extension="..." #> directive
$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

$foundModelType = Get-ProjectType $ModelType -Project $Project

Write-Host "Create Metadata Class"

Add-ProjectItemViaTemplate $outputPath -Template MetadataTemplate `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 
		ModelTypeName = $ModelType; 
	} `
	-SuccessMessage "Added Metadata output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

Write-Host "Completed"