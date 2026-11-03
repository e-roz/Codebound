namespace Codebound.Core
{
    // Rooms written in code for now; later they move to data files.
    public static class RoomLibrary
    {
        public static bool Has(string roomId) => roomId == Room11.Id || roomId == Room12.Id || roomId == Room13.Id;

        public static bool TryCreate(string roomId, out RoomDefinition room)
        {
            room = roomId == Room11.Id ? Room11.Create() : roomId == Room12.Id ? Room12.Create() : roomId == Room13.Id ? Room13.Create() : null;
            return room != null;
        }
    }
}
