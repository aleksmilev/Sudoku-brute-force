from colorama import Fore


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
