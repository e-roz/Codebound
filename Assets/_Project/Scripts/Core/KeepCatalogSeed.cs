using System.Collections.Generic;

namespace Codebound.Core
{
    // Starting content for the Keep catalog asset (see LESSON_PLAN.md). The asset is the live data; edit it there.
    public static class KeepCatalogSeed
    {
        public static List<KeepChapter> Create()
        {
            return new List<KeepChapter>
            {
                Chapter("vaults", "The Vaults",
                    Room("1-1", "The Twin Guards", "Predict", "Reference types"),
                    Room("1-2", "Copy That", "Trace", "Value types vs references"),
                    Room("1-3", "The Shared Inventory", "Bug hunt", "Sharing a List by mistake"),
                    Room("1-4", "Copy of a Copy", "Repair", "Shallow vs deep copy"),
                    Room("1-5", "The Empty Chest", "Bug hunt", "NullReferenceException"),
                    Room("1-6", "Who Changed My Guard?", "Interview", "Mutation vs reassignment")),
                Chapter("gates", "The Gates",
                    Room("2-1", "The Gate Rules", "Predict", "&& and || precedence"),
                    Room("2-2", "The Last Guard", "Bug hunt", "Off-by-one errors"),
                    Room("2-3", "The Gate That Never Closes", "Trace", "Loops that never end"),
                    Room("2-4", "The Toll Boundary", "Repair", "> vs >= at the boundary"),
                    Room("2-5", "The Split Treasure", "Bug hunt", "Integer division and zero"),
                    Room("2-6", "The Visitor Form", "Code review", "Parse vs TryParse")),
                Chapter("workshop", "The Workshop",
                    Room("3-1", "The Healer Machine", "Predict", "Return values"),
                    Room("3-2", "The Greedy Counter", "Bug hunt", "Side effects"),
                    Room("3-3", "The Stack of Clues", "Trace", "Reading a stack trace"),
                    Room("3-4", "The Silent Failure", "Bug hunt", "Swallowed exceptions"),
                    Room("3-5", "Red, Then Green", "Repair", "Test first"),
                    Room("3-6", "The Forty-Line Monster", "Code review", "One method, one job")),
                Chapter("archive", "The Archive",
                    Room("4-1", "The Duplicate Scrolls", "Predict", "List vs HashSet"),
                    Room("4-2", "The Shelf That Shrinks", "Bug hunt", "Changing a list while looping"),
                    Room("4-3", "The Missing Key", "Trace", "Dictionary lookups"),
                    Room("4-4", "The Case-Sensitive Index", "Bug hunt", "Case-sensitive keys"),
                    Room("4-5", "Searching the Whole Shelf", "Repair", "Search cost: List vs HashSet"),
                    Room("4-6", "The Right Container", "Code review", "Choosing a collection")),
                Sealed("great-hall", "The Great Hall"),
                Sealed("towers", "The Towers"),
                Sealed("architect", "Architect's Chamber")
            };
        }

        static KeepChapter Chapter(string id, string name, params KeepRoom[] rooms) =>
            new KeepChapter { id = id, name = name, rooms = new List<KeepRoom>(rooms) };

        static KeepChapter Sealed(string id, string name) =>
            new KeepChapter { id = id, name = name, isSealed = true };

        static KeepRoom Room(string id, string title, string roomType, string concept) =>
            new KeepRoom { id = id, title = title, roomType = roomType, concept = concept };
    }
}
