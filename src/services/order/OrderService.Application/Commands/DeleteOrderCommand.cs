﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace OrderService.Application.Commands
{
    public class DeleteOrderCommand : IRequest<bool> { 
        public Guid Id { get; set; }
    }


}
