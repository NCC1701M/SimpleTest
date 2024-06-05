#!/bin/bash

cwd=$(pwd)

if [ -x "$(command -v dotnet)" ]; then
	dotnet tool install --global dotnet-ef

	cd /workspaces/SimpleTest
	dotnet restore
fi
