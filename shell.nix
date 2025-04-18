{ system ? builtins.currentSystem
/*
, pkgs ? import (builtins.fetchTarball {
	url = "https://github.com/NixOS/nixpkgs/archive/24.11.tar.gz";
	sha256 = "1gx0hihb7kcddv5h0k7dysp2xhf1ny0aalxhjbpj2lmvj7h9g80a";
}) { inherit system; }
*/
, pkgs ? import (builtins.fetchTarball {
	url = "https://github.com/NixOS/nixpkgs/archive/bee6e9aa17a2b5ae91edc07a7f1415dfe3f42b36.tar.gz";
	sha256 = "1q365rk4ha38i7i03mvg3n2g5245awf38yssih5anr3fbj2fbvzw";
}) { inherit system; }
, lib ? pkgs.lib
, fetchpatch ? pkgs.fetchpatch
, dotnet-runtime_10 ? pkgs.dotnet-runtime_10
, dotnet-sdk_10 ? pkgs.dotnet-sdk_10
, kate ? pkgs.libsForQt5.kate/*.overrideAttrs (oldAttrs: {
	patches = (oldAttrs.patches or []) ++ [ (fetchpatch {
		url = "https://invent.kde.org/utilities/kate/-/commit/9ddf4f0c9eb3c26a0ab33c862d2b161bcbdc6a6e.patch"; # Fix name of OmniSharp LSP binary
		hash = "sha256-a2KqoxuuVhfAQUJA3/yEQb1QCoa1JCvLz7BZZnSLnzI=";
	}) ];
})*/
, omnisharp-roslyn ? pkgs.omnisharp-roslyn
, useKate ? true
}: pkgs.mkShell {
	packages = [ dotnet-sdk_10 ]
		++ lib.optionals useKate [ kate omnisharp-roslyn ];
	shellHook = ''
		export DOTNET_ROOT="${dotnet-runtime_10}"
	'';
}
