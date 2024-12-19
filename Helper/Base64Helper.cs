using System.Text.RegularExpressions;

namespace MovieTicketBookingSystem.Helper
{
    public class Base64Helper
    {
        /// <summary>
        /// Converts a byte array (e.g., from an image file) to a Base64 string.
        /// </summary>
        /// <param name="imageBytes">Byte array representing the image.</param>
        /// <returns>Base64 encoded string of the image.</returns>
        public static string ConvertToBase64(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("Image bytes cannot be null or empty.");
            }

            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        /// <summary>
        /// Decodes a Base64 string to a byte array.
        /// </summary>
        /// <param name="base64String">Base64 encoded string.</param>
        /// <returns>Byte array of the decoded data.</returns>
        public static byte[] ConvertFromBase64(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                throw new ArgumentException("Base64 string cannot be null or empty.");
            }

            // Remove Base64 header if it exists
            string cleanedBase64String = Regex.Replace(base64String, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);

            try
            {
                byte[] bytes = Convert.FromBase64String(cleanedBase64String);
                return bytes;
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid Base64 string format.", ex);
            }
        }
    }

}
