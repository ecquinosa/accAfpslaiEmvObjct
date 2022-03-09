
namespace accAfpslaiEmvObjct
{
    public class Utilities
    {      
        public static void ShowInformationMessage(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg, MiddleServerApi.msgHeader, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        public static void ShowWarningMessage(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg, MiddleServerApi.msgHeader, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }

        public static void ShowErrorMessage(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg, MiddleServerApi.msgHeader, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
        //public static bool IsPasswordValid(string userPass)
        //{
        //    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]*$");
        //    int userPassLength = 8;
        //    bool bln = false;

        //    if (userPass.Length < userPassLength) ShowWarningMessage(string.Format("Password should be {0} characters long", userPassLength));
        //    else if (!r.IsMatch(userPass)) ShowWarningMessage(string.Format("Password should be alphanumeric"));
        //    else {
        //        bool hasUpper = false; bool hasLower = false; bool hasDigit = false;
        //        for (int i = 0; i < userPass.Length && !(hasUpper && hasLower && hasDigit); i++)
        //        {
        //            char c = userPass[i];
        //            if (!hasUpper) hasUpper = char.IsUpper(c);
        //            if (!hasLower) hasLower = char.IsLower(c);
        //            if (!hasDigit) hasDigit = char.IsDigit(c);
        //        }

        //        if (hasUpper && hasLower && hasDigit) bln = true;
        //        else ShowWarningMessage(string.Format("Password should have numeric value, upper and lower case"));
        //    }

        //    return bln;
        //}

        public static string IsPasswordValidv2(string userPass, int requiredPasswordLength)
        {            
            if (userPass.Length < requiredPasswordLength) return string.Format("Password should be {0} characters long", requiredPasswordLength);         
            else
            {
                bool hasUpper = false; bool hasLower = false; bool hasDigit = false;
                for (int i = 0; i < userPass.Length && !(hasUpper && hasLower && hasDigit); i++)
                {
                    char c = userPass[i];
                    if (!hasUpper) hasUpper = char.IsUpper(c);
                    if (!hasLower) hasLower = char.IsLower(c);
                    if (!hasDigit) hasDigit = char.IsDigit(c);
                }

                if (hasUpper && hasLower && hasDigit) return "";
                else return string.Format("Password should have alpha/ numeric value, upper and lower case");
            }            
        }

        public static string GenerateCBSSalt(string userName, string sequenceNo, string timeStamp)
        {
            var salt = string.Concat(userName, "84A47863-BDD5-4949-B364-DD2C993FBE08" + "SPICY",sequenceNo,timeStamp); 

            var sha = System.Security.Cryptography.SHA256.Create();

            var hashed = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt));


            var binaryHashed = System.String.Empty;
            for (var x = 0; x < hashed.Length; x++)
            {
                binaryHashed = binaryHashed + System.String.Format("{0:x2}", hashed[x]);

            }

            return binaryHashed;
        }

        public static string GetCBS_ClientStatusCode(string CLIENT_STATUS_CODE)
        {
            string value = "";

            switch (CLIENT_STATUS_CODE)
            {
                case "A":
                    value = "ACTIVE";
                    break;
                case "H":
                    value = "HOLD";
                    break;
                case "T":
                    value = "TRMND";
                    break;
                default:
                    value = "";
                    break;
            }

            return value;
        }

        public static string GetCBS_AssociateTypeCode(string CLIENT_SUB_TYPE)
        {
            string value = "";

            switch (CLIENT_SUB_TYPE)
            {
                case "001":
                    value = "PVAO";
                    break;
                case "002":
                    value = "Dependent";
                    break;
                case "003":
                    value = "Cadet PMA";
                    break;
                case "004":
                    value = "Cadet PNPA";
                    break;
                case "005":
                    value = "Cadet OCS";
                    break;
                case "006":
                    value = "Cadet NOCC";
                    break;
                default:
                    value = "";
                    break;
            }

            return value;
        }

        public static string GetCBS_ClientType(string CLIENT_TYPE)
        {
            string value = "";

            switch (CLIENT_TYPE)
            {
                case "R":
                    value = "Regular";
                    break;
                case "A":
                    value = "Associate";
                    break;
                case "C":
                    value = "Corporate Regular";
                    break;
                case "D":
                    value = "Corporate Associate";
                    break;
                default:
                    value = "";
                    break;
            }

            return value;
        }

        public static string GetCBS_CivilStatus(string MARITAL_STATUS)
        {
            string value = "";

            switch (MARITAL_STATUS)
            {
                case "S":
                    value = "Single";
                    break;
                case "M":
                    value = "Married";
                    break;
                case "A":
                    value = "Divorced";
                    break;
                case "X":
                    value = "Separated";
                    break;
                case "W":
                    value = "Widowed";
                    break;
                default:
                    value = "";
                    break;
            }

            return value;
        }


    }
}
