using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;

namespace AsyncIO
{
    public static class Tasks
    {


        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the synchronous way and can be used to compare the performace of sync \ async approaches. 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContent(this IEnumerable<Uri> uris)
        {
            var webClient = new WebClient();
            return uris.Select(url => webClient.DownloadString(url));
        }



        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the asynchronous way and can be used to compare the performace of sync \ async approaches. 
        /// 
        /// maxConcurrentStreams parameter should control the maximum of concurrent streams that are running at the same time (throttling). 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <param name="maxConcurrentStreams">Max count of concurrent request streams</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContentAsync(this IEnumerable<Uri> uris, int maxConcurrentStreams)
        {          
            var getEnum = uris.GetEnumerator();
            var uriTasks = new List<Task<string>>(uris.Take(maxConcurrentStreams)
                .Select(x => new WebClient().DownloadStringTaskAsync(x)));          
            while (uriTasks.Count > 0)
            {
                yield return uriTasks[0].Result;
                uriTasks.RemoveAt(0);
                if (getEnum.MoveNext())
                {
                    uriTasks.Add(new WebClient().DownloadStringTaskAsync(getEnum.Current));
                }
            }
        }


        /// <summary>
        /// Calculates MD5 hash of required resource.
        /// 
        /// Method has to run asynchronous. 
        /// Resource can be any of type: http page, ftp file or local file.
        /// </summary>
        /// <param name="resource">Uri of resource</param>
        /// <returns>MD5 hash</returns>
        public async static Task<string> GetMD5Async(this Uri resource)
        {       
			using (Stream stream = await new WebClient().OpenReadTaskAsync(resource))
			{
				return await HashFromStreamAsync(stream);
			}
		}

        private static async Task<string> HashFromStreamAsync(Stream stream)
        {
            MD5 md5 = MD5.Create();
            byte[] streamBuffer = new byte[4096];
            int bytes;

            md5.Initialize();

            while ((bytes = await stream.ReadAsync(streamBuffer, 0, 4096)) > 0)
            {
                md5.TransformBlock(streamBuffer, 0, bytes, null, 0);
            }
            md5.TransformFinalBlock(new byte[0], 0, 0);
            return BitConverter.ToString(md5.Hash).Replace("-", string.Empty);
        }
    }
}
