
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
        public static bool IsPasswordValid(string userPass)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]*$");
            int userPassLength = 8;
            bool bln = false;

            if (userPass.Length < userPassLength) ShowWarningMessage(string.Format("Password should be {0} characters long", userPassLength));
            else if (!r.IsMatch(userPass)) ShowWarningMessage(string.Format("Password should be alphanumeric"));
            else {
                bool hasUpper = false; bool hasLower = false; bool hasDigit = false;
                for (int i = 0; i < userPass.Length && !(hasUpper && hasLower && hasDigit); i++)
                {
                    char c = userPass[i];
                    if (!hasUpper) hasUpper = char.IsUpper(c);
                    if (!hasLower) hasLower = char.IsLower(c);
                    if (!hasDigit) hasDigit = char.IsDigit(c);
                }

                if (hasUpper && hasLower && hasDigit) bln = true;
                else ShowWarningMessage(string.Format("Password should have numeric value, upper and lower case"));
            }

            return bln;
        }


    }
}
