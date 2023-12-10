{ pkgs ? import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/23.11.tar.gz") {}
}: pkgs.mkShell {
	packages = builtins.attrValues { inherit (pkgs) dotnet-sdk_8 kate; };
	shellHook = ''
		export DOTNET_ROOT="${pkgs.dotnet-runtime_8}"
	'';
}
