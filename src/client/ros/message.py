from spade.message import Message as SpadeMessage

class Message(SpadeMessage):
    def __init__(self, message: SpadeMessage, from_coordinator) -> None:
        super().__init__(
            to=message.to,
            sender=message.sender,
            body=message.body,
            thread=message.thread,
            metadata=message.metadata
        )
        self.from_coordinator = from_coordinator