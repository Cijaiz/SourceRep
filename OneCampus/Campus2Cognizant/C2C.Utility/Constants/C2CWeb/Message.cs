namespace C2C.Core.Constants.C2CWeb
{
    public class Message
    {
        public const string MSG_NOT_AUTHORIZED = "You are not authorized to perform this action";

        public const string CREATED = "Created successfully.";
        public const string UPDATED = "Updated successfully.";
        public const string VOTED = "Poll Voted successfully.";
        public const string ALREADY_VOTED = "You have already Voted.";
        public const string UPDATED_COUNT = "Updated successfully. No of record update :{0}";
        public const string DELETED = "Deleted successfully.";
        public const string PUBLISHED = "Published successfully.";
        public const string UNPUBLISHED = "Unpublished successfully.";

        public const string FAILED = "Failed to perform the operation.";
        public const string ERROR = "Error occurred while performing the operation.";
        public const string RECORED_NOT_FOUND = "Record not found.";
        public const string VALIDATION_FAILED = "Validation failed.";

        #region Membership Module
        public const string MEMBERSHIP_VALID_USER = "Valid user.";
        public const string MEMBERSHIP_INVALID_CREDENTIALS = "Invalid credentials.";
        public const string MEMBERSHIP_ACCOUNT_NOT_ACTIVE = "Your account is not activated.";
        public const string MEMBERSHIP_ACCOUNT_LOCKED = "You account has been locked.";
        public const string MEMBERSHIP_CHANGE_PASSWORD_SUCCESS = "Password changed successfully.";
        #endregion

        #region Role & Permission Module
        public const string ROLE_EXSISTS = "Role with the given name already Exists.";
        public const string ROLE_USER_EXSISTS = "User with the role exsists.Delete User to delete role.";
        public const string ROLE_ADDUSERS_FAILED = "Adding Users failed for a the specified role.";
        public const string ROLE_DELETEROLE_FAILED = "Deleting a role failed";
        public const string ROLE_DELETEUSER_FAILED = "Deleting a user from role failed";
        #endregion

        #region User & User Profile
        public const string USER_EXSISTS = "User already exists.";
        #endregion

        #region WelcomeNote

        public const string WELCOMENOTE = "WelcomeNote Module";
        public const string WELCOMENOTE_NULL = "WelcomeNote cannot be null";
        public const string WELCOMENOTE_STARTDATE_NULL = "WelcomeNote Startdate cannot be null";
        public const string WELCOMENOTE_ENDDATE_NULL = "WelcomeNote enddate cannot be null";
        public const string WELCOMENOTE_DATE_VALIDATION_FAILED = "StartDate cannot be greater than End date";
        public const string WELCOMENOTE_STARTDATE_VALIDATION_FAILED = "StartDate cannot be lesser than current date";
        #endregion

        #region Group
        public const string GROUP_EXIST = "Group already exist.";
        public const string GROUP_NOT_FOUNT_FOR_SEARCH = "No group found for search text entered.";
        public const string TYPE_TEXT_TO_SEARCH = "Type any text to search.";
        public const string NO_MEMBER_FOUND = "There is no member for the search text";
        public const string SELECT_USER_TO_PROCEED = "Select at least one user to proceed.";
        public const string MEMBERS_STATUS_UPDATED = "Members status updated successfully.";
        public const string MEMBER_STATUS_CHANGED = "Member status updated.";
        public const string MEMBER_ADDED = "Member added.";
        public const string MEMBERS_ADDED = "Members added successfully.";
        public const string GROUPSELECTION = "Please select atleast one Group";
        #endregion

        #region Chire File Upload
        public const string FILE_UPLOAD_SUCCESS = "File has been successfully Uploaded.";
        public const string FILE_UPLOAD_FAILURE = "Error in uploading the file.";
        public const string FILE_UPLOAD_FORMAT_ERROR = "column mismatches. Please provide correct column format.";
        public const string FILE_UPLOAD_FILE_NULL = "Upload File.";
        public const string FILE_UPLOAD_CONTENT_NULL = "Upload file with required content.";
        public const string FILE_UPLOAD_EXTENSION_ERROR = "File Extension is invalid. It Should be in .csv Format.";

        #endregion
    }
}
