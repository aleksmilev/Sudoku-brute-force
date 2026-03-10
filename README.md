## Sudoku Brute Force Solver (C# Console)

This project is a **Sudoku brute-force solver and grid manager** written in C#.  
It lets you:

- **Start and watch a Sudoku being solved automatically**
- **Generate and save new grids** from an online API
- **Store and reuse multiple puzzles** with different difficulties (Easy / Medium / Hard)
- Interact with everything through a **console menu UI**.

---

## Features

- **Brute-force Sudoku solver**
  - Classic backtracking algorithm.
  - Visualizes each step in the console as numbers are tried and backtracked.

- **Multiple grids management**
  - Keeps a collection of grids using `GridCollection`.
  - Loads three default grids (Easy, Medium, Hard) on startup.
  - Allows you to generate and save new grids with custom names.

- **Console menu interface**
  - Navigate menus with arrow keys and Enter.
  - Choose grids, difficulties, and answer Yes/No prompts interactively.

- **API integration**
  - Fetches Sudoku boards from `https://sugoku.onrender.com/board`.
  - Wraps downloaded boards into internal `Grid` objects.

---

## Project Structure

- `Core/`
  - `Menu.cs` – Main menu loop and user interaction flow.
  - `Game.cs` – Brute-force solver and visualization.

- `Models/`
  - `Grid.cs` – Represents the Sudoku board, transactions, and helper logic.
  - `Note.cs` – Represents a single cell, with value and type (static, temp, etc.).
  - `Move.cs` – Records a value placed at a given position for backtracking.
  - `Position.cs` – Simple X/Y coordinate type.
  - `GridCollection.cs` – Static collection of all known grids.

- `Helpers/`
  - `ConsoleHelper.cs` – All console UI helpers (menus, prompts, grid selection).
  - `ApiHelper.cs` – API calls and default grid creation.

---

## How It Works (Detailed)

### 1. Data model

- **Board representation (`Grid`)**
  - The Sudoku board is stored as a 9×9 `List<List<Note>>`.
  - Each `Note` holds:
    - `value` – the digit in the cell (0 means empty).
    - `type` – one of:
      - `static` – original clue from the puzzle.
      - `null` – empty cell.
      - `temp` – temporary value tried during solving.
      - `inserted` – value that has been accepted/committed.
  - `Grid.Transactions` is a `Stack<List<Move>>`:
    - Each `Move` records a number placed at a `Position` (x, y).
    - Every recursion step in the solver pushes a new transaction list.

- **Grid collection (`GridCollection`)**
  - A static `Dictionary<string, Grid>` that stores all known boards.
  - Used for:
    - Default grids created at startup.
    - User-generated and saved grids.

### 2. Startup flow

- The `Menu` constructor calls `ApiHelper.GetDefaultGrid()`:
  - Builds three `Grid` instances from constant JSON boards (Easy, Medium, Hard).
  - Registers them in `GridCollection` under names like `"Default 1"`, `"Default 2"`, etc.
- After loading defaults, `Menu` immediately shows the main menu using `ConsoleHelper.SelectMenuOption()`.

### 3. Menu and user interaction

- The main menu is rendered by `ConsoleHelper.SelectItem`:
  - Clears the console.
  - Optionally renders any previous content (like a grid preview).
  - Shows a bullet list of options.
  - Lets you move with **Up/Down arrows** and confirm with **Enter**.
- Options include:
  - **Start Game** – pick an existing grid and run the solver on a clone.
  - **Generate Game** – fetch a new grid from the API and optionally save it.
  - **Exit** – terminate the application.

### 4. Starting a game

1. `Menu.startGame()` is called.
2. `ConsoleHelper.SelectGrid()`:
   - Reads all keys from `GridCollection.Grids`.
   - Builds a list like `"Name - Difficulty"` for each grid.
   - Lets you choose one with the same arrow/Enter interaction.
3. If you select a grid:
   - `Grid.Clone(...)` is used to create a **fresh playable copy**:
     - Static clues are copied as numbers.
     - Non-static cells are reset to `0` (empty).
   - A new `Game` instance is created with this cloned grid.

### 5. The solving algorithm (`Game`)

The solver is a **depth-first search with backtracking**:

1. `Game` constructor:
   - Displays the initial grid (`Grid.Display()`).
   - Waits for a key press to let you see the starting puzzle.
   - Calls `Solve(grid, sleepMilliseconds)` where `sleepMilliseconds` controls animation speed.

2. `Solve(Grid grid, int sleep)` works as follows:
   - Calls `grid.GetUnusedNotes()` to find all empty positions.
     - If there are **no empty positions**, the puzzle is solved → return `true`.
   - Takes the **first empty position** as the next target cell.
   - Pushes a new empty `List<Move>` onto `grid.Transactions` (start of a transaction).
   - Tries numbers from **1 to 9**:
     - For each number:
       - Computes `sums = grid.GetSumsForNote(position, value)`:
         - Row sum, column sum, and 3×3 region sum **including** this tentative value.
       - Computes `usedNotes = grid.GetUsedNotes(position)`:
         - All numbers that already appear in that row, column, and region.
       - If:
         - `value` is **not** in `usedNotes`, and
         - no sum exceeds `Grid.MaxSum` (45),
         - then the value is considered **valid**.
       - The value is applied via `grid.Add(value, position)`:
         - Updates the `Note` at that cell (may mark it as `temp`).
         - Records a `Move` in the current transaction.
       - The grid is drawn in the console, and the solver sleeps briefly.
       - `Solve` is called **recursively**:
         - If the recursive call returns `true`, the solution is found and the success bubbles up.
         - If it returns `false`, we **backtrack**:
           - Call `grid.RevertTransaction()` to undo all moves in the current transaction.
   - After trying all 1–9 values:
     - If none lead to a complete solution, pop the transaction from the stack (if any) and return `false`.

3. When the top-level call to `Solve` returns `true`:
   - `grid.CommitAllTransactions()` is called:
     - Iterates through all remaining transactions and marks their cells as `inserted`.
   - The final solved board is displayed again.

### 6. Generating and saving new games

1. `Menu.generateGame()` is called from the main menu.
2. Flow:
   - Ask for a grid name.
   - Ask for difficulty via `ConsoleHelper.SelectDifficulty()` (Easy/Medium/Hard).
   - Show a “Fetching grid…” message.
   - Call `ApiHelper.GetRandomGrid(difficulty)`:
     - Sends an HTTP GET to the API.
     - Parses the JSON `board` into a `Grid`.
   - If fetching or parsing fails, show an error and return to the menu.
   - If it succeeds:
     - Assign the entered name and capitalized difficulty to the grid.
     - Use `ConsoleHelper.SelectYesNo` with a preview action to show the grid and ask:
       - “Do you want to save this grid?”
     - If **Yes**:
       - Call `GridCollection.AddGrid(grid)`.
       - Show a success message.
     - If **No**:
       - Show a “not saved” message.
   - Finally, wait for a key press and return to the main menu.

---

## Running the Project

1. Open the solution in your preferred C# environment (Visual Studio, Rider, or VS Code with C# tooling).
2. Build the project.
3. Run the console application.
4. Use:
   - **Arrow keys + Enter** to navigate menus.
   - **Y / N** for inline yes/no prompts.

---

## Possible Extensions

- Add a UI (WinUI, WPF, web) on top of the existing solver.
- Allow manual entry of custom Sudoku puzzles via the console or from files.
- Track solve time and statistics per grid.
- Add checks to ensure generated/loaded puzzles have a unique solution.
