namespace Codebound.Core
{
    public enum TraceEventType
    {
        LineExecuted,
        VariableDeclared,
        ObjectCreated,
        ReferenceAssigned,
        ObjectMutated,
        NullAssigned,
        Output,
        ExceptionThrown
    }

    public sealed class TraceEvent
    {
        public const int NoObject = 0;

        TraceEvent(TraceEventType type, int line, string variable, int objectId, string text)
        {
            Type = type;
            Line = line;
            Variable = variable ?? string.Empty;
            ObjectId = objectId;
            Text = text ?? string.Empty;
        }

        public TraceEventType Type { get; }
        public int Line { get; }
        public string Variable { get; }
        public int ObjectId { get; }
        public string Text { get; }

        public static TraceEvent LineExecuted(int line) =>
            new TraceEvent(TraceEventType.LineExecuted, line, null, NoObject, null);

        public static TraceEvent VariableDeclared(int line, string variable) =>
            new TraceEvent(TraceEventType.VariableDeclared, line, variable, NoObject, null);

        public static TraceEvent ObjectCreated(int line, int objectId, string typeName) =>
            new TraceEvent(TraceEventType.ObjectCreated, line, null, objectId, typeName);

        public static TraceEvent ReferenceAssigned(int line, string variable, int objectId) =>
            new TraceEvent(TraceEventType.ReferenceAssigned, line, variable, objectId, null);

        public static TraceEvent ObjectMutated(int line, int objectId, string description) =>
            new TraceEvent(TraceEventType.ObjectMutated, line, null, objectId, description);

        public static TraceEvent NullAssigned(int line, string variable) =>
            new TraceEvent(TraceEventType.NullAssigned, line, variable, NoObject, null);

        public static TraceEvent Output(int line, string text) =>
            new TraceEvent(TraceEventType.Output, line, null, NoObject, text);

        public static TraceEvent ExceptionThrown(int line, string exceptionName) =>
            new TraceEvent(TraceEventType.ExceptionThrown, line, null, NoObject, exceptionName);
    }
}
