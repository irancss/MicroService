﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    // IOrderCreated.cs
    public interface IOrderCreated
    {
        Guid OrderId { get; }
        DateTime CreatedAt { get; }
    }

}
