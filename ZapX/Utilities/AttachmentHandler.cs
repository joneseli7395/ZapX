using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Models;

namespace ZapX.Utilities
{
    public class AttachmentHandler
    {
        public TicketAttachment Attach(IFormFile attachment)
        {
            TicketAttachment ticketAttachment = new TicketAttachment();

            var memoryStream = new MemoryStream();
            attachment.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();
            var binary = Convert.ToBase64String(bytes);
            var ext = Path.GetExtension(attachment.FileName);

            ticketAttachment.FilePath = $"data:image/{ext};base64,{binary}";
            ticketAttachment.FileData = bytes;
            ticketAttachment.Created = DateTime.Now;

            return ticketAttachment;
        }
    }
}
