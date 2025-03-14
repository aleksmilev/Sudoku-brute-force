import json
import sys
from move import Move
from note import Note
from position import Position

class Grid:
    MAX_SUM = 45

    def __init__(self, grid_json):
        self.grid = self.convert_to_note_grid(json.loads(grid_json))
        self.transactions = [[]]

    @staticmethod
    def convert_to_note_grid(grid):
        return [[Note(value) for value in row] for row in grid]

    def display(self, clear_before=True):
        if clear_before:
            sys.stdout.write("\033c")

        for i, row in enumerate(self.grid):
            if i > 0 and i % 3 == 0:
                print("──────╂───────╂──────")

            for j, note in enumerate(row):
                if j > 0 and j % 3 == 0:
                    print("│ ", end="")

                color = note.get_color()
                char = str(note.value) if note.value != 0 else "●"
                print(color + char + " ", end="")

            print()

    def add(self, value, position):
        if not (1 <= value <= 9) or self.grid[position.y][position.x].value != 0:
            return

        sums = self.get_sums_for_note(position, value)
        used_notes = self.get_used_notes(position)

        self.grid[position.y][position.x].change_value(value, sums, used_notes)
        self.transactions[-1].append(Move(value, position))

        if any(sum_ > Grid.MAX_SUM for sum_ in sums) or value in used_notes:
            self.revert_transaction()

        if sum(sums) == Grid.MAX_SUM * 3:
            self.commit_transaction()

    def revert_transaction(self):
        for move in self.transactions[-1]:
            self.grid[move.position.y][move.position.x].reset()

        self.transactions.pop()
        if not self.transactions:
            self.transactions.append([])

    def commit_transaction(self):
        for move in self.transactions[-1]:
            self.grid[move.position.y][move.position.x].set_as_inserted()

        self.transactions.append([])

    def get_sums_for_note(self, position, value):
        row_sum = sum(self.grid[position.y][i].value for i in range(9)) + value
        col_sum = sum(self.grid[i][position.x].value for i in range(9)) + value
        region_start_row, region_start_col = (position.y // 3) * 3, (position.x // 3) * 3
        region_sum = sum(
            self.grid[i][j].value
            for i in range(region_start_row, region_start_row + 3)
            for j in range(region_start_col, region_start_col + 3)
        ) + value
        return row_sum, col_sum, region_sum

    def get_used_notes(self, position):
        used_notes = {self.grid[position.y][i].value for i in range(9)}
        used_notes |= {self.grid[i][position.x].value for i in range(9)}

        region_start_row, region_start_col = (position.y // 3) * 3, (position.x // 3) * 3
        used_notes |= {
            self.grid[i][j].value
            for i in range(region_start_row, region_start_row + 3)
            for j in range(region_start_col, region_start_col + 3)
        }

        return used_notes - {0}

    def get_unused_positions(self, region_row=0, region_col=0):
        return [
            Position(j + 1, i + 1)
            for i in range(region_row * 3, (region_row + 1) * 3)
            for j in range(region_col * 3, (region_col + 1) * 3)
            if self.grid[i][j].value == 0
        ]
