using System.Collections.Generic;
using System.Linq;

namespace AcumaticaDocuSign
{
    public static class ErrorCodeHelper
    {
        private static readonly Dictionary<string, string> ErrorCodes = new Dictionary<string, string>
                                                                     {
                                                                         { Constants.ErrorCode.DocuSignAccountNotExists, Messages.DocuSignAccountNotExists },
                                                                         { Constants.ErrorCode.DocuSignEnvelopeNotExists, Messages.DocuSignEnvelopeNotExists }
                                                                     };

        public static string GetValueByKey(string code)
        {
            return ErrorCodes.SingleOrDefault(x => x.Key == code).Value;
        }
    }
}