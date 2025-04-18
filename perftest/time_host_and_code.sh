#!/bin/sh
set -e
cd "$(dirname "$0")"
dotnet publish -r linux-x64 -c Release -p:DefineConstants=SLIM
exe="bin/Release/net10.0/linux-x64/publish/TASDDotnet.Tests"
time "$exe"
time "$exe"
time "$exe"
