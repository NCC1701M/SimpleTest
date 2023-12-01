#!/bin/bash

cwd=$(pwd)

if [ -x "$(command -v dotnet)" ]; then
	cd /workspaces/SimpleTest
	dotnet restore
fi
