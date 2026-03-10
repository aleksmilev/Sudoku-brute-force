#!/bin/bash

dotnet build
xterm -T "Sudoku Brute Force" -e "dotnet run"
