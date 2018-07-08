using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using JetBrains.Annotations;
using MimeKit;
using MimeKit.IO;
using ContentDisposition = MimeKit.ContentDisposition;
using ContentType = MimeKit.ContentType;

namespace Scalider.Mail
{

    /// <summary>
    /// Provides extension methods for the <see cref="MailMessage"/> class.
    /// </summary>
    /// <remarks>
    /// Based on the implementation by the ABP project
    /// https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp.MailKit/EmailExtensions.cs
    /// </remarks>
    public static class MailMessageExtensions
    {
        
        #region ToMimeMessage

        /// <summary>
        /// Converts a <see cref="MailMessage"/> to a <see cref="MimeMessage"/>.
        /// </summary>
        /// <param name="mail">The <see cref="MailMessage"/> to convert.</param>
        /// <returns>
        /// The <see cref="MimeMessage"/>.
        /// </returns>
        public static MimeMessage ToMimeMessage([NotNull] this MailMessage mail)
        {
            Check.NotNull(mail, nameof(mail));

            // Convert all the message headers to the required type
            var headers = from field in mail.Headers.AllKeys
                          from value in mail.Headers.GetValues(field)
                          select new Header(field, value);

            // Create the mime message
            var message = new MimeMessage(headers);

            // Note: If the user has already sent their MailMessage via System.Net.Mail.SmtpClient,
            // then the following MailMessage properties will have been merged into the Headers, so
            // check to make sure our MimeMessage properties are empty before adding them.
            if (mail.Sender != null)
                message.Sender = ToMailboxAddress(mail.Sender);

            if (mail.From != null)
            {
                message.Headers.Replace(HeaderId.From, string.Empty);
                message.From.Add(ToMailboxAddress(mail.From));
            }

            CopyAddressList(mail.ReplyToList, message.ReplyTo, message, HeaderId.ReplyTo);
            CopyAddressList(mail.To, message.To, message, HeaderId.To);
            CopyAddressList(mail.CC, message.Cc, message, HeaderId.Cc);
            CopyAddressList(mail.Bcc, message.Bcc, message, HeaderId.Bcc);

            // Set the message subject
            if (mail.SubjectEncoding != null)
                message.Headers.Replace(HeaderId.Subject, mail.SubjectEncoding, mail.Subject ?? string.Empty);
            else
                message.Subject = mail.Subject ?? string.Empty;

            // Adjust the message priority
            AdjustMessagePriority(mail.Priority, message);

            // Set the message body, with alternative views and attachments (if any)
            var body = GetBodyOrNull(mail);
            body = AppendAlternativeViews(mail, body) ?? body;
            body = AppendAttachments(mail, body) ?? body;

            message.Body = body ?? new TextPart(mail.IsBodyHtml ? "html" : "plain");

            // Done
            return message;
        }
        
        #endregion

        private static void CopyAddressList(MailAddressCollection sourceList, InternetAddressList targetList,
            MimeMessage message, HeaderId headerId)
        {
            if (sourceList == null || sourceList.Count == 0)
                return;

            message.Headers.Replace(headerId, string.Empty);
            targetList.AddRange(sourceList.Select(ToMailboxAddress).Where(t => t != null));
        }

        private static MailboxAddress ToMailboxAddress(MailAddress mailAddress) => mailAddress == null
            ? null
            : new MailboxAddress(mailAddress.DisplayName, mailAddress.Address);
        
        #region AdjustMessagePriority

        private static void AdjustMessagePriority(MailPriority priority, MimeMessage message)
        {
            switch (priority)
            {
                case MailPriority.High:
                    message.Headers.Replace(HeaderId.Priority, "urgent");
                    message.Headers.Replace(HeaderId.Importance, "high");
                    message.Headers.Replace(HeaderId.XPriority, "2 (High)");
                    break;
                case MailPriority.Low:
                    message.Headers.Replace(HeaderId.Priority, "non-urgent");
                    message.Headers.Replace(HeaderId.Importance, "low");
                    message.Headers.Replace(HeaderId.XPriority, "4 (Low)");
                    break;
                case MailPriority.Normal:
                    message.Headers.RemoveAll(HeaderId.XMSMailPriority);
                    message.Headers.RemoveAll(HeaderId.Importance);
                    message.Headers.RemoveAll(HeaderId.XPriority);
                    message.Headers.RemoveAll(HeaderId.Priority);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion

        private static MimeEntity GetBodyOrNull(MailMessage mail)
        {
            if (string.IsNullOrEmpty(mail.Body))
                return null;

            var text = new TextPart(mail.IsBodyHtml ? "html" : "plain");
            text.SetText(mail.BodyEncoding ?? Encoding.UTF8, mail.Body);
            return text;
        }
        
        #region AppendAlternativeViews

        private static MimeEntity AppendAlternativeViews(MailMessage mail, MimeEntity originalBody)
        {
            if (mail.AlternateViews.Count == 0)
                return null;

            var body = new MultipartAlternative();
            if (originalBody != null)
                body.Add(originalBody);

            foreach (var alternativeView in mail.AlternateViews)
            {
                var part = GetMimePart(alternativeView);
                if (alternativeView.BaseUri != null)
                    part.ContentLocation = alternativeView.BaseUri;

                // Determine if the alternative view have linked resources
                if (alternativeView.LinkedResources.Count == 0)
                {
                    body.Add(part);
                    continue;
                }

                // Create a multipart relation
                var type = part.ContentType.MediaType + "/" + part.ContentType.MediaSubtype;
                var related = new MultipartRelated();

                related.ContentType.Parameters.Add("type", type);
                if (alternativeView.BaseUri != null)
                    related.ContentLocation = alternativeView.BaseUri;

                related.Add(part);

                // Add all the linked resources to the relation
                foreach (var resource in alternativeView.LinkedResources)
                {
                    part = GetMimePart(resource);
                    if (resource.ContentLink != null)
                        part.ContentLocation = resource.ContentLink;

                    related.Add(part);
                }

                // Append the linked resource to the alternative view
                body.Add(related);
            }

            return body;
        }
        
        #endregion

        private static MimeEntity AppendAttachments(MailMessage mail, MimeEntity originalBody)
        {
            if (mail.Attachments.Count == 0)
                return null;

            var body = new Multipart("mixed");
            if (originalBody != null)
                body.Add(originalBody);

            foreach (var attachment in mail.Attachments)
                body.Add(GetMimePart(attachment));

            return body;
        }
        
        #region GetMimePart

        private static MimePart GetMimePart(AttachmentBase item)
        {
            var mimeType = item.ContentType.ToString();
            var contentType = ContentType.Parse(mimeType);
            var attachment = item as Attachment;
            var part = new MimePart(contentType);

            //
            if (attachment != null)
            {
                var disposition = attachment.ContentDisposition.ToString();
                part.ContentDisposition = ContentDisposition.Parse(disposition);
            }

            // Adjust the transfer encoding
            switch (item.TransferEncoding)
            {
                case TransferEncoding.QuotedPrintable:
                    part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                    break;
                case TransferEncoding.Base64:
                    part.ContentTransferEncoding = ContentEncoding.Base64;
                    break;
                case TransferEncoding.SevenBit:
                    part.ContentTransferEncoding = ContentEncoding.SevenBit;
                    break;
                case TransferEncoding.EightBit:
                    part.ContentTransferEncoding = ContentEncoding.EightBit;
                    break;
                case TransferEncoding.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Adjust the attachment content identifier
            if (item.ContentId != null)
                part.ContentId = item.ContentId;

            // Copy the content of the attachment
            var stream = new MemoryBlockStream();
            item.ContentStream.CopyTo(stream);
            stream.Position = 0;

            part.Content = new MimeContent(stream);

            // Done
            return part;
        }
        
        #endregion

    }

}