namespace ApiDemo.Common
{
    static public class CommonFunctions
    {
        public static int getUniqueId(List<int> ids)
        {
            int expected = 1; // Initialize the expected value

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == expected)
                    expected++; // Increment expected if the current ID matches
                else
                    break; // Break the loop if a missing ID is found
            }

            return expected;
        }
    }
}
