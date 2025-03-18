import time
import json
import sys
from colorama import Fore, Style

class Game:
    def __init__(self):
        try:
            self.grid = self.load_grid()

            self.grid.display()
            time.sleep(1)

            print("\nPress any key to start solving...")
            input()

            self.grid.display()
            time.sleep(1)

            if self.solve(self.grid):
                print(Fore.GREEN + "\nSudoku Solved Successfully!" + Style.RESET_ALL)
            else:
                print(Fore.RED + "\nFailed to solve Sudoku!" + Style.RESET_ALL)

            self.grid.display(True, True)

        except Exception as e:
            print(Fore.RED + f"\nAn error occurred: {e}" + Style.RESET_ALL)

    def load_grid(self):
        json_grid = """
        [
            [ 5, 6, 1, 0, 4, 0, 7, 0, 0 ],
            [ 0, 9, 0, 0, 2, 0, 1, 0, 8 ],
            [ 0, 0, 0, 0, 0, 0, 0, 6, 0 ],
            [ 8, 0, 0, 2, 0, 6, 0, 0, 0 ],
            [ 6, 0, 0, 1, 0, 9, 2, 0, 4 ],
            [ 9, 0, 2, 7, 0, 4, 5, 0, 6 ],
            [ 1, 2, 0, 0, 0, 0, 8, 0, 7 ],
            [ 0, 5, 6, 8, 7, 2, 0, 0, 9 ],
            [ 4, 0, 0, 5, 0, 1, 6, 2, 0 ]
        ]"""

        return Grid(json.loads(json_grid))

    def solve(self, grid):
        empty_pos = grid.get_unused_notes()
        if not empty_pos:
            return True

        row, col = empty_pos

        for num in range(1, 10):
            if grid.is_valid_move(row, col, num):
                grid.add(num, Position(col + 1, row + 1))
                grid.display()
                time.sleep(0.2)

                if self.solve(grid):
                    return True

                grid.revert_transaction()

        return False


class Grid:
    MaxSum = 45

    def __init__(self, grid_data):
        self.grid = [[Note(value) for value in row] for row in grid_data]
        self.transactions = []


    def display(self, clear_before=True, solved=False):
        if clear_before:
            sys.stdout.write("\033c")

        for i, row in enumerate(self.grid):
            if i > 0 and i % 3 == 0:
                print("─────────╂──────────╂─────────")

            for j, note in enumerate(row):
                if j > 0 and j % 3 == 0:
                    print("│ ", end="")

                if solved and note.type is "temp":
                    color = Fore.BLUE
                else:
                    color = note.get_color()

                char = str(note.value) if note.value != 0 else "●"
                print(f"{color}{char} {Style.RESET_ALL}", end=" ")

            print()

    def get_unused_notes(self):
        for row in range(9):
            for col in range(9):
                if self.grid[row][col].value == 0:
                    return row, col
        return None

    def is_valid_move(self, row, col, num):
        for i in range(9):
            if self.grid[row][i].value == num or self.grid[i][col].value == num:
                return False

        box_start_row = (row // 3) * 3
        box_start_col = (col // 3) * 3
        for i in range(3):
            for j in range(3):
                if self.grid[box_start_row + i][box_start_col + j].value == num:
                    return False

        return True

    def add(self, value, position):
        row, col = position.Y, position.X
        if not (1 <= value <= 9) or self.grid[row][col].value != 0:
            return

        self.transactions.append((row, col, self.grid[row][col].value))
        self.grid[row][col].change_value(value)

    def revert_transaction(self):
        if not self.transactions:
            return

        row, col, prev_value = self.transactions.pop()
        self.grid[row][col].reset()


class Position:
    def __init__(self, X, Y):
        self.X = X - 1
        self.Y = Y - 1


class Note:
    TYPES_TO_COLORS = {
        "static": Fore.WHITE,
        "null": Fore.GREEN,
        "temp": Fore.RED,
    }

    def __init__(self, value):
        self.value = value
        self.type = "null" if value == 0 else "static"

    def get_color(self):
        return Note.TYPES_TO_COLORS.get(self.type, Fore.RESET)

    def change_value(self, value):
        if self.value != 0 or value == 0:
            return
        self.value = value
        self.type = "temp"

    def reset(self):
        self.value = 0
        self.type = "null"


if __name__ == "__main__":
    Game()
