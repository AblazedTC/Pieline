using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PieLine
{
    public static class CommonHelpers
    {
        public static void PersistantWindows(Window current, Window next)
        {
            next.WindowStartupLocation = WindowStartupLocation.Manual;
            next.Left = current.Left;
            next.Top = current.Top;
            next.Width = current.Width;
            next.Height = current.Height;
            next.WindowState = current.WindowState;

            next.Show();
            current.Close();
        }

        public static void SetError(TextBlock errorBlock, string message)
        {
            if (errorBlock == null) return;

            if (string.IsNullOrEmpty(message))
            {
                errorBlock.Visibility = Visibility.Collapsed;
                errorBlock.Text = "";
            }
            else
            {
                errorBlock.Text = message;
                errorBlock.Visibility = Visibility.Visible;
            }
        }

        public static void HandlePhoneTextChanged(TextBox textBox, ref bool isUpdating)
        {
            if (textBox == null) return;
            if (isUpdating) return;

            isUpdating = true;

            string allowed = "0123456789- ()";
            string original = textBox.Text;

            int digitsSeen = 0;
            var chars = original.Where(c =>
            {
                if (char.IsDigit(c))
                {
                    if (digitsSeen >= 10) return false;
                    digitsSeen++;
                    return true;
                }
                return allowed.Contains(c);
            }).ToArray();

            string filtered = new string(chars);

            if (filtered != original)
            {
                textBox.Text = filtered;
                textBox.CaretIndex = filtered.Length;
            }

            isUpdating = false;
        }

        public static string ExtractDigits(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return new string(text.Where(char.IsDigit).ToArray());
        }
    }
}