using MyApprovalsHub.Agent.Models;
using AdaptiveCards.Templating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using MyApprovalsHub.Common.Models;

namespace MyApprovalsHub.Agent.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ConversationBot _conversation;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "NotificationDefault.json");

        public NotificationController(ConversationBot conversation, IConfiguration configuration)
        {
            this._conversation = conversation;
            _configuration = configuration;
        }


        [HttpPost]
        public async Task<ActionResult> PostAsync(CancellationToken cancellationToken = default)
        {

            int membersCount = 0;
            string users = string.Empty;

            var installations = await this._conversation.Notification.GetInstallationsAsync(cancellationToken);

            if (installations.Count() == 0)
            {
                return Ok($"There are no users with the bot id: {_configuration.GetSection("BOT_ID")?.Value} installed");
            }

            using var content = new StreamContent(this.HttpContext.Request.Body);

            var contentString = await content.ReadAsStringAsync();

            PendingApproval pendingApproval = null;

            if (!string.IsNullOrEmpty(contentString))
            {
                pendingApproval = System.Text.Json.JsonSerializer.Deserialize<PendingApproval>(contentString);

                if (pendingApproval == null)
                {
                    return Ok("The pendingApproval information is missing or is invalid");

                }
            }

            // Read adaptive card template
            var cardTemplate = await System.IO.File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

            foreach (var installation in installations)
            {
                // "Person" means this bot is installed as Personal app
                if (installation.Type == NotificationTargetType.Person)
                {
                    var members = await installation.GetMembersAsync(cancellationToken);

                    if (members == null)
                    {
                        return Ok("The bothas no members.");
                    }

                    membersCount += members.Count();


                    // find the person
                    var member = members.ToList().Find(m => m.Account.UserPrincipalName == pendingApproval.RequestorEmail);


                    for (int i = 0; i < members.Length; i++)
                    {
                        users = users + members[i].Account.UserPrincipalName + " - " + members[i].Account.Name + "; BotAppId:" + installation.BotAppId;
                    }


                    if (member != null)
                    {

                        // Build and send adaptive card
                        var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
                        (
                            new NotificationDefaultModel
                            {
                                ShortDescription = pendingApproval.ShortDescription,
                                Description = pendingApproval.Description,
                                Number = pendingApproval.Number,
                                Status = pendingApproval.State,
                                RequestorEmail = pendingApproval.RequestorEmail,
                                RequestorName = pendingApproval.RequestorName,
                                OpenedAt = pendingApproval.OpenedAt,
                                ApproverName = pendingApproval.ApproverName,
                                ApproverEmail = pendingApproval.ApproverEmail,
                                ApprovedOnDate = pendingApproval.ApprovedOnDate,
                                NotificationUrl = pendingApproval.DetailsUrl
                            }
                        );

                        await member.SendAdaptiveCard(JsonConvert.DeserializeObject(cardContent), cancellationToken);
                    }
                }
            }


            return Ok($"Installations: {installations.Count()}\nMembers: {membersCount}\nUsers:{users}");
        }
    }
}
