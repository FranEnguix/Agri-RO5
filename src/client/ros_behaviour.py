import sys
from spade.behaviour import CyclicBehaviour
from spade.message import Message
from ros.factory import CommandConstructor
from ros.message import Message as RobotMessage

class RosBehaviourMessageManager(CyclicBehaviour):
    async def on_start(self):
        print(f"{self.agent.name}: init RosMessageManager behaviour.")

    async def on_end(self):
        print(f"{self.agent.name}: finished RosMessageManager behaviour.")
        await self.agent.stop()

    async def run(self):
        message = await self.receive(sys.float_info.max)
        if message:
            is_coordinator = await self.is_coordinator(message)
            self.agent.robot_message_queue.put(RobotMessage(message, is_coordinator)) 

    async def is_coordinator(self, message: Message):
        from_coordinator = message.get_metadata("ros", "coordinator")
        return from_coordinator is not None or bool(from_coordinator)


class RosBehaviourDispatcher(CyclicBehaviour):
    async def on_start(self):
        self.command_constructor = CommandConstructor()
        print(f"{self.agent.name}: init RosDispatcher behaviour.")

    async def on_end(self):
        print(f"{self.agent.name}: finished RosDispatcher behaviour.")
        await self.agent.stop()

    async def run(self):
        message: RobotMessage = self.agent.robot_message_queue.get()
        if message:
            commands = self.command_constructor.create_command(message)
            for command in commands:
                self.agent.action_queue.put(command)
        
