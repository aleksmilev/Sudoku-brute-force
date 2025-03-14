import time
from collections import deque
from grid import Grid
from colorama import Fore, Style

class Game:
    def __init__(self):
        try:
            print(Fore.YELLOW + "Initializing Sudoku Solver...\n" + Style.RESET_ALL)
            self.grid = self.load_grid()
            self.grid.display()
            time.sleep(1)
            solved = self.solve(self.grid, 0.1)

            if solved:
                print(Fore.GREEN + "\nSolved Successfully!" + Style.RESET_ALL)
            else:
                print(Fore.RED + "\nFailed to solve Sudoku!" + Style.RESET_ALL)

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
        ]
        """
        return Grid(json_grid)

    def solve(self, grid, sleep=0):
        skipped_regions = deque()

        for region_row in range(3):
            for region_col in range(3):
                if not self.solve_region(grid, region_row, region_col, sleep):
                    skipped_regions.append((region_row, region_col))

        return not skipped_regions

    def solve_region(self, grid, region_row, region_col, sleep):
        unused_positions = grid.get_unused_positions(region_row, region_col)

        for pos in unused_positions:
            for num in range(1, 10):
                if num not in grid.get_used_notes(pos):
                    grid.add(num, pos)
                    grid.display()
                    time.sleep(sleep)
                    return True

        return False
