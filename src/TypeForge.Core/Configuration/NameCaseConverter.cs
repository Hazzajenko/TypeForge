using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeForge.Core.Configuration;



/*
public class NameCaseConverter : JsonConverter<ExportModelType>
{
    public override NameCaseConverter Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
       reader.Read();
       typeToConvert.
       // reader.
           
    }

    public override void Write(Utf8JsonWriter writer, ExportModelType value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        NameCaseConverter value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id);
        writer.WriteBoolean("isGroup", value.IsGroup);
        writer.WriteString("lastMessageContent", value.LastMessageContent);
        writer.WriteString("lastMessageSenderId", value.LastMessageSenderId);
        writer.WriteString("lastMessageFrom", value.LastMessageFrom);
        writer.WriteString(
            "lastMessageSentTime",
            value.LastMessageSentTime.ToString(CultureInfo.CurrentCulture)
        );
        writer.WriteBoolean("isLastMessageReadByUser", value.IsLastMessageReadByUser);
        writer.WriteBoolean("isLastMessageUserSender", value.IsLastMessageUserSender);

        if (value is GroupChatNameCaseConverter groupChatModel)
        {
            writer.WriteString("groupChatName", groupChatModel.GroupChatName);
            writer.WriteString("groupChatPhotoUrl", groupChatModel.GroupChatPhotoUrl);
        }
        else if (value is UserNameCaseConverter userMessageModel)
        {
            writer.WriteString("otherUserId", userMessageModel.OtherUserId);
        }

        writer.WriteEndObject();
    }
}*/
