using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;

namespace PDM.BusinessLogic
{
    public class MicrosoftOutlook
    {
        public void Start(List<string> filePaths)
        {
            var application = new Application();

            var mail = (MailItem)application.CreateItem(OlItemType.olMailItem);

            for (int i = 0; i < filePaths.Count(); i++)
            {
                var filePath = filePaths.ElementAt(i);

                //try
                {
                    mail.Attachments.Add(filePath, OlAttachmentType.olByValue, i + 1, filePath);
                }
                //catch (COMException)
                {
                    //System.Runtime.InteropServices.COMException (0x80004005): The attachment size exceeds the allowable limit.
                    //Show error to user?
                }
            }

            mail.Body = string.Empty;

            mail.Display(false);
        }  
    }
}
