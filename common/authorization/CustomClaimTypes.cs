namespace SS.Common.authorization
{
    public static class CustomClaimTypes
    {
        public const string FullName = "name";
        public const string IdirId = "idir_userid";
        public const string IdirUserName = "preferred_username";
        public const string UserId = nameof(CustomClaimTypes) + nameof(UserId);
        public const string Permission = nameof(CustomClaimTypes)+ nameof(Permission);
        public const string HomeLocationId = nameof(CustomClaimTypes) + nameof(HomeLocationId);
    }
}
