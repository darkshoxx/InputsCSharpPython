import keyboard


def tab() -> None:
    keyboard.press_and_release("tab")


def type_text(my_string: str) -> None:
    keyboard.write(my_string)


if __name__ == "__main__":
    type_text("This is my typing test")