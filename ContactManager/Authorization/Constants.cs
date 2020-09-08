namespace ContactManager.Authorization
{
    public class Constants
    {
        public static readonly string CreateOperationName = "create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string ContactAdministratorsRole = "ContactAdministrators";
        public static readonly string ContactManagersRole = "ContactManagers";
        public static readonly string ContactUsersRole = "ContactUsers";
    }
}