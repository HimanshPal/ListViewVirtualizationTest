using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace VirtualizationTest.Services
{
    public sealed class ToastService
    {
        public void ShowToast(string content)
        {
            // show toast
            var toast = BuildToastText01(content);
            toast.ExpirationTime = DateTimeOffset.Now.AddSeconds(1);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }

        public void ShowToast(string title, string content, bool? isNotificationCenterPushRequested = null)
        {
            ToastNotification toast;
            if (isNotificationCenterPushRequested == true)
            {
                toast = BuildToastText02(title, content, isSoundRequested: true);
                toast.ExpirationTime = DateTimeOffset.Now.AddHours(1);
            }
            else
            {
                toast = BuildToastText02(title, content);
                toast.ExpirationTime = DateTimeOffset.Now.AddSeconds(1);
            }

            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }

        private static ToastNotification BuildToastText01(string content, bool isSoundRequested = false)
        {
            // build toast
            const ToastTemplateType template = ToastTemplateType.ToastText01;
            var xml = ToastNotificationManager.GetTemplateContent(template);
            IXmlNode toastNode = xml.SelectSingleNode("/toast");
            var elements = xml.GetElementsByTagName("text");

            // content
            var text = xml.CreateTextNode(content);
            elements[0].AppendChild(text);

            if (!isSoundRequested)
            {
                var audio = xml.CreateElement("audio");
                audio.SetAttribute("silent", "true");
                toastNode.AppendChild(audio);
            }

            // toast
            return new ToastNotification(xml);
        }

        private static ToastNotification BuildToastText02(string title, string content, bool isSoundRequested = false)
        {
            // build toast
            const ToastTemplateType template = ToastTemplateType.ToastText02;
            var xml = ToastNotificationManager.GetTemplateContent(template);
            IXmlNode toastNode = xml.SelectSingleNode("/toast");
            var elements = xml.GetElementsByTagName("text");

            // title
            var text = xml.CreateTextNode(title);
            elements[0].AppendChild(text);

            // content
            text = xml.CreateTextNode(content);
            elements[1].AppendChild(text);

            if (!isSoundRequested)
            {
                var audio = xml.CreateElement("audio");
                audio.SetAttribute("silent", "true");
                toastNode.AppendChild(audio);
            }

            // sh toast
            return new ToastNotification(xml);
        }
    }
}
