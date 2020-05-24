namespace DBSysCore
{
    public enum StatusCode
    {
        Ok,
        Error,

        ConnectionError,
        ConnectionInvalidOperation,
        ConnectionSQLError,

        GrantsUnathorized,
        GrantsInproper,
        GrantsVirtualAdminNotAllowed,

        LoginNoRegisteredUsers,
        LoginAlreadyAuthorized,
        LoginInvalidLogin,
        LoginInvalidPass,

        ProgramStateInvalid,

        DumpMergeNoFiles,

        AddStaffPersonExists,
        AddStaffInvalidPersonId,
    }
}
