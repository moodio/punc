using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Punc.Utils
{
    public static class EmailTemplateEngine
    {
        public static MailMessage GenerateMessage(MailAddress from, MailAddress to, string subject, string template,
            Dictionary<string, string> vars)
        {
            var message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = from;
            message.To.Add(to);
            message.Subject = subject;
            message.Body = ReplaceTokens(template, vars);

            return message;
        }

        /// <summary>
        /// Replace tokens with their variables if they exist
        /// Tokens are the variable names padded with a '#{' at the start, and '}#' to form the format #{[TokenName]}#
        /// i.e. for the Name variable, the token is #{Name}#
        /// </summary>
        /// <param name="content"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static string ReplaceTokens(string content, Dictionary<string, string> variables)
        {
            var res = content;
            foreach (var var in variables)
            {
                res = res.Replace($"#{{{var.Key}}}#",var.Value);
            }

            return res;
        }
    }
}
