using Microsoft.AspNetCore.Mvc;

namespace Card_Reader_Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DocumentInteligenceController : ControllerBase
    {
        /// <summary>
        /// Gets the details from the card sent
        /// </summary>
        /// <param name="file">Image of the card</param>
        /// <param name="url"> Url to the image to be treated </param>
        /// <returns>A <see cref="string"/> containing the pokemon name and a <seealso cref="string"/> containg the Pokemon ID </returns>
        /// <exception cref="ArgumentNullException"> both file and url are null</exception>
        /// <exception cref="NotImplementedException">Not ready for HEIF</exception>
        [HttpPost(Name = "GetCardDetailsPost")]
        public async Task<(string, string)> GetCardDetails([FromBody] IFormFile? file, [FromQuery( Name = "Url")] string? url)
        {
            string PokeName = "", PokeID = "";
            if (string.IsNullOrEmpty(url) && file is null)
                throw new("File and url can't both be null");
            else if (!string.IsNullOrEmpty(url))
            //url is not null
            {

            }
            else if (file is not null)
            //file is not null
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                switch (ext)
                {
                    case "png":
                    case "jpg":
                        break;
                    case "heif":
                        throw new NotImplementedException("HEIF not ");
                    default:
                        throw new("only takes png and jpg");
                }
            }

            await Task.Delay(1);
            return (PokeName, PokeID);
        }

    }
}