namespace Codebound.Core
{
    // Rooms written in code for now; later they move to data files.
    public static class RoomLibrary
    {
        public static bool Has(string roomId) => roomId == Room11.Id;

        public static bool TryCreate(string roomId, out RoomDefinition room)
        {
            room = roomId == Room11.Id ? Room11.Create() : null;
            return room != null;
        }
    }
}
