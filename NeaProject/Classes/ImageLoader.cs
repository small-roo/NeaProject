using SkiaSharp;

namespace NeaProject.Classes
{
    public class ImageLoader
    {
        private readonly Uri _uri;

        public ImageLoader(Uri uri)
        {
            _uri = uri;
        }
        public async Task<SKBitmap> GetBitmapAsync()
        {
            // create an http client
            using HttpClient client = new();

            // getting a response message for the uri
            HttpResponseMessage response = await client.GetAsync(_uri);

            // makes sure the response message is one that allows the program to continue
            // throws an exception otherwise
            response.EnsureSuccessStatusCode();

            // stream is set to webserver's response as a stream
            Stream stream = await response.Content.ReadAsStreamAsync();

            // converting from a stream into a variable I can pass through SkiaSharp methods
            SKData encodedStream = SKData.Create(stream);

            // https://stackoverflow.com/questions/65820269/how-do-ioad-an-image-from-a-file-and-draw-it-on-a-wpf-skiasharp-canvas
            using var image = SKImage.FromEncodedData(encodedStream);
            var bitmap = SKBitmap.FromImage(image);

            return bitmap;
        }
    }
}
