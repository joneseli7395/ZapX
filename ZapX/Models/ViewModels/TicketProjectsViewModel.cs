﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZapX.Models.ViewModels
{
    public class TicketProjectsViewModel
    {
        public Project Project { get; set; }
        public Ticket Ticket { get; set; }
        public List<Ticket> Tickets { get; set; }
}
}