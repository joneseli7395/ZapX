using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Extensions;

namespace ZapX.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        public string FilePath { get; set; }
        [MaxFileSize(2000)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".pdf", ".doc"})]
        public byte[] FileData { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string UserId { get; set; }
        public virtual BTUser User { get; set; }
    }
}
