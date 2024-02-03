from email.message import Message
import time
import os

from commander import Axis, ImageMode
from entity import EntityAgent
from spade.message import Message

from ros.commands import Command
from image_data import ImageData

class EntityShell:

    async def init(agent: EntityAgent):
        time.sleep(4)

    async def perception(agent: EntityAgent, data: ImageData):
        await agent.change_color(1, 0, 0, 0.8)
        if agent.name == "xfe_agente1":
            message = Message(to="fe_agente2@gtirouter.dsic.upv.es")
            message.body = "hola"
            message.set_metadata("ros", "message")
            message.set_metadata("ros", "coordinator")
            await agent.behaviours[0].send(message)

        time.sleep(4)

    async def cognition(agent: EntityAgent):
        await agent.change_color(0, 1, 0, 0.8)
        time.sleep(4)

    async def action(agent: EntityAgent, command: Command):
        await agent.change_color(0, 0, 1, 0.8)
        await command.execute()
        time.sleep(4)