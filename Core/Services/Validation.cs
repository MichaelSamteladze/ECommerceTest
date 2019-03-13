using Core.Properties;
using Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Services
{
    public class Validation
    {
        #region Methods
        public static SimpleKeyValue<string, string> GetError(string ErrorKey, string ErrorMessage)
        {
            return new SimpleKeyValue<string, string> { Key = ErrorKey, Value = ErrorMessage };
        }

        public static SimpleKeyValue<string, string> Validate(Func<bool> ErrorAction, string ErrorKey, string ErrorMessage)
        {
            SimpleKeyValue<string, string> Error = null;
            if (ErrorAction())
            {
                Error = GetError(ErrorKey, ErrorMessage);
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidateEmail(string ErrorKey, string Email, bool ValidateRequired, bool ValidateUnique, int? UserID)
        {
            SimpleKeyValue<string, string> Error = null;
            if (string.IsNullOrWhiteSpace(Email))
            {
                if (ValidateRequired)
                {
                    Error = GetError(ErrorKey, Resources.ValidationRequiredField);
                }
            }
            else
            {
                if (!Regex.IsMatch(Email, Constants.RegularExpressions.Email))
                {
                    Error = GetError(ErrorKey, Resources.ValidationEmailNotValid);
                }
                else if (ValidateUnique && !UsersDataAccess.IsEmailUniq(UserID: UserID, Email: Email))
                {
                    Error = GetError(ErrorKey, Resources.ValidationEmailNotUnique);
                }
            }
            return Error;
        }

        public static SimpleKeyValue<string, string> ValidatePassword(string ErrorKey, string Password)
        {
            var Error = ValidateRequired(ErrorKey: ErrorKey, ValueToValidate: Password);

            if (Error == null)
            {
                //if (Password.Length < 8)
                //{
                //    Error = GetError(ErrorKey, Resources.ValidationPasswordLength);
                //}
                //else if (!(Password.Any(char.IsLetter)))
                //{

                //    Error = GetError(ErrorKey, Resources.ValidationPasswordStrength);
                //}
                //else if (!(Password.Any(char.IsDigit)))
                //{
                //    Error = GetError(ErrorKey, Resources.ValidationPasswordStrength);
                //}

                if (Password.Length < 8 || !(Password.Any(char.IsLetter)) || !(Password.Any(char.IsDigit)))
                {
                    Error = GetError(ErrorKey, $"{Resources.ValidationPasswordLength}. {Resources.ValidationPasswordStrength}");
                }
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidatePasswordRepeat(string ErrorKey, string Password, string PasswordRepeat)
        {
            SimpleKeyValue<string, string> Error = null;
            if (Password != PasswordRepeat)
            {
                Error = GetError(ErrorKey, Resources.ValidationPasswordsNotMatch);
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidateOldPassword(string ErrorKey, string UserPassword, string OldPassword)
        {
            var Error = ValidateRequired(ErrorKey, OldPassword);
            if (Error == null)
            {
                if (UserPassword != OldPassword.MD5())
                {
                    Error = GetError(ErrorKey, Resources.ValidationOldPasswordNotMatch);
                }
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidateRequired(string ErrorKey, object ValueToValidate)
        {
            SimpleKeyValue<string, string> Error = null;


            if (ValueToValidate == null)
            {
                Error = GetError(ErrorKey, Resources.ValidationRequiredField);
            }
            else if (ValueToValidate.GetType() == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(ValueToValidate as string))
                {
                    Error = GetError(ErrorKey, Resources.ValidationRequiredField);
                }
            }

            return Error;
        }
        #endregion
    }
}