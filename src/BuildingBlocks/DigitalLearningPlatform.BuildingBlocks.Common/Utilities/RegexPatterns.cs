using System.Text.RegularExpressions;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Utilities
{
    public static class RegexPatterns
    {
        public static readonly Regex Email =  new(
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$",
            RegexOptions.Compiled);

        public static readonly Regex Url = new(
            @"^(http|https)://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(?:/[\w\-\./?%&=]*)?$",
            RegexOptions.Compiled);
        
        public static readonly Regex StrongPassword = new(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            RegexOptions.Compiled);
        
        public static readonly Regex UserName = new(
            @"^[a-zA-Z0-9_]{3,20}$",
            RegexOptions.Compiled);
        public static readonly Regex Slug = new(
            @"^[a-z0-9\-]+$",
            RegexOptions.Compiled);

    }
}
