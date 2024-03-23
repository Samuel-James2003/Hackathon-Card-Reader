namespace Card_Reader_Api.Model
{
    public class JsonPayloadBuilder
    {
        private string _model;
        private List<(string role, string content)> _messages;
        public JsonPayloadBuilder(string model)
        {
            _model = model;
            _messages = new List<(string role, string content)>();
        }
        public void AddMessage(string role, string content)
        {
            _messages.Add((role, content));
        }
        public string BuildPayload()
        {
            var messagesJson = string.Join(",", _messages.Select(m =>
                $@"{{
            ""role"": ""{m.role}"",
            ""content"": ""{m.content.Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "")}""
        }}"
            ));
            return $@"{{
        ""model"": ""{_model}"",
        ""messages"": [{messagesJson}]
    }}";
        }


    }
}
