﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.ValueObjects
{
    public class ApiUrls
    {
        public string EmailConfirmationUrl { get; set; } = string.Empty;
        public string PasswordResetUrl { get; set; } = string.Empty;
    }
}
