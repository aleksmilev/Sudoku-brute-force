#!/bin/bash

dotnet build
xterm -e "dotnet run"
