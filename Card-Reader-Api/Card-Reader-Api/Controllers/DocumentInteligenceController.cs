using Microsoft.AspNetCore.Mvc;

namespace Card_Reader_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentInteligenceController : ControllerBase
    {
        /// <summary>
        /// Gets the details from the card sent
        /// </summary>
        /// <returns>A <see cref="string"/> containing the pokemon name and a <seealso cref="string"/> containg the Pokemon ID </returns>
        [HttpPost(Name = "GetCardDetailsPost")]
        public async Task<(string,string)> GetCardDetails([FromForm]IFormFile? file, string? url)
        {
            if (string.IsNullOrEmpty(url) && file is null)
                throw new ArgumentNullException(nameof(url));
            var ext = Path.GetExtension(file.FileName).ToLower();
            switch (ext)
            {
                case "png":
                case "jpg":
                    break;
                case "heif":
                    throw new NotImplementedException("HEIF not ");
                default:
                   throw new ("only takes png and jpg");
            }
            string PokeName = "", PokeID = "";
            await Task.Delay(1);



            return (PokeName,PokeID);
        }

    }
}