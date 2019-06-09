// TODO: get AmazonS3 from Nuget

using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;

namespace SearchSpringFiles
{
    class Program
    {
        private static IAmazonS3 _s3client;
        private const string _bucketName = "<bucket-name-here>";
        private const string _accessKeyId = "<access-key-id-here>";
        private const string _secretAccessKey = "<secret-access-key-here>";
        private static readonly RegionEndpoint _bucketRegion = RegionEndpoint.USWest2;


        static void Main(string[] args)
        {
            _s3client = new AmazonS3Client(_accessKeyId, _secretAccessKey ,_bucketRegion);
            ListingObjectsAsync().Wait();

            Console.ReadLine();
        }

        static async Task ListingObjectsAsync()
        {
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await _s3client.ListObjectsV2Async(request);

                    // Process the response.
                    foreach (S3Object entry in response.S3Objects)
                    {
                        Console.WriteLine("key = {0} size = {1} lastModified = {2}",
                            entry.Key, entry.Size, entry.LastModified);
                    }
                    Console.WriteLine("Next Continuation Token: {0}", response.NextContinuationToken);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                Console.WriteLine("S3 error occurred. Exception: " + amazonS3Exception.ToString());
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                Console.ReadKey();
            }
        }
    }
}
