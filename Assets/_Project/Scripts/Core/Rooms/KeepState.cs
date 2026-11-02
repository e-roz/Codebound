using System.Collections.Generic;

namespace Codebound.Core
{
    public sealed class GuardState
    {
        public GuardState(string name) => Name = name;

        public string Name { get; }
        public int ObjectId { get; internal set; }
        public bool IsNull { get; internal set; }
    }

    public sealed class ChestState
    {
        public ChestState(int id, string typeName)
        {
            Id = id;
            TypeName = typeName;
        }

        public int Id { get; }
        public string TypeName { get; }
        public int ItemCount { get; internal set; }
    }

    // What the Keep looks like after the trace events played so far. The view only draws this.
    public sealed class KeepState
    {
        readonly List<GuardState> _guards = new List<GuardState>();
        readonly List<ChestState> _chests = new List<ChestState>();

        public IReadOnlyList<GuardState> Guards => _guards;
        public IReadOnlyList<ChestState> Chests => _chests;
        public int CurrentLine { get; private set; }
        public string Output { get; private set; } = string.Empty;
        public string Error { get; private set; } = string.Empty;
        public string Caption { get; private set; } = string.Empty;

        public int ChestIndex(int objectId)
        {
            for (var i = 0; i < _chests.Count; i++)
                if (_chests[i].Id == objectId) return i;
            return -1;
        }

        public IReadOnlyList<string> WatchLines()
        {
            var lines = new List<string>();
            foreach (var guard in _guards)
                lines.Add(guard.IsNull ? $"{guard.Name} -> null"
                    : guard.ObjectId != TraceEvent.NoObject ? $"{guard.Name} -> chest #{guard.ObjectId}"
                    : $"{guard.Name} -> (nothing yet)");
            return lines;
        }

        public void Clear()
        {
            _guards.Clear();
            _chests.Clear();
            CurrentLine = 0;
            Output = string.Empty;
            Error = string.Empty;
            Caption = string.Empty;
        }

        public void Apply(TraceEvent e)
        {
            switch (e.Type)
            {
                case TraceEventType.LineExecuted:
                    CurrentLine = e.Line;
                    Caption = $"Line {e.Line} runs.";
                    break;
                case TraceEventType.VariableDeclared:
                    if (FindGuard(e.Variable) == null) _guards.Add(new GuardState(e.Variable));
                    Caption = $"{e.Variable} is a new guard. It holds nothing yet.";
                    break;
                case TraceEventType.ObjectCreated:
                    _chests.Add(new ChestState(e.ObjectId, e.Text));
                    Caption = $"A new {e.Text} appears (#{e.ObjectId}).";
                    break;
                case TraceEventType.ReferenceAssigned:
                    var target = FindGuard(e.Variable);
                    if (target != null)
                    {
                        target.ObjectId = e.ObjectId;
                        target.IsNull = false;
                    }
                    Caption = $"{e.Variable} now points to chest #{e.ObjectId}.";
                    break;
                case TraceEventType.NullAssigned:
                    var nulled = FindGuard(e.Variable);
                    if (nulled != null)
                    {
                        nulled.ObjectId = TraceEvent.NoObject;
                        nulled.IsNull = true;
                    }
                    Caption = $"{e.Variable} points to nothing (null).";
                    break;
                case TraceEventType.ObjectMutated:
                    var index = ChestIndex(e.ObjectId);
                    if (index >= 0) _chests[index].ItemCount += ItemChange(e.Text);
                    Caption = $"Chest #{e.ObjectId}: {e.Text}.";
                    break;
                case TraceEventType.Output:
                    Output = e.Text;
                    Caption = $"The program prints {e.Text}.";
                    break;
                case TraceEventType.ExceptionThrown:
                    Error = e.Text;
                    Caption = $"{e.Text}! The program stops.";
                    break;
            }
        }

        // ObjectMutated text is written as "<item> added" or "<item> removed".
        static int ItemChange(string text)
        {
            if (text.EndsWith("added")) return 1;
            if (text.EndsWith("removed")) return -1;
            return 0;
        }

        GuardState FindGuard(string name)
        {
            foreach (var guard in _guards)
                if (guard.Name == name) return guard;
            return null;
        }
    }
}
