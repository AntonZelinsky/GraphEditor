namespace GraphEditor.Helper
{
    public static class HashCode
    {
        public static int GetHashCode(int fromId, int toId)
        {
            var salt = 100;
            unchecked
            {
                return fromId*toId + salt;
            }
        }
    }
}