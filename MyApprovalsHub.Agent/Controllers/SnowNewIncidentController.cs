using AdaptiveCards.Templating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Ediscovery;
using Microsoft.TeamsFx.Conversation;
using MyApprovalsHub.Agent.Models;
using MyApprovalsHub.Common.Models;
using Newtonsoft.Json;

namespace MyApprovalsHub.Agent.Controllers
{

    [Route("api/snownewincident")]
    [ApiController]
    public class SnowNewIncidentController : Controller
    {
        private readonly ConversationBot _conversation;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "NotificationSnowNewIncident.json");

        public SnowNewIncidentController(ConversationBot conversation)
        {
            this._conversation = conversation;
        }


        [HttpPost]
        public async Task<ActionResult> PostAsync(CancellationToken cancellationToken = default)
        {

            int membersCount = 0;
            string users = string.Empty;

            var installations = await this._conversation.Notification.GetInstallationsAsync(cancellationToken);

            if (installations.Count() == 0)
            {
                return Ok("There are no users with the bot installed");
            }

            using var content = new StreamContent(this.HttpContext.Request.Body);

            var contentString = await content.ReadAsStringAsync();

            SnowIncidentModel snowIncidentModel = null;

            if (!string.IsNullOrEmpty(contentString))
            {
                snowIncidentModel = JsonConvert.DeserializeObject<SnowIncidentModel>(contentString);

                if (snowIncidentModel == null)
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
                        return Ok("The bot has no members.");
                    }

                    membersCount += members.Count();


                    // find the person
                    var member = members.ToList().Find(m => m.Account.UserPrincipalName == snowIncidentModel.Requestor);


                    for (int i = 0; i < members.Length; i++)
                    {
                        users = users + members[i].Account.UserPrincipalName + " - " + members[i].Account.Name + "; BotAppId:" + installation.BotAppId;
                    }


                    if (member != null)
                    {

                        // Build and send adaptive card
                        var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
                        (
                            new SnowIncidentModel
                            {
                                CaseID = snowIncidentModel.CaseID,
                                Description = snowIncidentModel.Description,
                                OpenedAt = snowIncidentModel.OpenedAt,
                                Requestor = snowIncidentModel.Requestor,
                                ShortDescription = snowIncidentModel.ShortDescription,
                                LongDescription = snowIncidentModel.LongDescription,
                                Title = snowIncidentModel.Title,
                                ViewDetailsUrl = snowIncidentModel.ViewDetailsUrl
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
