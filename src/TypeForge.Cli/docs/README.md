# TypeForge

TypeForge is a powerful tool designed to map C# classes to TypeScript types. Whether you're developing an API or building a complex application, TypeForge provides an efficient solution to maintain consistent models across your C# backend and TypeScript frontend.

## Features
* Transform C# classes into TypeScript types or interfaces
* Control casing of filenames, typenames, and property names
* Adjust the depth of directory traversal
* Flatten the file structure if needed
* Maintain or remove the root folder
* Handle nullable types
* Generate an index file for easy exports
* Option to end lines with semicolons

## CLI Arguments

The CLI tool supports the following arguments:

| Argument | Description                                               |
|----------|-----------------------------------------------------------|
| input    | Directory of files to convert. Leave empty to use config. |
| output   | Directory to output files to. Leave empty to use config.  |

Additionally, the CLI supports the following options:

[//]: # ([Type or Interface]&#40;#type-or-interface--tm---typemodel&#41;: Specifies whether the generated TypeScript code should represent types or interfaces.)


| Option                                                                             | Description                                                                 |
|------------------------------------------------------------------------------------|-----------------------------------------------------------------------------|
| [fc, fileNameCase](#file-name-case--fc---fileNameCase)                             | File name case. Default = KebabCase. Example = kebab-case                   |
| [fp, fileNamePrefix](#file-name-prefix--fp---fileNamePrefix)                       | File name prefix, Default = null. Example = I                               |
| [fs, fileNameSuffix](#file-name-suffix--fs---fileNameSuffix)                       | File name suffix, Default = null. Example = Model                           |
| [fd, folderNameCase](#folder-name-case--fd---folderNameCase)                       | Folder name case, Default = KebabCase                                       |
| [tm, typeModel](#type-or-interface--tm---typeModel)                                | Type or Interface. Default = Type                                           |
| [tc, typeNameCase](#type-name-case--tc---typeNameCase)                             | Type name case. Default = PascalCase                                        |
| [tp, typeNamePrefix](#type-name-prefix--tp---typeNamePrefix)                       | Type name prefix. Default = null. Example = I                               |
| [ts, typeNameSuffix](#type-name-suffix--ts---typeNameSuffix)                       | Type name suffix. Default = null. Example = Model                           |
| [pc, propertyNameCase](#property-name-case--pc---propertyNameCase)                 | Property name case. Default = CamelCase                                     |
| [nt, nullableType](#nullable-types--nt---nullableType)                             | Nullable type. Default = QuestionMark. QuestionMark or Null Or Undefined    |
| [gi, generateIndexFile](#generate-index-file--gi---generateIndexFile)              | Generate an index file to export all generated files. Default = true        |
| [es, endLinesWithSemicolon](#end-lines-with-semicolon--es---endLinesWithSemiColon) | End lines with semicolon. Default = false                                   |
| [ic, includeChildren](#include-children--ic---includeChildren)                     | Include children. Default = true                                            |
| [d, depth](#depth--d---depth)                                                      | Depth. Default = -1; -1 = infinite; IncludeChildren must be true            |
| [f, flatten](#flatten--f---flatten)                                                | Flatten. Default = false; If true, all files will be in the same folder     |
| [kr, keepRootFolder](#keep-root-folder--kr---keepRootFolder)                       | Keep root folder. Default = true; If false, the root folder will be removed |


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
  "EndLinesWithSemicolon": false,
  "Directories": [
    {
      "Input": "C:/directory/to/backend/TestNamespace",
      "IncludeChildren": true,
      "Flatten": false,
      "Depth": -1,
      "KeepRootFolder": true,
      "Output": "C:/directory/to/frontend/output"
    },
    {
      "Input": "C:/directory/to/backend/OtherNameSpace",
      "IncludeChildren": true,
      "Flatten": false,
      "Depth": -1,
      "KeepRootFolder": true,
      "Output": "C:/directory/to/frontend/output"
    },
    {
      "Input": "C:/directory/to/backend/OtherNameSpaceTest",
      "IncludeChildren": true,
      "Flatten": false,
      "Depth": -1,
      "KeepRootFolder": true,
      "Output": "C:/directory/to/frontend/output2"
    }
  ]
}
```

This file is JSON formatted and it is very easy to adapt to your needs. You can define multiple directory mappings with specific options, as well as global settings for the tool.


---

### Type or Interface (`-tm, --typeModel`)

The `typeModel` option determines whether to use TypeScript's `type` or `interface` for the generated TypeScript definition. Here are the available options:

- `"Type"` - Uses `type` for the TypeScript definition.
- `"Interface"` - Uses `interface` for the TypeScript definition.

Here are examples of how this would affect your output:

#### Using `"Type"`

```typescript
// When TypeModel === "Type"
export type Example = {
    property: string
}
```

#### Using `"Interface"`

```typescript
// When TypeModel === "Interface"
export interface Example {
    property: string
}
```

---
### Nullable Types (`-nt, --nullableType`)

The `nullableType` option determines how nullable properties are represented in the generated types. Here are the available options:

- `"?"` - Adds an optional property operator (`?`) to the property.
- `"Undefined"` - Makes the property a union with `undefined`.
- `"Null"` - Makes the property a union with `null`.

Here are examples of how this would affect your output:

#### Using `"?"`

```typescript
// When NullableType === "?"
export type Example = {
    property?: string
}
```

#### Using `"Undefined"`

```typescript
// When NullableType === "Undefined"
export type Example = {
    property: string | undefined
}
```

#### Using `"Null"`

```typescript
// When NullableType === "Null"
export type Example = {
    property: string | null
}
```

---
### End Lines With Semicolon (`-es, --endLinesWithSemicolon`)

The `endLinesWithSemicolon` option determines whether to end lines with a semicolon in the generated TypeScript definition. Here are the available options:

- `true` - Ends lines with a semicolon.
- `false` - Does not end lines with a semicolon.

Here are examples of how this would affect your output:

#### With `true`

```typescript
// When EndLinesWithSemicolon === true
export type Example = {
    property: string;
}
```

#### With `false`

```typescript
// When EndLinesWithSemicolon === false
export type Example = {
    property: string
}
```

---
### Property Name Case (`-pc, --propertyNameCase`)

The `propertyNameCase` option determines the case of property names in the generated TypeScript definition. Here are the available options:

- `CamelCase` - Makes property names camel cased.
- `PascalCase` - Makes property names Pascal cased.
- `KebabCase` - Makes property names kebab cased.
- `SnakeCase` - Makes property names snake cased.

Here are examples of how this would affect your output:

#### Camel Case

```typescript
// When PropertyNameCase === "CamelCase"
export type Example = {
    propertyName: string
}
```

#### Pascal Case

```typescript
// When PropertyNameCase === "PascalCase"
export type Example = {
    PropertyName: string
}
````

#### Kebab Case

```typescript
// When PropertyNameCase === "KebabCase"
export type Example = {
    'property-name': string
}
```

#### Snake Case

```typescript
// When PropertyNameCase === "SnakeCase"
export type Example = {
    property_name: string
}
```

Please note that in TypeScript, Kebab Case and Snake Case would require accessing the properties using bracket notation. For example, for Kebab Case, you would need to access the `property-name` like so: `example['property-name']`.

---
### File Name Prefix (`-fp, --fileNamePrefix`)

The `fileNamePrefix` option lets you specify a prefix that will be added to the beginning of all file names.

Consider a scenario where you have a file named `example.ts`. If you set the `fileNamePrefix` to "I", the output file would be named `Iexample.ts`.

Here is an example:

#### Without Prefix

```typescript
// When FileNamePrefix is empty
// File name: example.ts
export type Example = {
    propertyName: string
}
```

#### With Prefix

```typescript
// When FileNamePrefix === "I"
// File name: Iexample.ts
export type Example = {
    propertyName: string
}
```

Remember that the value you set for `fileNamePrefix` will be prepended to all generated file names.

---
### File Name Suffix (`-fs, --fileNameSuffix`)

The `fileNameSuffix` option allows you to specify a suffix that will be added to the end of all file names.

Consider a scenario where you have a file named `example.ts`. If you set the `fileNameSuffix` to "model", the output file would be named `example-model.ts`.

Here is an example:

#### Without Suffix

```typescript
// File: example.ts
export type Example = {
    propertyName: string
}
```

#### With Suffix

```typescript
// File: example-model.ts
export type Example = {
    propertyName: string
}
```

Remember that the value you set for `fileNameSuffix` will be appended to all generated file names.

---
### Folder Name Case (`-fd, --folderNameCase`)

The `folderNameCase` option allows you to specify the case format for folder names.

Consider a scenario where you have a folder named `MyFolder` and you set the `folderNameCase` to "KebabCase". The output folder name would be `my-folder`.

Here are examples of different folder name cases:


#### KebabCase (Default)

Input:
```
FolderNameCase: "KebabCase"
Folder: MyFolder
```

Output:
```
Folder: my-folder
```

#### CamelCase

Input:
```
FolderNameCase: "CamelCase"
Folder: MyFolder
```

Output:
```
Folder: myFolder
```

#### PascalCase

Input:
```
FolderNameCase: "PascalCase"
Folder: MyFolder
```

Output:
```
Folder: MyFolder
```

Remember that the `folderNameCase` option defines the case format for all generated folder names.

---

### Type Name Prefix (`-tp, --typeNamePrefix`)

The `typeNamePrefix` option lets you specify a prefix that will be added to the beginning of all type names.

Consider a scenario where you have a type named `Example`. If you set the `typeNamePrefix` to "I", the output type would be named `IExample`.

Here is an example:

#### Without Prefix

```typescript
// When TypeNamePrefix is empty
export type Example = {
    propertyName: string;
};
```

#### With Prefix

```typescript
// When TypeNamePrefix === "I"
export type IExample = {
    propertyName: string;
};
```

Remember that the value you set for `typeNamePrefix` will be prepended to all generated type names.

---
### Type Name Suffix (`-ts, --typeNameSuffix`)

The `typeNameSuffix` option allows you to specify a suffix that will be added to the end of all type names.

Consider a scenario where you have a type named `Example`. If you set the `typeNameSuffix` to "Model", the output type would be named `ExampleModel`.

Here is an example:

#### Without Suffix

```typescript
// When TypeNameSuffix is empty
export type Example = {
    propertyName: string
}
```

#### With Suffix

```typescript
// When TypeNameSuffix === "Model"
export type ExampleModel = {
    propertyName: string
}
```

Remember that the value you set for `typeNameSuffix` will be appended to all generated type names.

---
### Generate Index File (`-gi, --generateIndexFile`)

The `generateIndexFile` option controls whether an index file should be generated to export all the generated files.

When `generateIndexFile` is set to `true`, an index file will be created to provide a convenient way to import all the generated files in one place.

Here is an example of how the index file is generated:

```typescript
// Generated Index File

export * from './file1'
export * from './file2'
export * from './file3'
// ... and so on
```

By default, `generateIndexFile` is set to `true`, but you can disable it by setting it to `false` if you don't want an index file to be generated.

---

### Directory Specific Options

---
### Include Children (`-ic, --includeChildren`)

The `includeChildren` option controls whether to include files from all the subdirectories when processing a directory.

By default, `includeChildren` is set to `true`, which means that when you specify a directory, all the files in that directory and its subdirectories will be processed.

Here is an example to illustrate how `includeChildren` works with a folder structure:

In this example
``` json
"Directories": [
  {
    "Input": "C:/directory/to/c#project/Requests",
    "Output": "C:/directory/to/frontend/output/"
  },
  {
    "Input": "C:/directory/to/c#project/Responses",
    "Output": "C:/directory/to/frontend/output/"
  }
]
```

```
C:/directory/to/backend/
├── Requests
│   ├── File1.cs
│   ├── File2.cs
│   └── SubNamespace
│       ├── File3.cs
│       └── File4.cs
└── Responses
    ├── File5.cs
    └── SubNamespace
        ├── File6.cs
        └── File7.cs
```

#### Example 1: `includeChildren = true`

If `includeChildren` is set to `true`, all the files in the specified directory and its subdirectories will be processed:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   ├── file-2.ts
│   └── sub-namespace
│       ├── index.ts
│       ├── file-3.ts
│       └── file-4.ts
└── responses
    ├── index.ts
    ├── file-5.ts
    └── sub-namespace
        ├── index.ts
        ├── file-6.ts
        └── file-7.ts
```

#### Example 2: `includeChildren = false`

If `includeChildren` is set to `false`, only the files in the specified directory will be processed, and the files in the subdirectories will be excluded:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   └── file-2.ts
└── responses
    ├── index.ts
    └── file-5.ts
```

---
### Depth (`-d, --depth`)

The `depth` option allows you to control the number of levels of subdirectories to include when processing a directory. It works in conjunction with the `includeChildren` option.

By default, `depth` is set to `-1`, which means that all subdirectories will be processed.

Here is an example to illustrate how `depth` works with a folder structure:

In this example
``` json
"Directories": [
  {
    "Input": "C:/directory/to/c#project/Requests",
    "IncludeChildren": true,
    "Output": "C:/directory/to/frontend/output/"
  },
  {
    "Input": "C:/directory/to/c#project/Responses",
    "IncludeChildren": true,
    "Output": "C:/directory/to/frontend/output/"
  }
]
```

```
C:/directory/to/backend/
├── Requests
│   ├── File1.cs
│   ├── File2.cs
│   └── SubNamespace
│       ├── File3.cs
│       ├── File4.cs
│       └── SubSubNamespace
│           ├── File5.cs
│           └── File6.cs
└── Responses
    ├── File7.cs
    └── SubNamespace
        ├── File8.cs
        ├── File9.cs
        └── SubSubNamespace
            ├── File10.cs
            └── File11.cs
```

#### Example 1: `depth = -1` (Default)

If `depth` is set to `-1`, all subdirectories will be processed:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   ├── file-2.ts
│   └── sub-namespace
│       ├── index.ts
│       ├── file-3.ts
│       ├── file-4.ts
│       └── sub-sub-namespace
│           ├── index.ts
│           ├── file-5.ts
│           └── file-6.ts
└── responses
    ├── index.ts
    ├── file-7.ts
    └── sub-namespace
        ├── index.ts
        ├── file-8.ts
        ├── file-9.ts
        └── sub-sub-namespace
            ├── index.ts
            ├── file-10.ts
            └── file-11.ts
```

#### Example 2: `depth <= 1`

If `depth` is less than or equal to `1`, only the files in the specified directory will be processed:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   └── file-2.ts
└── responses
    ├── index.ts
    └── file-5.ts
```

#### Example 3: `depth = 2`

If `depth` is set to `2`, the files in the specified directory and its immediate subdirectories will be processed:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   ├── file-2.ts
└── responses
    ├── index.ts
    ├── file-5.ts
    └─── sub-namespace
        ├── index.ts
        ├── file-6.ts
        └── file-7.ts
```
Note: The `depth` option works in conjunction with the `includeChildren` option. If `includeChildren` is set to `false`, the `depth` setting will have no effect, as only the specified directory will be processed.

---
### Flatten (`-f, --flatten`)

The `flatten` option controls whether to flatten the output structure and place all generated files in the same directory, regardless of their original directory structure.

By default, `flatten` is set to `false`, which means that the generated files will be organized in the same folder structure as the input files.

Here is an example to illustrate how `flatten` works with a folder structure:

In this example
``` json
"Directories": [
  {
    "Input": "C:/directory/to/backend/Requests",
    "Output": "C:/directory/to/frontend/output/"
  },
  {
    "Input": "C:/directory/to/backend/Responses",
    "Output": "C:/directory/to/frontend/output/"
  }
]
```

```
C:/directory/to/backend/
├── Requests
│   ├── File1.cs
│   ├── File2.cs
│   └── SubNamespace
│       ├── File3.cs
│       └── File4.cs
└── Responses
    ├── File5.cs
    └── SubNamespace
        ├── File6.cs
        └── File7.cs
```

#### Example 1: `flatten = false`

If `flatten` is set to `false`, the generated files will be organized in the same folder structure as the input files:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   ├── file-2.ts
│   └── sub-namespace
│       ├── index.ts
│       ├── file-3.ts
│       └── file-4.ts
└── responses
    ├── index.ts
    ├── file-5.ts
    └── sub-namespace
        ├── index.ts
        ├── file-6.ts
        └── file-7.ts
```

#### Example 2: `flatten = true`

If `flatten` is set to `true`, all the generated files will be placed in the same directory, regardless of their original directory structure:

```
C:/directory/to/frontend/output/
├── index.ts
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   ├── file-2.ts
│   ├── file-3.ts
│   └── file-4.ts
└── responses
    ├── index.ts
    ├── file-5.ts
    ├── file-6.ts
    └── file-7.ts
```
Note: The `flatten` option can be useful if you prefer a flat output structure and do not need to preserve the original directory hierarchy of the input files.

---
### Keep Root Folder (`-kr, --keepRootFolder`)

The `keepRootFolder` option controls whether to keep the root folder of the input files in the output structure.

By default, `keepRootFolder` is set to `true`, which means that the root folder of each input directory will be preserved in the output structure.

Here is an example to illustrate how `keepRootFolder` works with a folder structure:

In this example
``` json
"Directories": [
  {
    "Input": "C:/directory/to/backend/Requests",
    "Output": "C:/directory/to/frontend/output/"
  },
  {
    "Input": "C:/directory/to/backend/Responses",
    "Output": "C:/directory/to/frontend/output/"
  }
]
```

```
C:/directory/to/backend/
├── Requests
│   ├── File1.cs
│   ├── File2.cs
│   └── SubNamespace
│       ├── File3.cs
│       └── File4.cs
└── Responses
    ├── File5.cs
    └── SubNamespace
        ├── File6.cs
        └── File7.cs
```

#### Example 1: `keepRootFolder = true`

If `keepRootFolder` is set to `true`, the root folder of each input directory will be preserved in the output structure:

```
C:/directory/to/frontend/output/
├── requests
│   ├── index.ts
│   ├── file-1.ts
│   ├── file-2.ts
│   └── sub-namespace
│       ├── index.ts
│       ├── file-3.ts
│       └── file-4.ts
└── responses
    ├── index.ts
    ├── file-5.ts
    └── sub-namespace
        ├── index.ts
        ├── file-6.ts
        └── file-7.ts
```

#### Example 2: `keepRootFolder = false`

If `keepRootFolder` is set to `false`, the root folder of each input directory will not be included in the output structure:

```
C:/directory/to/frontend/output/
├── index.ts
├── file-1.ts
├── file-2.ts
├── file-5.ts
├── sub-namespace
│   ├── index.ts
│   ├── file-3.ts
│   └── file-4.ts
└── sub-namespace
    ├── index.ts
    ├── file-6.ts
    └── file-7.ts
```
Note: The `keepRootFolder` option can be useful if you want to maintain the original directory structure of the input files in the output, or if you prefer a flatter output structure by excluding the root folders.

---


