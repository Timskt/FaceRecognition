using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FaceRecognition.Common.Message;

public class MessageModel : ValueChangedMessage<MessageList>
{
    public MessageModel(MessageList value) : base(value)
    {
    }
}