# TASD .NET

Library for handling input movies in the new [TASD format](https://github.com/tasd-org/tasd-spec).
Built with C# 14, targeting .NET Standard 2.0 and above.

The library is currently unfinished.

# Building

Use the standard `dotnet build` from the `/src` directory.

If you have [Nix](https://nixos.org), run `nix-shell`/`nix develop` to get the .NET SDK (and a copy of the Kate editor with a broken LSP -_-).

There are unit tests in `/test` which you can run with `dotnet test`,
and `/perftest` has a shell script for quantifying performance improvements, but I think AoT compilation has made it too fast to measure confidently.

# Licence

[Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0)
