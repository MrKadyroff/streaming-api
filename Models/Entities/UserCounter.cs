using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities
{
    [Keyless]
    public class UserCounter
    {
        public int Count { get; set; }

    }
}
