using System.Collections.Generic;

namespace Codebound.Core
{
    public sealed class GuardState
    {
        public GuardState(string name) => Name = name;

        public string Name { get; }
        public int ObjectId { get; internal set; }
        public bool IsNull { get; internal set; }
        public int? Value { get; internal set; }
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
        readonly List<string> _items = new List<string>();

        public IReadOnlyList<string> Items => _items;
        public int ItemCount => _items.Count;

        internal void Add(string item) => _items.Add(item);
        internal void Remove(string item) => _items.Remove(item);
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

        string TypeName(int objectId)
        {
            var index = ChestIndex(objectId);
            return index >= 0 ? _chests[index].TypeName : "Chest";
        }

        string Noun(int objectId) => TypeName(objectId).ToLowerInvariant();

        public IReadOnlyList<string> WatchLines()
        {
            var lines = new List<string>();
            foreach (var guard in _guards)
                lines.Add(guard.Value.HasValue ? $"{guard.Name} = {guard.Value.Value}"
                    : guard.IsNull ? $"{guard.Name} -> null"
                    : guard.ObjectId != TraceEvent.NoObject ? $"{guard.Name} -> {Noun(guard.ObjectId)} #{guard.ObjectId}"
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
                    Caption = $"{e.Variable} now points to {Noun(e.ObjectId)} #{e.ObjectId}.";
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
                    if (index >= 0) ApplyItemChange(_chests[index], e.Text);
                    Caption = $"{TypeName(e.ObjectId)} #{e.ObjectId}: {e.Text}.";
                    break;
                case TraceEventType.ValueSet:
                    var holder = FindGuard(e.Variable);
                    if (holder != null) holder.Value = e.Value;
                    Caption = $"{e.Variable} now holds {e.Value}.";
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
        static void ApplyItemChange(ChestState chest, string text)
        {
            const string added = " added";
            const string removed = " removed";
            if (text.EndsWith(added)) chest.Add(text.Substring(0, text.Length - added.Length));
            else if (text.EndsWith(removed)) chest.Remove(text.Substring(0, text.Length - removed.Length));
        }

        GuardState FindGuard(string name)
        {
            foreach (var guard in _guards)
                if (guard.Name == name) return guard;
            return null;
        }
    }
}
