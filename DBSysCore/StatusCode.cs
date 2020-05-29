namespace DBSysCore
{
    public enum StatusCode
    {
        Ok,
        Error,

        ConnectionError,
        ConnectionInvalidOperation,
        ConnectionSQLError,

        ConnectionDumpModelNotFound,
        ConnectionUsersModelNotFound,

        GrantsUnathorized,
        GrantsInproper,
        GrantsVirtualAdminNotAllowed,

        LoginNoRegisteredUsers,
        LoginAlreadyAuthorized,
        LoginInvalidLogin,
        LoginInvalidPass,

        ProgramStateInvalid,

        LoadStaticTestsStaticTableNotFound,

        BeginChallengeInvalidOTKStaffId,

        DumpUseInvalidFilename,

        DumpMergeNoFiles,

        AddStaffPersonExists,
        AddStaffInvalidPersonId,

        NoFPGAVersionFound,
    }
}
