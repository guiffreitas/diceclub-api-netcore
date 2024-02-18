﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public DateTime BirthDate { get; set; }

        //DBcontext reference navigation to dependent, One to One relation
        public UserProfile? Profile { get; set; }
        public User() : base() { }
    }
}
