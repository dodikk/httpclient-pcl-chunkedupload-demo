

namespace HttpClientTestiOS
{
  using System;
  using System.IO;

  using NUnit.Framework;


  using PortableUploader;


  [TestFixture]
  public class PostRequestInitializationTest
  {
    Stream dataToUpload;

    [SetUp]
    public void SetUp()
    {
      const string helloWorld = "Hello HttpClient PCL";
      byte[] data = System.Text.Encoding.UTF8.GetBytes(helloWorld);

      this.dataToUpload = new MemoryStream(data);
    }

    [TearDown]
    public void TearDown()
    {
      this.dataToUpload.Dispose();
      this.dataToUpload = null;
    }

    [Test]
    public async void TestSettingTransferEncodingExplicitlyIsExpectedToWork()
    {
      var options = new TestTweaks();
      {
        options.ShouldAddChunkedEncodingExplicitly = true;
        options.ShouldAddChunkedEncodingProperty = false;
      }

      Stream result = await FakeChunkedLoader.UploadAsync(this.dataToUpload, "HelloWorld.txt", "text/plain", options);
      Assert.IsNotNull(result);
    }


    [Test]
    public async void TestSettingTransferEncodingFromHeaderIsExpectedToWork()
    {
      var options = new TestTweaks();
      {
        options.ShouldAddChunkedEncodingExplicitly = false;
        options.ShouldAddChunkedEncodingProperty = true;
      }

      Stream result = await FakeChunkedLoader.UploadAsync(this.dataToUpload, "HelloWorld.txt", "text/plain", options);
      Assert.IsNotNull(result);
    }


    [Test]
    public async void Test_ContentLengthHeader_IsAdded_NoChunkedEncoding()
    {
      var options = new TestTweaks();
      {
        options.ShouldAddChunkedEncodingExplicitly = false;
        options.ShouldAddChunkedEncodingProperty = false;
      }


      // No way to verify content-length header without a sniffer.
      // See comments below for headers of the sent package
      Stream result = await FakeChunkedLoader.UploadAsync(this.dataToUpload, "HelloWorld.txt", "text/plain", options);
      Assert.IsNotNull(result);


//      POST /-/item/v1/sitecore/Media%20Library/NewItem HTTP/1.1
//      X-Scitemwebapi-Username: sitecore\uploaderuser
//      X-Scitemwebapi-Password: uploaderuser
//      Content-Type: multipart/form-data; boundary="beed9363-58d5-4ead-96a1-1ff3dca06d0d"
//        Content-Length: 203
//        Connection: keep-alive
//        Host: scmobileteam.sitecoretest.net
    }
  
    [Test]
    public void TestChunkedEncodingSupportIsExpected()
    {
      Assert.IsTrue( FakeChunkedLoader.IsChunkedEncodingSupported() );
    }
  }
}
