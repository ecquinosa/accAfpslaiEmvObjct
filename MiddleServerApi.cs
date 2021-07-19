using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace accAfpslaiEmvObjct
{
    public class MiddleServerApi
    {

        public MiddleServerApi(string baseUrl, string apiKey, string branchIssue, afpslaiEmvSystem systm)
        {
            this.baseUrl = baseUrl;
            this.apiKey = apiKey;
            this.branchIssue = branchIssue;            
            switch (systm)
            {
                case afpslaiEmvSystem.dcs:
                    this.system = "dcs";
                    msgHeader = "Data Capture System";
                    break;
                case afpslaiEmvSystem.cps:
                    this.system = "cps";
                    msgHeader = "Card Printing and Merging Data Software";
                    break;
                case afpslaiEmvSystem.urm:
                    this.system = "urm";
                    msgHeader = "User and Role Management";
                    break;
                case afpslaiEmvSystem.login:
                    this.system = "login";
                    msgHeader = "Login";
                    break;
            } 
        }

        private string system;
        private string baseUrl;
        private string apiKey;
        private string branchIssue;
        public static string msgHeader;
        public user dcsUser = null;

        public enum afpslaiEmvSystem
        {
            dcs = 1,
            cps,
            urm,
            login
        }

        public enum msApi
        {
            getOnlineRegByCIF = 1,
            getRole,
            getAssociateType,
            getBranch,
            getCivilStatus,
            getCountry,
            getMembershipStatus,
            getMembershipType,
            getPrintType,
            getRecardReason,
            getCard,
            getAddress,
            validateLogIn,
            addSystemUser,
            addOnlineRegistration,
            addMember,
            addCard,
            addAddress,
            addSystemRole,
            delSystemRole,
            addAssociateType,
            delAssociateType,
            addBranch,
            delBranch,
            addCivilStatus,
            delCivilStatus,
            addMembershipStatus,
            delMembershipStatus,
            addMembershipType,
            delMembershipType,
            addPrintType,
            delPrintType,
            addRecardReason,
            delRecardReason,
            checkServerDBStatus,
            getDCSSystemSetting,
            getOnlineRegistration,
            saveMemberImages,
            cancelCapture,
            addDCSSystemSettings,
            checkMemberIfCaptured,
            pullCBSData,
            getCardForPrint,
            getSystemUser,
            delSystemUser,
            resetUserPassword,
            changeUserPassword,
            addCPSCardElements,
            getCpsCardElements,
            pushCMSData,
            getMembersPrintingTypeSummary,
            getMembersRecardReasonSummary,
            getMember
        }

        public enum cpsCardElement
        {
            photo=1,
            memberSince,
            validThru,
            name,
            cif
        }

        public bool ExecuteApiRequest(string url, string soapStr, ref string soapResponse, ref string err)
        {
            HttpWebRequest myHttpWebRequest = null;
            HttpClient client = new HttpClient();

            try
            {
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                var uri = new Uri(url);
                string _baseUrl = string.Format("http://{0}", uri.Authority);
                if (baseUrl.Contains("https://")) _baseUrl = string.Format("https://{0}", uri.Authority);
                string otherUrl = uri.LocalPath;

                client.BaseAddress = new Uri(_baseUrl);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", txtToken.Text);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var buffer = System.Text.Encoding.UTF8.GetBytes(soapStr);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                byteContent.Headers.ContentLength = buffer.Length;

                HttpResponseMessage response = client.PostAsync(otherUrl, byteContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    soapResponse = response.Content.ReadAsStringAsync().Result;
                    return true;
                }
                else
                {
                    err = string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase);
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("One or more errors occurred.")) err = "Unable to reach middle server api.";
                else err = ex.Message;
                return false;
            }
            finally
            {
                client.Dispose();
                myHttpWebRequest = null;
            }
        }

        public bool ValidateLogIn(string userName, string userPass)
        {
            loginRequest user = new loginRequest();
            user.user_name = userName;
            user.user_pass = userPass;

            requestCredential cred = new requestCredential();
            cred.dateRequest = DateTime.Now.ToString();
            cred.key = apiKey;
            cred.userId = 0;
            cred.userName = userName;
            cred.userPass = userPass;
            cred.branch = branchIssue;

            requestPayload reqPayload = new requestPayload();
            reqPayload.authentication = accAfpslaiEmvEncDec.Aes256CbcEncrypter.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(cred));
            reqPayload.payload = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            reqPayload.system = system;

            string soapResponse = "";
            string err = "";

            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(reqPayload);
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.validateLogIn)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj[0].ToString();
                    dcsUser = Newtonsoft.Json.JsonConvert.DeserializeObject<user>(obj);

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool GetTable(msApi api, ref object obj, string optPayload = "")
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(optPayload));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), api)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        private requestCredential requestCredential()
        {
            requestCredential cred = new requestCredential();
            cred.dateRequest = DateTime.Now.ToString();
            cred.key = apiKey;
            //cred.userId = 0;
            //cred.userName = "admin";
            //cred.userPass = "eyJpdiI6InNad2RnWndKdjUvUlRkVnZFelB1UGc9PSIsInZhbHVlIjoiQ1NwWVdMbnZMcmdRSmxEUlUrczV6dz09IiwibWFjIjoiYjNhNjI1YmNjMzQ4NWRlMDdlYzhjZDRkYWE4M2M1MDRhNTdhYmI5ZTdiYjcwOGE4M2I0NjRkYjMyZGY4Njc0OCJ9";

            cred.userId = dcsUser.userId;
            cred.userName = dcsUser.userName;
            cred.userPass = dcsUser.userPass;
            cred.branch = branchIssue;

            return cred;
        }

        private requestPayload requestPayload(string payload)
        {
            requestPayload reqPayload = new requestPayload();
            reqPayload.system = this.system;
            reqPayload.authentication = accAfpslaiEmvEncDec.Aes256CbcEncrypter.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(requestCredential()));
            reqPayload.payload = payload;

            return reqPayload;
        }

        public bool GetTable2(msApi api, ref object obj)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(""));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), api)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool GetCard(ref object obj)
        {
            string soapResponse = "";
            string err = "";
            //var s = "{ value: test }";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload("{ 'value': 'test' }"));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.getCard)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool GetCardForPrint(string cif, ref object obj)
        {
            string soapResponse = "";
            string err = "";          
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload("{ 'cif': '" + cif + "' }"));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.getCardForPrint)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool GetMember(int memberId, string cif, string branch, ref object obj)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload("{ 'memberId': " + memberId + ",'cif': '" + cif + "','branch': '" + branch + "' }"));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.getMember)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool GetMembersPrintingTypeSummary(string branch, DateTime startDate, DateTime endDate, ref object obj)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload("{ 'startDate': '" + startDate + "','endDate': '" + endDate + "','branch': '" + branch + "' }"));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.getMembersPrintingTypeSummary)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }
              
        public bool GetMembersRecardReasonSummary(string branch, DateTime startDate, DateTime endDate, ref object obj)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload("{ 'startDate': '" + startDate + "','endDate': '" + endDate + "','branch': '" + branch + "' }"));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.getMembersRecardReasonSummary)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool checkServerDBStatus(string url = "")
        {
            if (url == "") url = baseUrl;

            string soapResponse = "";
            string err = "";
            string soapStr = "";
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", url, System.Enum.GetName(typeof(msApi), msApi.checkServerDBStatus)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj;
                    if (obj != null) return true; else return false;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool addMember(member member, ref int memberId)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(member)));

            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.addMember)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj;

                    memberId = (int)obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool addDeleteGenericTable(object inputObj, bool isAdd = true)
        {
            var obj = (dynamic)null;
            msApi api = msApi.addAssociateType;

            if (inputObj is associate_type)
            {
                obj = (associate_type)inputObj;
                if (isAdd) api = msApi.addAssociateType; else api = msApi.delAssociateType;
            }
            else if (inputObj is branch)
            {
                obj = (branch)inputObj;
                if (isAdd) api = msApi.addBranch; else api = msApi.delBranch;
            }
            else if (inputObj is civil_status)
            {
                obj = (civil_status)inputObj;
                if (isAdd) api = msApi.addCivilStatus; else api = msApi.delCivilStatus;
            }
            else if (inputObj is membership_status)
            {
                obj = (membership_status)inputObj;
                if (isAdd) api = msApi.addMembershipStatus; else api = msApi.delMembershipStatus;
            }
            else if (inputObj is membership_type)
            {
                obj = (membership_type)inputObj;
                if (isAdd) api = msApi.addMembershipType; else api = msApi.delMembershipType;
            }
            else if (inputObj is print_type)
            {
                obj = (print_type)inputObj;
                if (isAdd) api = msApi.addPrintType; else api = msApi.delPrintType;
            }
            else if (inputObj is recard_reason)
            {
                obj = (recard_reason)inputObj;
                if (isAdd) api = msApi.addRecardReason; else api = msApi.delRecardReason;
            }
            else if (inputObj is system_role)
            {
                obj = (system_role)inputObj;
                if (isAdd) api = msApi.addSystemRole; else api = msApi.delSystemRole;
            }
            else if (inputObj is system_user)
            {
                obj = (system_user)inputObj;
                api = msApi.delSystemUser;
            }


            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(obj)));

            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), api)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    //var obj = payload.obj;
                    Utilities.ShowInformationMessage(message.ToString());

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool addEditUser(system_user user)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(user)));

            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.addSystemUser)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {                    
                    Utilities.ShowInformationMessage(message.ToString());

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool resetUserPassword(system_user user)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(user)));

            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.resetUserPassword)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    Utilities.ShowInformationMessage(message.ToString());

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool changeUserPassword(int userId, string userOldPass, string userNewPass)
        {
            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload("{ 'userId': " + userId + ", 'userOldPass': '" + userOldPass + "', 'userNewPass': '" + userNewPass + "' }"));

            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.changeUserPassword)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    Utilities.ShowInformationMessage(message.ToString());

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }
        public bool checkMemberIfCaptured(member member)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(member)));
            //string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(member);
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.checkMemberIfCaptured)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    //var obj = payload.obj;
                    //dcsUser = Newtonsoft.Json.JsonConvert.DeserializeObject<user>(obj);

                    //memberId = (int)obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool PushCMSData(cbsCms cbsCms)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(cbsCms)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.pushCMSData)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    //var obj = payload.obj;
                    if(!string.IsNullOrEmpty(message.ToString())) Utilities.ShowWarningMessage(message.ToString());

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool addCard(card card, ref int cardId)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(card)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.addCard)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj;
                    cardId = (int)obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool addDCSSystemSettings(dcs_system_setting dcs_system_setting)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(dcs_system_setting)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.addDCSSystemSettings)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    //var obj = payload.obj;

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool saveMemberImages(memberImages memberImages)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(memberImages)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.saveMemberImages)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj;
                    //dcsUser = Newtonsoft.Json.JsonConvert.DeserializeObject<user>(obj);

                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool addAddress(address address, ref int addressId)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(address)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.addAddress)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj;
                    addressId = (int)obj;
                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool AddCPSCardElements(cps_card_elements cce)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(cce)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.addCPSCardElements)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    var obj = payload.obj;                    
                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

        public bool cancelCapture(cancelCapture cancelCapture)
        {

            string soapResponse = "";
            string err = "";
            string soapStr = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload(Newtonsoft.Json.JsonConvert.SerializeObject(cancelCapture)));
            bool response = ExecuteApiRequest(string.Format(@"{0}/api/{1}", baseUrl, System.Enum.GetName(typeof(msApi), msApi.cancelCapture)), soapStr, ref soapResponse, ref err);
            if (response)
            {
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(soapResponse);
                var result = payload.result;
                var message = payload.message;
                if (result == 0)
                {
                    return true;
                }
                else
                {
                    Utilities.ShowErrorMessage(message.ToString());
                    return false;
                }
            }
            else
            {
                Utilities.ShowErrorMessage(err);
                return false;
            }
        }

    }

}
