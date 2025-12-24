using eSignASPLibrary.Util;
using System.Collections.Generic;


namespace eSignASPLibrary
{
    public sealed class eSign
    {
        private eSignInternal sign;
        public enum status
        {
            Failure = 0,
            Success = 1,
            Pending = 2,
        }

        public enum DocType
        {
            Pdf = 0,
            Hash = 1,
            eMandate = 2
        }

        public enum AppreanceRunDirection
        {
            RUN_DIRECTION_LTR,
            RUN_DIRECTION_RTL
        }

        public enum Coordinates
        {
            Top_Left = 1,
            Top_Center = 2,
            Top_Right = 3,
            Middle_Left = 4,
            Middle_Right = 5,
            Middle_Center = 6,
            Bottom_Left = 7,
            Bottom_Center = 8,
            Bottom_Right = 9
        }

        public enum PageToBeSigned
        {
            FIRST = 1,
            LAST = 2,
            EVEN = 3,
            ODD = 4,
            ALL = 5,
            SPECIFY = 6,
            PAGE_LEVEL = 7
        }

        public enum eSignAPIVersion
        {
            V2,
            V3
        }

        public enum AuthMode
        {
            OTP = 1,
            Fingerprint = 2,
            IRIS = 3,
            FACE = 4
        }        
        public eSign(string PFXFilePath, string PFXPassword, string PFXAlias, bool IsProxyRequired, string ProxyIP, int ProxyPort, string ProxyUserName, string ProxyPassword, string ASPID, string eSignURL, string eSignURLV2, string eSignCheckStatusURL, int SignatureContents = 21000)
        {
            eSignSettings.isProxyRequired = IsProxyRequired;
            eSignSettings.proxyIP = ProxyIP;
            eSignSettings.proxyPort = ProxyPort;
            eSignSettings.userName = ProxyUserName;
            eSignSettings.password = ProxyPassword;
            eSignSettings.pfxPath = PFXFilePath;
            eSignSettings.pfxPassword = PFXPassword;
            eSignSettings.pfxAlias = PFXAlias;
            eSignSettings.SignatureContents = SignatureContents;
            eSignSettings.ASPID = ASPID;
            eSignSettings.eSignURL = eSignURL;
            eSignSettings.eSignURLV2 = eSignURLV2;
            eSignSettings.eSignCheckStatusURL = eSignCheckStatusURL;
            sign = new eSignInternal();
        }
        public eSignServiceReturn GetGateWayParam(List<eSignInput> eSignInputs, string signerID, string transactionID, string resposeURL, string redirectUrl, string TempFolderPath, eSignAPIVersion eSignAPIVersion = eSignAPIVersion.V3, AuthMode authMode = AuthMode.OTP, bool isLTVRequired = true)
        {
            return sign.GetGateWayParam(eSignInputs, signerID, transactionID, resposeURL, redirectUrl, TempFolderPath, eSignAPIVersion, authMode, isLTVRequired);
        }
        public eSignServiceReturn GetSigedDocument(string ResponseXML, string PreSignedTempFile)
        {
            return sign.GetSigedDocument(ResponseXML, PreSignedTempFile);
        }
        public eSignServiceReturn GetSigedDocument(string ResponseXML, byte[] PreSignedDocBytes)
        {
            return sign.GetSigedDocument(ResponseXML, PreSignedDocBytes);
        }
        public eSignServiceReturn GetStatus(string transactionID)
        {
            return sign.GetStatus(transactionID);
        }
    }
}
