﻿using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindByClaimPrincipleWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user.FindFirst(ClaimTypes.Email).Value;

            return await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
        }

        public static async Task<AppUser> FindByEmailFromClaimPrincile(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user.FindFirst(ClaimTypes.Email).Value;

            return await userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
