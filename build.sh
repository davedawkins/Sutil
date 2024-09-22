#!/bin/sh -x

dotnet tool restore
dotnet run --project EasyBuild/EasyBuild.fsproj -- $@
