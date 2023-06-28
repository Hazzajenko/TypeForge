# TypeForge

TypeForge is a handy utility to convert your C# classes into TypeScript types. It is built with flexibility in mind, providing numerous customization options via CLI arguments or a configuration file.

## Usage
To use TypeForge, you can run the CLI with the appropriate options, or specify your options in a JSON config file.

### CLI Arguments

```
typeforge <directory> <output> [options]
```

- `directory`: The directory of files to convert. Leave empty to use config.
- `output`: The directory to output files to. Leave empty to use config.

Additional options can be specified:

- `-fc, --fileNameCase`: File name case (default: "KebabCase"). Example: "kebab-case"
- `-fp, --fileNamePrefix`: File name prefix (default: null). Example: "I"
- `-fs, --fileNameSuffix`: File name suffix (default: null). Example: "Model"
- `-fd, --folderNameCase`: Folder name case (default: "KebabCase")
- `-tm, --typeModel`: Type or Interface (default: "Type")
- `-tc, --typeNameCase`: Type name case (default: "PascalCase")
- `-tp, --typeNamePrefix`: Type name prefix (default: null). Example: "I"
- `-ts, --typeNameSuffix`: Type name suffix (default: null). Example: "Model"
- `-pc, --propertyNameCase`: Property name case (default: "CamelCase")
- `-nt, --nullableType`: Nullable type (default: "QuestionMark"). Options: "QuestionMark", "Null", or "Undefined"
- `-gi, --generateIndexFile`: Generate an index file to export all generated files (default: true)
- `-gn, --groupByNamespace`: Group by namespace (default: true)
- `-nf, --nameSpaceInOneFile`: Namespace in one file (default: false)
- `-es, --endLinesWithSemicolon`: End lines with semicolon (default: false)

### Config File

Alternatively, you can specify your options in a JSON config file.

```json
{
  "FolderNameCase": "KebabCase",
  "TypeNamePrefix": "",
  "TypeNameSuffix": "Model",
  "FileNamePrefix": "",
  "FileNameSuffix": "Model",
  "ExportModelType": "Type",
  "PropertyNameCase": "CamelCase",
  "FileNameCase": "KebabCase",
  "NullableType": "?",
  "GenerateIndexFile": true,
  "GroupByNameSpace": true,
  "NameSpaceInOneFile": false,
  "EndLinesWithSemicolon": false,
  "NameSpaces": [
    {
      "Name": "TestNamespace",
      "Output": "/output/for/namespace1"
    },
    {
      "Name": "OtherNameSpace",
      "Output": "/output/for/namespace1"
    }
  ]
}
```

## Installation
Instructions on how to install...

## Contributing
Guidelines on how to contribute...

## License
Distributed under the MIT License. See LICENSE for more information.

## Support
Information on how to seek support...

For any additional information or queries, feel free to contact us...