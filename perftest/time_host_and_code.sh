#!/bin/sh
dotnet build -c Release -p:DefineConstants=SLIM && time "$(dirname "$0")/bin/Release/net8.0/PerfTest"
