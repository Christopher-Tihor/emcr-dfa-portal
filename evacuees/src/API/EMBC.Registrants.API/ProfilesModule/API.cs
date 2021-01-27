﻿// -------------------------------------------------------------------------
//  Copyright © 2020 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  https://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using EMBC.Registrants.API.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMBC.Registrants.API.ProfilesModule
{
    [Route("api/profiles")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileManager profileManager;

        public ProfileController(IProfileManager profileManager)
        {
            this.profileManager = profileManager;
        }

        /// <summary>
        /// Get the current logged in user's profile
        /// </summary>
        /// <returns>Currently logged in user's profile</returns>
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<Profile>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await profileManager.GetProfileByBceid(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Create or update the current user's profile
        /// </summary>
        /// <param name="profile">The profile information</param>
        /// <returns>profile id</returns>
        [HttpPost("current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<string>> Upsert(Profile profile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.Id == null)
            {
                profile.Id = userId;
                await profileManager.CreateProfile(profile);
            }
            else
            {
                await profileManager.UpdateProfile(profile);
            }

            return Ok(profile.Id);
        }

        /// <summary>
        /// Get the logged in user's profile and conflicts with the data that came from the authenticating identity provider
        /// </summary>
        /// <returns>The current user's profile, the identity provider's profile and the detected conflicts</returns>
        [HttpGet("current/conflicts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<UserProfile>> GetProfileConflicts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userProfileWithConflicts = await profileManager.GetProfileAndConflicts(userId);
            if (userProfileWithConflicts == null) return NotFound();
            return Ok(userProfileWithConflicts);
        }
    }

    /// <summary>
    /// User's profile
    /// </summary>
    public class Profile
    {
        public string Id { get; set; }

        [Required]
        public PersonDetails PersonalDetails { get; set; }

        [Required]
        public ContactDetails ContactDetails { get; set; }

        [Required]
        public Address PrimaryAddress { get; set; }

        public Address MailingAddress { get; set; }
        public bool IsMailingAddressSameAsPrimaryAddress { get; set; }
        public bool RestrictedAccess { get; set; }
        public string SecretPhrase { get; set; }
    }

    /// <summary>
    /// DTO for conflict resolution data
    /// </summary>
    public class UserProfile
    {
        public bool IsNewUser => EraProfile == null;
        public Profile EraProfile { get; set; }
        public Profile LoginProfile { get; set; }
        public IEnumerable<ProfileDataConflict> Conflicts { get; set; }
    }

    /// <summary>
    /// profile data element name in conflict
    /// </summary>
    public class ProfileDataConflict
    {
        public string ConflictDataElement { get; set; }
    }
}