using System.Collections.Generic;

namespace BBMS_WebAPI.Utilities
{
    public static class BloodGroupMapper
    {
        private static readonly Dictionary<string, int> BloodGroupMap = new Dictionary<string, int>
        {
            { "A+Ve", 9 },
            { "A-Ve", 10 },
            { "B+Ve", 11 },
            { "B-Ve", 12 },
            { "AB+Ve", 13 },
            { "AB-Ve", 14 },
            { "O+Ve", 15 },
            { "O-Ve", 16 },
            { "Oh+Ve", 17 },
            { "Oh-Ve", 18 }
        };

        public static int? GetBloodGroupID(string bloodGroupName)
        {
            if (BloodGroupMap.TryGetValue(bloodGroupName, out int id))
            {
                return id;
            }
            return null; // Invalid BloodGroupName
        }
    }
}
