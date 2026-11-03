namespace Codebound.Core
{
    // Rooms written in code for now; later they move to data files.
    public static class RoomLibrary
    {
        public static bool Has(string roomId) => TryCreate(roomId, out _);

        public static bool TryCreate(string roomId, out RoomDefinition room)
        {
            switch (roomId)
            {
                case Room11.Id: room = Room11.Create(); break;
                case Room12.Id: room = Room12.Create(); break;
                case Room13.Id: room = Room13.Create(); break;
                case Room14.Id: room = Room14.Create(); break;
                case Room15.Id: room = Room15.Create(); break;
                case Room16.Id: room = Room16.Create(); break;
                default: room = null; break;
            }
            return room != null;
        }
    }
}
