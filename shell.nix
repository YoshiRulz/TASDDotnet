{ pkgs ? import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/23.05.tar.gz") {}
, lib ? pkgs.lib
, dotnet-sdk ? pkgs.dotnet-sdk
, kate ? pkgs.kate
}: pkgs.mkShell { packages = [ dotnet-sdk kate ]; }
