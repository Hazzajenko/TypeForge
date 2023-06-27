help:
	dotnet run -- -?
	
pack:
	dotnet pack

pack-update:
	dotnet pack /p:Version=${ver} --version-suffix alpha

pack-release:
	dotnet pack -c Release /p:Version=${ver}

update:
	dotnet pack /p:Version=${ver} --version-suffix alpha
	dotnet tool update --global --add-source ./nupkg TypeForge --version ${ver}


install:
	dotnet tool install --global --add-source ./nupkg TypeForge.Cli

install-ver:
	dotnet tool install --global --add-source ./nupkg TypeForge --version ${ver}

uninstall:
	dotnet tool uninstall --global TypeForge

reset:
	dotnet tool uninstall --global TypeForge
	dotnet pack
	dotnet tool install --global --add-source ./nupkg TypeForge