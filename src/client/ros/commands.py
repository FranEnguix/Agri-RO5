from __future__ import annotations
from abc import ABC, abstractmethod


class Command(ABC):
    """
    The Command interface declares a method for executing a command.
    """
    def __init__(self, from_coordinator: bool) -> None:
        super().__init__()
        self.from_coordinator = from_coordinator

    @abstractmethod
    async def execute(self) -> None:
        pass

class Move(Command):
    def __init__(self, position: dict):
        super().__init__()
        self.position = position

    async def execute(self) -> None:
        print(f"moving to {self.position}")

class WaterValve(Command):
    def __init__(self, open: bool, index: int):
        super().__init__()
        self.open = open
        self.index = index

    async def execute(self) -> None:
        print(f"water valve #{self.index} set to open = {self.open}")
