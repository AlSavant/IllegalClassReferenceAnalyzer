# IllegalClassReferenceAnalyzer
Diagnostics Analyzer that prohibits use of referenced Types in dependency injection.

This analyzer will mark as error all references to a specified list of Types. The specified list can be provided per project using this analyzer as an AdditionalFile. The structure of the file must be as such:

```
<?xml version="1.0" encoding="utf-8" ?>
<AnalyzerSettings>
	<ForbiddenType>System.Net.Http.IHttpClientFactory</ForbiddenType>
</AnalyzerSettings>
```

This AdditionalFile will prohibit all references to System.Net.Http.IHttpClientFactory. Additional ForbiddenType elements can be inserted as needed.

AdditionalFile entries should be added to the project's csproj file as such:

```
<ItemGroup>
	<AdditionalFiles Include="Settings\IllegalTypes.xml" />
</ItemGroup>
```

The analyzer will iterate all AdditionalFiles and parse the ones that have the compatible structure above.

In some cases, for multi project solutions, one or more projects may be referencing the same additional file. In cases where some of these projects need to exclude one or more type from the common AdditionalFile, they can have secondary exclusive AdditionalFiles only referenced by these projects, using the AllowedType node:

```
<?xml version="1.0" encoding="utf-8" ?>
<AnalyzerSettings>
	<AllowedType>System.Net.Http.IHttpClientFactory</AllowedType>
</AnalyzerSettings>
```

