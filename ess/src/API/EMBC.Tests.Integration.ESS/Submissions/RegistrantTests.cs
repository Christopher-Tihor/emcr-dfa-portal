﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMBC.ESS;
using EMBC.ESS.Managers.Submissions;
using EMBC.ESS.Shared.Contracts.Submissions;
using EMBC.ESS.Utilities.Dynamics;
using EMBC.ESS.Utilities.Dynamics.Microsoft.Dynamics.CRM;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace EMBC.Tests.Integration.ESS.Submissions
{
    public class RegistrantTests : WebAppTestBase
    {
        private readonly SubmissionsManager manager;
        private readonly string teamUserId = "988c03c5-94c8-42f6-bf83-ffc57326e216";

        private async Task<RegistrantProfile> GetRegistrantByUserId(string userId) => await TestHelper.GetRegistrantByUserId(manager, userId);

        private async Task<IEnumerable<EvacuationFile>> GetEvacuationFileById(string fileId) => await TestHelper.GetEvacuationFileById(manager, fileId);


        public RegistrantTests(ITestOutputHelper output, WebApplicationFactory<Startup> webApplicationFactory) : base(output, webApplicationFactory)
        {
            manager = services.GetRequiredService<SubmissionsManager>();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateNewRegistrantProfile()
        {
            var baseRegistrant = await GetRegistrantByUserId("CHRIS-TEST");

            var newProfileBceId = Guid.NewGuid().ToString("N").Substring(0, 10);
            var country = "CAN";
            var province = "BC";
            var community = "226adfaf-9f97-ea11-b813-005056830319";
            string city = null;

            baseRegistrant.Id = null;
            baseRegistrant.UserId = newProfileBceId;
            baseRegistrant.PrimaryAddress.Country = country;
            baseRegistrant.PrimaryAddress.StateProvince = province;
            baseRegistrant.PrimaryAddress.Community = community;
            baseRegistrant.PrimaryAddress.City = city;
            baseRegistrant.MailingAddress.Country = country;
            baseRegistrant.MailingAddress.StateProvince = province;
            baseRegistrant.MailingAddress.Community = community;
            baseRegistrant.MailingAddress.City = city;

            var id = await manager.Handle(new SaveRegistrantCommand { Profile = baseRegistrant });

            var newRegistrant = (await GetRegistrantByUserId(newProfileBceId)).ShouldNotBeNull();

            newRegistrant.Id.ShouldBe(id);
            newRegistrant.Id.ShouldNotBe(baseRegistrant.Id);
            newRegistrant.PrimaryAddress.Country.ShouldBe(country);
            newRegistrant.PrimaryAddress.StateProvince.ShouldBe(province);
            newRegistrant.PrimaryAddress.Community.ShouldBe(community);
            newRegistrant.PrimaryAddress.City.ShouldBe(city);

            newRegistrant.MailingAddress.Country.ShouldBe(country);
            newRegistrant.MailingAddress.StateProvince.ShouldBe(province);
            newRegistrant.MailingAddress.Community.ShouldBe(community);
            newRegistrant.MailingAddress.City.ShouldBe(city);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanDeleteRegistrantProfile()
        {
            var baseRegistrant = await GetRegistrantByUserId("CHRIS-TEST");

            baseRegistrant.Id = null;
            baseRegistrant.UserId = Guid.NewGuid().ToString("N").Substring(0, 10);

            var newRegistrantId = await manager.Handle(new SaveRegistrantCommand { Profile = baseRegistrant });
            newRegistrantId.ShouldNotBeNull();

            await manager.Handle(new DeleteRegistrantCommand { UserId = baseRegistrant.UserId });

            (await GetRegistrantByUserId(baseRegistrant.UserId)).ShouldBeNull();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateProfile()
        {
            var registrant = await GetRegistrantByUserId("CHRIS-TEST");
            var currentCommunity = registrant.PrimaryAddress.Community;
            var newCommunity = currentCommunity == "406adfaf-9f97-ea11-b813-005056830319"
                ? "226adfaf-9f97-ea11-b813-005056830319"
                : "406adfaf-9f97-ea11-b813-005056830319";

            string currentCountry = registrant.PrimaryAddress.Country;
            string newCountry;
            string newProvince;
            string newCity;
            string newPostalCode;
            if (currentCountry.Equals("CAN"))
            {
                newCountry = "USA";
                newProvince = "WA";
                newCity = "Seattle";
                newPostalCode = "12345";
            }
            else
            {
                newCountry = "CAN";
                newProvince = "BC";
                newCity = "Vancouver";
                newPostalCode = "v1v 1v1";
            }

            registrant.PrimaryAddress.Country = newCountry;
            registrant.PrimaryAddress.StateProvince = newProvince;
            registrant.PrimaryAddress.Community = newCommunity;
            registrant.PrimaryAddress.City = newCity;
            registrant.PrimaryAddress.PostalCode = newPostalCode;

            string newEmail = "christest3@email" + Guid.NewGuid().ToString("N").Substring(0, 10);
            registrant.Email = newEmail;

            var currentPhone = registrant.Phone;
            string newPhone = currentPhone == "7789877897" ? "6045557777" : "7789998888";
            registrant.Phone = newPhone;

            var id = await manager.Handle(new SaveRegistrantCommand { Profile = registrant });

            var updatedRegistrant = await GetRegistrantByUserId("CHRIS-TEST");
            updatedRegistrant.Id.ShouldBe(id);
            updatedRegistrant.Id.ShouldBe(registrant.Id);
            updatedRegistrant.Email.ShouldBe(newEmail);
            updatedRegistrant.Phone.ShouldBe(newPhone);
            updatedRegistrant.PrimaryAddress.Country.ShouldBe(newCountry);
            updatedRegistrant.PrimaryAddress.StateProvince.ShouldBe(newProvince);
            updatedRegistrant.PrimaryAddress.Community.ShouldBe(newCommunity);
            updatedRegistrant.PrimaryAddress.City.ShouldBe(newCity);
            updatedRegistrant.PrimaryAddress.PostalCode.ShouldBe(newPostalCode);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task Link_RegistrantToHouseholdMember_ReturnsRegistrantId()
        {
            var baseRegistrant = await GetRegistrantByUserId("CHRIS-TEST");

            var newProfileBceId = Guid.NewGuid().ToString("N").Substring(0, 10);
            var country = "CAN";
            var province = "BC";
            var community = "226adfaf-9f97-ea11-b813-005056830319";
            string city = null;

            baseRegistrant.Id = null;
            baseRegistrant.UserId = newProfileBceId;
            baseRegistrant.PrimaryAddress.Country = country;
            baseRegistrant.PrimaryAddress.StateProvince = province;
            baseRegistrant.PrimaryAddress.Community = community;
            baseRegistrant.PrimaryAddress.City = city;

            var id = await manager.Handle(new SaveRegistrantCommand { Profile = baseRegistrant });

            var registrant = (await GetRegistrantByUserId(newProfileBceId)).ShouldNotBeNull();

            var file = (await GetEvacuationFileById("101010")).FirstOrDefault();
            var member = file.NeedsAssessment.HouseholdMembers.FirstOrDefault();

            var fileId = await manager.Handle(new LinkRegistrantCommand { FileId = file.Id, RegistantId = registrant.Id, HouseholdMemberId = member.Id });
            fileId.ShouldBe(file.Id);

            var updatedFile = (await GetEvacuationFileById("101010")).FirstOrDefault();
            updatedFile.NeedsAssessment.HouseholdMembers.Where(m => m.Id == member.Id).SingleOrDefault().LinkedRegistrantId.ShouldBe(registrant.Id);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanDeleteProfileAddressLinks()
        {
            var registrant = await GetRegistrantByUserId("CHRIS-TEST");
            if (string.IsNullOrEmpty(registrant.PrimaryAddress.StateProvince))
            {
                await CanUpdateProfile(); //populates the community and state/province fields
                registrant = await GetRegistrantByUserId("CHRIS-TEST");
            }

            string newProvince = null;
            string newCommunity = null;
            registrant.PrimaryAddress.StateProvince = newProvince;
            registrant.PrimaryAddress.Community = newCommunity;

            var id = await manager.Handle(new SaveRegistrantCommand { Profile = registrant });

            var updatedRegistrant = await GetRegistrantByUserId("CHRIS-TEST");
            updatedRegistrant.Id.ShouldBe(id);
            updatedRegistrant.Id.ShouldBe(registrant.Id);
            updatedRegistrant.PrimaryAddress.StateProvince.ShouldBe(newProvince);
            updatedRegistrant.PrimaryAddress.Community.ShouldBe(newCommunity);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanUpdateRegistrantVerified()
        {
            var registrant = await GetRegistrantByUserId("CHRIS-TEST");
            var currentVerifiedStatus = registrant.VerifiedUser;
            var newStatus = !currentVerifiedStatus;

            var id = await manager.Handle(new SetRegistrantVerificationStatusCommand { RegistrantId = registrant.Id, Verified = newStatus });

            var updatedRegistrant = (await GetRegistrantByUserId("CHRIS-TEST"));
            updatedRegistrant.Id.ShouldBe(id);
            updatedRegistrant.Id.ShouldBe(registrant.Id);
            updatedRegistrant.VerifiedUser.ShouldBe(newStatus);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanVerifySecurityQuestions()
        {
            var expectedNumberOfCorrectAnswers = 3;
            var registrant = await GetRegistrantByUserId("CHRIS-TEST");

            //if need to update security questions
            //List<SecurityQuestion> securityQuestions = new List<SecurityQuestion>();
            //securityQuestions.Add(new SecurityQuestion { Id = 1, Question = "question1", Answer = "answer1 - test", AnswerChanged = true });
            //securityQuestions.Add(new SecurityQuestion { Id = 2, Question = "question2", Answer = "answer2 - test", AnswerChanged = true });
            //securityQuestions.Add(new SecurityQuestion { Id = 3, Question = "question3", Answer = "answer3 - test", AnswerChanged = true });
            //registrant.SecurityQuestions = securityQuestions;
            //await manager.Handle(new SaveRegistrantCommand { Profile = registrant });

            var answers = registrant.SecurityQuestions.Select(q =>
            {
                q.Answer = $"answer{q.Id}";
                return q;
            }).ToArray();
            var actualNumberOfCorrectAnswers = await manager.Handle(new VerifySecurityQuestionsQuery { Answers = answers, RegistrantId = registrant.Id });

            actualNumberOfCorrectAnswers.NumberOfCorrectAnswers.ShouldBe(expectedNumberOfCorrectAnswers);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanPartlyVerifySecurityQuestions()
        {
            var expectedNumberOfCorrectAnswers = 2;
            var registrant = await GetRegistrantByUserId("CHRIS-TEST");

            var answers = registrant.SecurityQuestions.Select(q =>
            {
                q.Answer = $"answer{q.Id}";
                return q;
            }).ToArray();
            answers[2].Answer = "wrong";
            var actualNumberOfCorrectAnswers = await manager.Handle(new VerifySecurityQuestionsQuery { Answers = answers, RegistrantId = registrant.Id });

            actualNumberOfCorrectAnswers.NumberOfCorrectAnswers.ShouldBe(expectedNumberOfCorrectAnswers);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanReprintSupport()
        {
            var dynamicsContext = services.GetRequiredService<EssContext>();
            var testPrintRequest = dynamicsContext.era_referralprints
                .Expand(pr => pr.era_ESSFileId)
                .Where(pr => pr.statecode == (int)EntityState.Active && pr._era_requestinguserid_value != null)
                .OrderByDescending(pr => pr.createdon)
                .Take(new Random().Next(20))
                .ToArray()
                .First();

            await dynamicsContext.LoadPropertyAsync(testPrintRequest, nameof(era_referralprint.era_era_referralprint_era_evacueesupport));

            var supportId = testPrintRequest.era_era_referralprint_era_evacueesupport.First().era_name;
            var fileId = testPrintRequest.era_ESSFileId.era_name;

            var printRequestId = await manager.Handle(new ReprintSupportCommand
            {
                FileId = fileId,
                ReprintReason = "test",
                RequestingUserId = teamUserId,
                SupportId = supportId
            });

            printRequestId.ShouldNotBeNullOrEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanInviteRegistrant()
        {
            var registrant = await GetRegistrantByUserId("CHRIS-TEST");
            var inviteId = await manager.Handle(new InviteRegistrantCommand
            {
                ContactId = registrant.Id,
                Email = "test@nowhere.notavailable",
                RequestingUserId = null
            });
            inviteId.ShouldNotBeNullOrEmpty();
        }
    }
}
