[T4Scaffolding.Scaffolder(Description = "Scaffold Search CRUD")][CmdletBinding()]
param(     
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ModelType,
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ControllerName,
    [string]$Project,
	[string]$CodeLanguage,
	[switch]$NoChildItems = $false,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[switch]$ReferenceScriptLibraries = $false,
	[string]$DbContextType,
	[string]$Area
)

$outputPath = "Models\Search_{0}DTO" -f $ModelType  # The filename extension will be added based on the template's <#@ Output Extension="..." #> directive
$search_outputPath = "Views\{0}\Search" -f $ControllerName
$searchResult_outputPath = "Views\{0}\SearchResults" -f $ControllerName
$results_outputPath = "Views\{0}\Results" -f $ControllerName
$indexSearch_outputPath = "Views\{0}\Index" -f $ControllerName
$createOrEdit_outputPath = "Views\{0}\CreateOrEdit" -f $ControllerName
$delete_outputPath = "Views\{0}\Delete" -f $ControllerName
$list_outputPath = "Views\{0}\List" -f $ControllerName
$metadata_outputPath = "Models\EntityMetadata\{0}" -f $ModelType
$repository_outputPath = "Models\Repository_{0}" -f $ModelType
$controller_outputPath = "Controllers\{0}Controller" -f $ControllerName
$details_outpathPath = "Views\{0}\Details" -f $ControllerName

if (!$NoChildItems) {
	$dbContextScaffolderResult = Scaffold DbContext -ModelType $ModelType -DbContextType $DbContextType -Area $Area -Project $Project -CodeLanguage $CodeLanguage
	$foundDbContextType = $dbContextScaffolderResult.DbContextType
	if (!$foundDbContextType) { return }
}
if (!$foundDbContextType) { $foundDbContextType = Get-ProjectType $DbContextType -Project $Project }
if (!$foundDbContextType) { return }

# ====  Search DTO       ======================================================================================================================================

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

$foundModelType = Get-ProjectType $ModelType -Project $Project

$primaryKey = Get-PrimaryKey $foundModelType.FullName -Project $Project -ErrorIfNotFound

if (!$primaryKey) { return }

Write-Host "Create Search DTO Class"

Add-ProjectItemViaTemplate $outputPath -Template SearchDTO `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKey = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Search View       ======================================================================================================================================

$searchModelType = "Search_{0}DTO" -f $ModelType
$foundSearchModelType = Get-ProjectType $searchModelType -Project $Project

Write-Host "Create Search View"

Add-ProjectItemViaTemplate $search_outputPath -Template Search `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKey = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Paginated Search Results View     ======================================================================================================================================

$searchModelType = "Search_{0}DTO" -f $ModelType
$foundSearchModelType = Get-ProjectType $searchModelType -Project $Project

Write-Host "Create Search View"

Add-ProjectItemViaTemplate $searchResult_outputPath -Template SearchResults `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKey = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Page of Search Results View     ======================================================================================================================================

$searchModelType = "Search_{0}DTO" -f $ModelType
$foundSearchModelType = Get-ProjectType $searchModelType -Project $Project

Write-Host "Create Results View"

Add-ProjectItemViaTemplate $results_outputPath -Template Results `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKey = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Index View     ======================================================================================================================================

Write-Host "Create Index Search View"

Add-ProjectItemViaTemplate $indexSearch_outputPath -Template Index `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKey = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Controller Class     ======================================================================================================================================

Write-Host "Create Controller Class"

# Prepare all the parameter values to pass to the template, then invoke the template with those values
$repositoryName = "Repository_" + $foundModelType.Name
$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)
$controllerNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($controller_outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
$areaNamespace = if ($Area) { [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + ".Areas.$Area") } else { $defaultNamespace }
$dbContextNamespace = $foundDbContextType.Namespace.FullName
$repositoriesNamespace = [T4Scaffolding.Namespaces]::Normalize($areaNamespace + ".Models")
$modelTypePluralized = Get-PluralizedWord $foundModelType.Name
$relatedEntities = [Array](Get-RelatedEntities $foundModelType.FullName -Project $project)
if (!$relatedEntities) { $relatedEntities = @() }


Add-ProjectItemViaTemplate $controller_outputPath -Template Controller `
	-Model @{ 
		Controller = $ControllerName;
		ControllerName = $ControllerName + "Controller";
		ModelType = [MarshalByRefObject]$foundModelType; 
		PrimaryKey = [string]$primaryKey; 
		DefaultNamespace = $defaultNamespace; 
		AreaNamespace = $areaNamespace; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		RepositoriesNamespace = $repositoriesNamespace;
		ModelTypeNamespace = $modelTypeNamespace; 
		ControllerNamespace = $controllerNamespace; 
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		DbContextName = [string]$DbContextType;
		Repository = $repositoryName;
		ModelTypePluralized = [string]$modelTypePluralized; 
		RelatedEntities = $relatedEntities;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Metadata Class     ======================================================================================================================================

Write-Host "Create Metadata Class"

Add-ProjectItemViaTemplate $metadata_outputPath -Template Metadata `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKeyName = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		ReferenceScriptLibraries = $ReferenceScriptLibraries.ToBool();
		RelatedEntities = $relatedEntities;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  CreateOrEdit View     ======================================================================================================================================

Write-Host "Create CreateOrEdit View"

Add-ProjectItemViaTemplate $createOrEdit_outputPath -Template CreateOrEdit `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKeyName = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		ViewName = [string]"CreateOrEdit";
		ReferenceScriptLibraries = $ReferenceScriptLibraries.ToBool();
		RelatedEntities = $relatedEntities;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Delete View     ======================================================================================================================================

Write-Host "Create Delete View"
if ($foundModelType) { $relatedEntities = [Array](Get-RelatedEntities $foundModelType.FullName -Project $project) }
if (!$relatedEntities) { $relatedEntities = @() }

Add-ProjectItemViaTemplate $delete_outputPath -Template Delete `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKeyName = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		ViewName = [string]"Delete";
		ReferenceScriptLibraries = $ReferenceScriptLibraries.ToBool();
		RelatedEntities = $relatedEntities;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Details View     ======================================================================================================================================

Write-Host "Create Details View"
if ($foundModelType) { $relatedEntities = [Array](Get-RelatedEntities $foundModelType.FullName -Project $project) }
if (!$relatedEntities) { $relatedEntities = @() }

Add-ProjectItemViaTemplate $details_outpathPath -Template Details `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKeyName = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		ViewName = [string]"Details";
		ReferenceScriptLibraries = $ReferenceScriptLibraries.ToBool();
		RelatedEntities = $relatedEntities;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  List View     ======================================================================================================================================

Write-Host "Create List View"

Add-ProjectItemViaTemplate $list_outputPath -Template List `
	-Model @{ 
		Namespace = $namespace; 
		ModelType = [MarshalByRefObject]$foundModelType; 	
		ModelTypeName = $ModelType;
		PrimaryKeyName = [string]$primaryKey;
		ControllerName = [string]$ControllerName;
		DbContextName = [string]$DbContextType;
		ViewName = [string]"List";
		ReferenceScriptLibraries = $ReferenceScriptLibraries.ToBool();
		RelatedEntities = $relatedEntities;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

# ====  Repository Class     ======================================================================================================================================

Write-Host "Create Repository Class"

$modelTypePluralized = Get-PluralizedWord $foundModelType.Name
$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
$repositoryNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($repository_outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)

Add-ProjectItemViaTemplate $repository_outputPath -Template Repository `
	-Model @{ 
		ModelType = [MarshalByRefObject]$foundModelType; 
		PrimaryKey = [string]$primaryKey; 
		DefaultNamespace = $defaultNamespace; 
		RepositoryNamespace = $repositoryNamespace; 
		ModelTypeNamespace = $modelTypeNamespace; 
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		DbContextName = [string]$DbContextType;
		ControllerName = $ControllerName;
		SearchModelType = [MarshalByRefObject]$foundSearchModelType;
	} `
	-SuccessMessage "Added Search output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

Write-Host "Completed"