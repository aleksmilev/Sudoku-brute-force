from colorama import Fore

class Note:
    TYPES_TO_COLORS = {
        "static": Fore.WHITE,
        "null": Fore.GREEN,
        "temp": Fore.RED,
        "inserted": Fore.BLUE
    }

    def __init__(self, value):
        self.value = value
        self.type = "null" if value == 0 else "static"

    def get_color(self):
        return Note.TYPES_TO_COLORS.get(self.type, Fore.RESET)

    def set_as_inserted(self):
        self.type = "inserted"

    def change_value(self, value, sums, used_notes):
        if self.value != 0 or value == 0 or value in used_notes:
            return

        self.value = value
        self.type = "temp" if any(sum_ < 45 for sum_ in sums) else "null"

        if self.type == "null":
            self.value = 0

    def reset(self):
        self.type = "null"
        self.value = 0
