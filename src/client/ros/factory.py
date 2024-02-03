from ros.commands import Move, WaterValve, Command

from ros.message import Message

import random

class CommandConstructor():

    def __init__(self) -> None:
        pass

    async def create_command(self, message: Message) -> list[Command]:
        if message.from_coordinator:
            return [Move(position={"x": -5, "y": -1}), WaterValve(open=True, index=0)]
        else:
            return [Move(position={"x": random.randint(1, 10), "y": random.randint(1, 10)}), WaterValve(open=False, index=0)]