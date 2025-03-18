import sys
from colorama import Fore, Style
from note import Note

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