﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class Auth0TokenResponseDTO
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
