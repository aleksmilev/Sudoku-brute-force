import time
import json
from colorama import Fore, Style
from grid import Grid
from position import Position

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