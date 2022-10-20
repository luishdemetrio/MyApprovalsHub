using MyApprovalsHub.Bot.Models;
using AdaptiveCards.Templating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using System.Text;

namespace MyApprovalsHub.Bot.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ConversationBot _conversation;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "NotificationDefault.json");

        public NotificationController(ConversationBot conversation)
        {
            this._conversation = conversation;
        }


     

        [HttpPost]
        public async Task<ActionResult> PostAsync(CancellationToken cancellationToken = default)
        {
            // Read adaptive card template
            var cardTemplate = await System.IO.File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

            var installations = await this._conversation.Notification.GetInstallationsAsync(cancellationToken);

            foreach (var installation in installations)
            {

                // "Person" means this bot is installed as Personal app
                if (installation.Type == NotificationTargetType.Person)
                {

                    var members = await installation.GetMembersAsync(cancellationToken);

                    if (members != null)
                    {

                        using var content = new StreamContent(this.HttpContext.Request.Body);

                        var contentString = await content.ReadAsStringAsync();

                        string userPrincipalName = string.Empty;

                        if (!string.IsNullOrEmpty(contentString))
                        {
                            dynamic jsonResponse = JsonConvert.DeserializeObject(contentString);

                            userPrincipalName = jsonResponse.UserPrincipalName;

                            // find the person
                            var member = members.ToList().Find(m => m.Account.UserPrincipalName == userPrincipalName);

                            if (member != null)
                            {
                                // Build and send adaptive card
                                var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
                                (
                                    new NotificationDefaultModel
                                    {
                                        Title = "New Event Occurred!",
                                        AppName = "Contoso App Notification",
                                        Description = $"This is a sample http-triggered notification to {installation.Type}",
                                        NotificationUrl = "https://www.adaptivecards.io/",
                                    }
                                );

                                await member.SendAdaptiveCard(JsonConvert.DeserializeObject(cardContent), cancellationToken);
                            }
                        }

                       
                    }
                    
                    
                    //// Build and send adaptive card
                    //var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
                    //(
                    //    new NotificationDefaultModel
                    //    {
                    //        Title = "New Event Occurred!",
                    //        AppName = "Contoso App Notification",
                    //        Description = $"This is a sample http-triggered notification to {installation.Type}",
                    //        NotificationUrl = "https://www.adaptivecards.io/",
                    //    }
                    //);

                    //// Directly notify the individual person
                    //await installation.SendAdaptiveCard(JsonConvert.DeserializeObject(cardContent), cancellationToken);
                }


               
            }

            return Ok();
        }
    }
}
