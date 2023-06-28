help:
	dotnet run -- -?
	
pack:
	dotnet pack

pack-update:
	dotnet pack /p:Version=${ver} --version-suffix alpha

pack-release-ver:
	dotnet pack --configuration Release /p:Version=${ver}

pack-release:
	dotnet pack --configuration Release

update:
	dotnet pack /p:Version=${ver} --version-suffix alpha
	dotnet tool update --global --add-source ./nupkg TypeForge --version ${ver}

publish:
	dotnet nuget push ./nupkg/TypeForge.Cli.1.0.0.nupkg --source https://api.nuget.org/v3/index.json --api-key ${key}

publish-ver:
	dotnet nuget push ./nupkg/TypeForge.Cli.${ver}.nupkg --source https://api.nuget.org/v3/index.json --api-key ${key}

install:
	dotnet tool install --global --add-source ./nupkg TypeForge.Cli

install-ver:
	dotnet tool install --global --add-source ./nupkg TypeForge --version ${ver}

uninstall:
	dotnet tool uninstall --global TypeForge.Cli

reset:
	dotnet tool uninstall --global TypeForge.Cli
	dotnet pack
	dotnet tool install --global --add-source ./nupkg TypeForge.Cli