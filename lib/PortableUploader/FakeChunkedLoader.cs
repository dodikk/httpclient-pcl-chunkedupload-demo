namespace PortableUploader
{
  using System;
  using System.Threading.Tasks;
  using System.Net.Http;
  using System.IO;
  using System.Net.Http.Headers;


  public static class FakeChunkedLoader
  {
    public static async Task<Stream> UploadAsync(
      Stream data, 
      string fileName,
      string mimeType,
      TestTweaks tweaks)
    {
      string url = "http://scmobileteam.sitecoretest.net/-/item/v1/sitecore/Media%20Library";
      string userName = "sitecore\\uploaderuser";
      string password = "uploaderuser";


      HttpRequestMessage result = new HttpRequestMessage(HttpMethod.Post, url);

      MultipartFormDataContent multiPartContent = new MultipartFormDataContent();

      StreamContent strContent = new StreamContent(data);

      ContentDispositionHeaderValue cdHeaderValue = new ContentDispositionHeaderValue("data");
      cdHeaderValue.FileName = "\"" + fileName + "\"";
      cdHeaderValue.Name = "\"datafile\"";
      strContent.Headers.ContentDisposition = cdHeaderValue;

      if (null != mimeType)
      {
        strContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
      }

      multiPartContent.Add(strContent);
      result.Content = multiPartContent;



      // TODO : find a right way of setting the chunked encoding
      {
        if (tweaks.ShouldAddChunkedEncodingProperty)
        {
          // causes System.Net.ProtocolViolationException
          result.Headers.TransferEncodingChunked = true;
        }
        else if (tweaks.ShouldAddChunkedEncodingExplicitly)
        {
          // causes System.Net.ProtocolViolationException
          result.Headers.Add("Transfer-Encoding", "chunked");
        }
      }


      // authentication
      {
        result.Headers.Add("X-Scitemwebapi-Username", userName);
        result.Headers.Add("X-Scitemwebapi-Password", password);
      }


      HttpClient sender = new HttpClient();
      HttpResponseMessage response = await sender.SendAsync(result);

      return await response.Content.ReadAsStreamAsync();
    }
  
  }
}

